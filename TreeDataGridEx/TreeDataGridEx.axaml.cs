using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Markup.Xaml.MarkupExtensions;

namespace TreeDataGridEx;

public class TreeDataGridEx : TemplatedControl
{
    public static readonly StyledProperty<IEnumerable> ItemsSourceProperty =
        AvaloniaProperty.Register<TreeDataGridEx, IEnumerable>(nameof(ItemsSource));

    public static readonly StyledProperty<ObservableCollection<TreeDataGridColumn>?> ColumnsProperty = 
        AvaloniaProperty.Register<TreeDataGridEx, ObservableCollection<TreeDataGridColumn>?>(nameof(Columns));

    private ITreeDataGridSource? _source;
    private TreeDataGrid? _treeDataGrid;

    public IEnumerable ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public ObservableCollection<TreeDataGridColumn>? Columns
    {
        get => GetValue(ColumnsProperty);
        set => SetValue(ColumnsProperty, value);
    }

    public TreeDataGrid? TreeDataGrid => _treeDataGrid;

    public TreeDataGridEx()
    {
        SetCurrentValue(ColumnsProperty, new ObservableCollection<TreeDataGridColumn>());
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _treeDataGrid = e.NameScope.Find<TreeDataGrid>("PART_TreeDataGrid");

        var itemsSource = ItemsSource;
        var columns = Columns;

        if (_treeDataGrid is null || columns is null)
        {
            return;
        }

        var source = CreateSource(itemsSource, columns);
        if (source is not null)
        {
            _source = source;
            _treeDataGrid.Source = _source;
        }
    }

    private static ITreeDataGridSource? CreateSource(IEnumerable items, ObservableCollection<TreeDataGridColumn> columns)
    {
        var modelType = items.GetType().GenericTypeArguments[0];

        var source = columns.Any(x => x is TreeDataGridHierarchicalExpanderColumn)
            ? CreateHierarchicalSource(modelType, items)
            : CreateFlatSource(modelType, items);
        if (source is null)
        {
            return null;
        }

        var columnsType = typeof(ColumnList<>).MakeGenericType(modelType);
        var add = columnsType.GetMethod("Add");
        if (add is null)
        {
            return null;
        }

        foreach (var column in columns)
        {
            var c = CreateColumn(modelType, column);
            if (c is not null)
            {
                add.Invoke(source.Columns, new object[] { c });
            }
        }

        return source;
    }

    private static ITreeDataGridSource? CreateFlatSource(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] 
        Type modelType,
        IEnumerable items)
    {
        var type = typeof(FlatTreeDataGridSource<>).MakeGenericType(modelType);
        return (ITreeDataGridSource?)Activator.CreateInstance(type, items);
    }

    private static ITreeDataGridSource? CreateHierarchicalSource(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] 
        Type modelType,
        IEnumerable items)
    {
        var type = typeof(HierarchicalTreeDataGridSource<>).MakeGenericType(modelType);
        return (ITreeDataGridSource?)Activator.CreateInstance(type, items);
    }

    private static IColumn? CreateColumn(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] 
        Type modelType,
        TreeDataGridColumn column)
    {
        switch (column)
        {
            case TreeDataGridTemplateColumn templateColumn:
            {
                if (templateColumn.CellTemplate is null)
                {
                    return null;
                }
                return CreateTemplateColumn(
                    modelType,
                    templateColumn.Header,
                    templateColumn.CellTemplate,
                    templateColumn.CellEditingTemplate,
                    templateColumn.Width);
            }
            case TreeDataGridTextColumn textColumn:
            {
                if (textColumn.Binding is null)
                {
                    return null;
                }
                return CreateTextColumn(
                    modelType,
                    textColumn.Header,
                    textColumn.Binding,
                    textColumn.Width);
            }
            case TreeDataGridCheckBoxColumn checkBoxColumn:
            {
                if (checkBoxColumn.Binding is null)
                {
                    return null;
                }
                return CreateCheckBoxColumn(
                    modelType,
                    checkBoxColumn.Header,
                    checkBoxColumn.Binding,
                    checkBoxColumn.Width);
            }
            case TreeDataGridHierarchicalExpanderColumn hierarchicalExpanderColumn:
            {
                if (hierarchicalExpanderColumn.ChildrenName is null)
                {
                    return null;
                }
                var inner = hierarchicalExpanderColumn.Inner is not null 
                    ? CreateColumn(modelType, hierarchicalExpanderColumn.Inner)
                    : null;
                return CreateHierarchicalExpanderColumn(
                    modelType,
                    inner,
                    hierarchicalExpanderColumn.ChildrenName);
            }
            default:
            {
                return null;
            }
        }
    }

    private static IColumn? CreateHierarchicalExpanderColumn(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] 
        Type modelType,
        IColumn? inner, 
        string childrenName)
    {
        var property = modelType.GetProperty(childrenName);
        if (property is null)
        {
            return null;
        }
        var childSelector = CreateChildSelectorLambdaExpression(modelType, property).Compile();
        var type = typeof(HierarchicalExpanderColumn<>).MakeGenericType(modelType);
        return (IColumn?) Activator.CreateInstance(type, inner, childSelector, null, null);
    }

    private static IColumn? CreateTemplateColumn(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        Type modelType,
        object? header,
        IDataTemplate cellTemplate,
        IDataTemplate? cellEditingTemplate = null,
        GridLength? width = null)
    {
        var type = typeof(TemplateColumn<>).MakeGenericType(modelType);
        return (IColumn?) Activator.CreateInstance(type, header, cellTemplate, cellEditingTemplate, width, null);
    }

    private static IColumn? CreateTextColumn(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] 
        Type modelType,
        object? header,
        IBinding binding,
        GridLength? width = null)
    {
        var path = (binding as Binding)?.Path ?? (binding as CompiledBindingExtension)?.Path.ToString();
        if (path is null)
        {
            return null;
        }
        var property = modelType.GetProperty(path);
        if (property is null)
        {
            return null;
        }
        var propertyType = property.PropertyType;
        var getter = CreateGetterLambdaExpression(modelType, property);
        var type = typeof(TextColumn<,>).MakeGenericType(modelType, propertyType);
        if (!property.CanWrite || (property.SetMethod is not null && !property.SetMethod.IsPublic))
        {
            return (IColumn?) Activator.CreateInstance(type, header, getter, width, null);
        }
        var setter = CreateSetterLambdaExpression(modelType, property).Compile();
        return (IColumn?) Activator.CreateInstance(type, header, getter, setter, width, null);
    }

    private static IColumn? CreateCheckBoxColumn(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] 
        Type modelType,
        object? header,
        IBinding? binding,
        GridLength? width = null)
    {
        var path = (binding as Binding)?.Path ?? (binding as CompiledBindingExtension)?.Path.ToString();
        if (path is null)
        {
            return null;
        }
        var property = modelType.GetProperty(path);
        if (property is null)
        {
            return null;
        }
        var getter = CreateGetterLambdaExpression(modelType, property);
        var type = typeof(CheckBoxColumn<>).MakeGenericType(modelType);
        if (!property.CanWrite || (property.SetMethod is not null && !property.SetMethod.IsPublic))
        {
            return (IColumn?) Activator.CreateInstance(type, header, getter, width, null);
        }
        var setter = CreateSetterLambdaExpression(modelType, property).Compile();
        return (IColumn?) Activator.CreateInstance(type, header, getter, setter, width, null);
    }

    private static LambdaExpression CreateGetterLambdaExpression(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] 
        Type modelType, 
        PropertyInfo property)
    {
        var valueType = property.PropertyType;
        var modelParameter = Expression.Parameter(modelType, "model");
        var propertyAccess = Expression.Property(modelParameter, property);
        var convertedPropertyAccess = Expression.Convert(propertyAccess, valueType);
        var lambdaType = typeof(Func<,>).MakeGenericType(modelType, valueType);
        return Expression.Lambda(lambdaType, convertedPropertyAccess, modelParameter);
    }

    private static LambdaExpression CreateSetterLambdaExpression(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] 
        Type modelType, 
        PropertyInfo property)
    {
        var valueType = property.PropertyType;
        var modelParameter = Expression.Parameter(modelType, "model");
        var valueParameter = Expression.Parameter(valueType, "value");
        var propertyAccess = Expression.Property(modelParameter, property);
        var assign = Expression.Assign(propertyAccess, Expression.Convert(valueParameter, property.PropertyType));
        var lambdaType = typeof(Action<,>).MakeGenericType(modelType, valueType);
        return Expression.Lambda(lambdaType, assign, modelParameter, valueParameter);
    }

    private static LambdaExpression CreateChildSelectorLambdaExpression(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] 
        Type modelType, 
        PropertyInfo property)
    {
        var valueType = typeof(IEnumerable<>).MakeGenericType(modelType);
        var modelParameter = Expression.Parameter(modelType, "model");
        var propertyAccess = Expression.Property(modelParameter, property);
        var convertedPropertyAccess = Expression.Convert(propertyAccess, valueType);
        var lambdaType = typeof(Func<,>).MakeGenericType(modelType, valueType);
        return Expression.Lambda(lambdaType, convertedPropertyAccess, modelParameter);
    }
}
