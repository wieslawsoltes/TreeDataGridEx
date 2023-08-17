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
using Avalonia.Data;
using Avalonia.Markup.Xaml.MarkupExtensions;

namespace TreeDataGridEx;

public class TreeDataGridEx : TemplatedControl
{
    public static readonly StyledProperty<bool> AutoDragDropRowsProperty =
        TreeDataGrid.AutoDragDropRowsProperty.AddOwner<TreeDataGridEx>();

    public static readonly StyledProperty<bool> CanUserResizeColumnsProperty =
        TreeDataGrid.CanUserResizeColumnsProperty.AddOwner<TreeDataGridEx>();

    public static readonly StyledProperty<bool> CanUserSortColumnsProperty =
        TreeDataGrid.CanUserSortColumnsProperty.AddOwner<TreeDataGridEx>();

    public static readonly StyledProperty<ObservableCollection<TreeDataGridColumn>?> ColumnsProperty = 
        AvaloniaProperty.Register<TreeDataGridEx, ObservableCollection<TreeDataGridColumn>?>(nameof(Columns));

    public static readonly StyledProperty<bool> ShowColumnHeadersProperty =
        TreeDataGrid.ShowColumnHeadersProperty.AddOwner<TreeDataGridEx>();

    public static readonly StyledProperty<IEnumerable> ItemsSourceProperty =
        AvaloniaProperty.Register<TreeDataGridEx, IEnumerable>(nameof(ItemsSource));

    private ITreeDataGridSource? _source;
    private TreeDataGrid? _treeDataGrid;

    public bool AutoDragDropRows
    {
        get => GetValue(AutoDragDropRowsProperty);
        set => SetValue(AutoDragDropRowsProperty, value);
    }

    public bool CanUserResizeColumns
    {
        get => GetValue(CanUserResizeColumnsProperty);
        set => SetValue(CanUserResizeColumnsProperty, value);
    }

    public bool CanUserSortColumns
    {
        get => GetValue(CanUserSortColumnsProperty);
        set => SetValue(CanUserSortColumnsProperty, value);
    }

    public ObservableCollection<TreeDataGridColumn>? Columns
    {
        get => GetValue(ColumnsProperty);
        set => SetValue(ColumnsProperty, value);
    }

    public bool ShowColumnHeaders
    {
        get => GetValue(ShowColumnHeadersProperty);
        set => SetValue(ShowColumnHeadersProperty, value);
    }

    public IEnumerable ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public ITreeDataGridSource? Source => _source;

    public TreeDataGrid? TreeDataGrid => _treeDataGrid;

    
    public TreeDataGridEx()
    {
        SetCurrentValue(ColumnsProperty, new ObservableCollection<TreeDataGridColumn>());
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _treeDataGrid = e.NameScope.Find<TreeDataGrid>("PART_TreeDataGrid");

        Initialize();
    }

    private void Initialize()
    {
        (_source as IDisposable)?.Dispose();

        var itemsSource = ItemsSource;
        var columns = Columns;

        if (_treeDataGrid is null || columns is null)
        {
            return;
        }
/*
        var source = CreateSource(itemsSource, columns);
        if (source is not null)
        {
            _source = source;
            _treeDataGrid.Source = _source;
        }*/
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ItemsSourceProperty || change.Property == ColumnsProperty)
        {
            Initialize();
        }
    }
/*
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ColumnList<>))]
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
            try
            {
                var c = CreateColumn(column.DataType ?? modelType, column);
                if (c is not null)
                {
                    add.Invoke(source.Columns, new object[] { c });
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        return source;
    }

    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(FlatTreeDataGridSource<>))]
    private static ITreeDataGridSource? CreateFlatSource(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] 
        Type modelType,
        IEnumerable items)
    {
        var type = typeof(FlatTreeDataGridSource<>).MakeGenericType(modelType);

        return (ITreeDataGridSource?)Activator.CreateInstance(type, items);
    }

    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(HierarchicalExpanderColumn<>))]
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
                    templateColumn);
            }
            case TreeDataGridTextColumn textColumn:
            {
                if (textColumn.Binding is null)
                {
                    return null;
                }

                return CreateTextColumn(
                    modelType,
                    textColumn);
            }
            case TreeDataGridCheckBoxColumn checkBoxColumn:
            {
                if (checkBoxColumn.Binding is null)
                {
                    return null;
                }

                return CreateCheckBoxColumn(
                    modelType,
                    checkBoxColumn);
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

    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(HierarchicalExpanderColumn<>))]
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

        // TODO:
        // - hasChildrenSelector
        // - isExpandedSelector
        return (IColumn?) Activator.CreateInstance(type, inner, childSelector, null, null);
    }

    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(TemplateColumn<>))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ColumnOptions<>))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(TemplateColumnOptions<>))]
    private static IColumn? CreateTemplateColumn(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        Type modelType,
        TreeDataGridTemplateColumn templateColumn)
    {
        var header = templateColumn.Header;
        var cellTemplate = templateColumn.CellTemplate;
        var cellEditingTemplate = templateColumn.CellEditingTemplate;
        var width = templateColumn.Width;

        var type = typeof(TemplateColumn<>).MakeGenericType(modelType);

        var optionsType = typeof(TemplateColumnOptions<>).MakeGenericType(modelType);
        var options = Activator.CreateInstance(optionsType);

        // ColumnOptions
        optionsType.GetProperty("CanUserResizeColumn")?.SetValue(options, templateColumn.CanUserResizeColumn);
        optionsType.GetProperty("CanUserSortColumn")?.SetValue(options, templateColumn.CanUserSortColumn);
        optionsType.GetProperty("MinWidth")?.SetValue(options, templateColumn.MinWidth);
        optionsType.GetProperty("MaxWidth")?.SetValue(options, templateColumn.MaxWidth);
        // TODO: CompareAscending
        // TODO: CompareDescending
        optionsType.GetProperty("BeginEditGestures")?.SetValue(options, templateColumn.BeginEditGestures);

        // TemplateColumnOptions
        // - IsTextSearchEnabled
        // - TextSearchValueSelector

        return (IColumn?) Activator.CreateInstance(type, header, cellTemplate, cellEditingTemplate, width, options);
    }

    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(TextColumn<,>))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ColumnOptions<>))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(TextColumnOptions<>))]
    private static IColumn? CreateTextColumn(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] 
        Type modelType,
        TreeDataGridTextColumn textColumn)
    {
        var header = textColumn.Header;
        var binding = textColumn.Binding;
        var width = textColumn.Width;

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

        var optionsType = typeof(TextColumnOptions<>).MakeGenericType(modelType);
        var options = Activator.CreateInstance(optionsType);

        // ColumnOptions
        optionsType.GetProperty("CanUserResizeColumn")?.SetValue(options, textColumn.CanUserResizeColumn);
        optionsType.GetProperty("CanUserSortColumn")?.SetValue(options, textColumn.CanUserSortColumn);
        optionsType.GetProperty("MinWidth")?.SetValue(options, textColumn.MinWidth);
        optionsType.GetProperty("MaxWidth")?.SetValue(options, textColumn.MaxWidth);
        // TODO: CompareAscending
        // TODO: CompareDescending
        optionsType.GetProperty("BeginEditGestures")?.SetValue(options, textColumn.BeginEditGestures);

        // TextColumnOptions
        optionsType.GetProperty("IsTextSearchEnabled")?.SetValue(options, textColumn.IsTextSearchEnabled);
        optionsType.GetProperty("TextTrimming")?.SetValue(options, textColumn.TextTrimming);
        optionsType.GetProperty("TextWrapping")?.SetValue(options, textColumn.TextWrapping);
    
        if (!property.CanWrite || (property.SetMethod is not null && !property.SetMethod.IsPublic))
        {
            return (IColumn?) Activator.CreateInstance(type, header, getter, width, options);
        }

        var setter = CreateSetterLambdaExpression(modelType, property).Compile();

        return (IColumn?) Activator.CreateInstance(type, header, getter, setter, width, options);
    }

    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(CheckBoxColumn<>))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ColumnOptions<>))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(CheckBoxColumnOptions<>))]
    private static IColumn? CreateCheckBoxColumn(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] 
        Type modelType,
        TreeDataGridCheckBoxColumn checkBoxColumn)
    {
        var header = checkBoxColumn.Header;
        var binding = checkBoxColumn.Binding;
        var width = checkBoxColumn.Width;

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

        var optionsType = typeof(CheckBoxColumnOptions<>).MakeGenericType(modelType);
        var options = Activator.CreateInstance(optionsType);

        // ColumnOptions
        optionsType.GetProperty("CanUserResizeColumn")?.SetValue(options, checkBoxColumn.CanUserResizeColumn);
        optionsType.GetProperty("CanUserSortColumn")?.SetValue(options, checkBoxColumn.CanUserSortColumn);
        optionsType.GetProperty("MinWidth")?.SetValue(options, checkBoxColumn.MinWidth);
        optionsType.GetProperty("MaxWidth")?.SetValue(options, checkBoxColumn.MaxWidth);
        // TODO: CompareAscending
        // TODO: CompareDescending
        optionsType.GetProperty("BeginEditGestures")?.SetValue(options, checkBoxColumn.BeginEditGestures);

        // CheckBoxColumnOptions (none)

        if (!property.CanWrite || (property.SetMethod is not null && !property.SetMethod.IsPublic))
        {
            return (IColumn?) Activator.CreateInstance(type, header, getter, width, options);
        }

        var setter = CreateSetterLambdaExpression(modelType, property).Compile();

        return (IColumn?) Activator.CreateInstance(type, header, getter, setter, width, options);
    }

    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Func<,>))]
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

    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Action<,>))]
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

    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(IEnumerable<>))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Func<,>))]
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
    }*/
}
