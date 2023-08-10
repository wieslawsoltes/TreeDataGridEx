using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;

namespace TreeDataGridEx.Controls;

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

    public TreeDataGridEx()
    {
        SetCurrentValue(ColumnsProperty, new ObservableCollection<TreeDataGridColumn>());
    }
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _treeDataGrid = e.NameScope.Find<TreeDataGrid>("PART_TreeDataGrid");

        if (_treeDataGrid is not null)
        {
            _source = CreateSource();

            var add = _source.Columns.GetType().GetMethod("Add");

            foreach (var column in Columns)
            {
                switch (column)
                {
                    case TreeDataGridTemplateColumn templateColumn:
                    {
                        var c = CreateTemplateColumn(
                            templateColumn.Header,
                            templateColumn.CellTemplate,
                            templateColumn.CellEditingTemplate,
                            templateColumn.Width);
                        add.Invoke(_source.Columns, new object[] { c });
                        break;
                    }
                    case TreeDataGridTextColumn textColumn:
                    {
                        var c = CreateTextColumn(
                            textColumn.Header,
                            textColumn.Name,
                            textColumn.Width);
                        add.Invoke(_source.Columns, new object[] { c });
                        break;
                    }
                    case TreeDataGridCheckBoxColumn checkBoxColumn:
                    {
                        var c = CreateCheckBoxColumn(
                            checkBoxColumn.Header,
                            checkBoxColumn.Name,
                            checkBoxColumn.Width);
                        add.Invoke(_source.Columns, new object[] { c });
                        break;
                    }
                }
            }

            _treeDataGrid.Source = _source;
        }
    }

    private ITreeDataGridSource? CreateSource()
    {
        var flatTreeDataGridSourceType = typeof(FlatTreeDataGridSource<>);
        var modelType = ItemsSource.GetType().GenericTypeArguments[0];
        var type = flatTreeDataGridSourceType.MakeGenericType(modelType);
        return (ITreeDataGridSource?)Activator.CreateInstance(type, new object[] { ItemsSource });
    }

    private IColumn? CreateTemplateColumn(
        object? header,
        IDataTemplate cellTemplate,
        IDataTemplate? cellEditingTemplate = null,
        GridLength? width = null)
    {
        var templateColumnType = typeof(TemplateColumn<>);
        var modelType = ItemsSource.GetType().GenericTypeArguments[0];
        var type = templateColumnType.MakeGenericType(modelType);
        return (IColumn?) Activator.CreateInstance(type, new object[] { header, cellTemplate, cellEditingTemplate, width, null });
    }

    private IColumn? CreateTextColumn(
        object? header,
        string name,
        GridLength? width = null)
    {
        var templateColumnType = typeof(TextColumn<,>);
        var modelType = ItemsSource.GetType().GenericTypeArguments[0];
        var property = modelType.GetProperty(name);
        var propertyType = property.PropertyType;
        var getter = CreateGetterLambdaExpression(modelType, property);
        var setter = CreateSetterLambdaExpression(modelType, property).Compile();
        var type = templateColumnType.MakeGenericType(new Type[] { modelType, propertyType });
        if (!property.CanWrite || (property.SetMethod is not null && !property.SetMethod.IsPublic))
        {
            return (IColumn?) Activator.CreateInstance(type, new object[] { header, getter, width, null });
        }
        return (IColumn?) Activator.CreateInstance(type, new object[] { header, getter, setter, width, null });
    }

    private IColumn? CreateCheckBoxColumn(
        object? header,
        string name,
        GridLength? width = null)
    {
        var templateColumnType = typeof(CheckBoxColumn<>);
        var modelType = ItemsSource.GetType().GenericTypeArguments[0];
        var property = modelType.GetProperty(name);
        var propertyType = property.PropertyType;
        var getter = CreateGetterLambdaExpression(modelType, property);
        var setter = CreateSetterLambdaExpression(modelType, property).Compile();
        var type = templateColumnType.MakeGenericType(new Type[] { modelType });
        if (!property.CanWrite || (property.SetMethod is not null && !property.SetMethod.IsPublic))
        {
            return (IColumn?) Activator.CreateInstance(type, new object[] { header, getter, width, null });
        }
        return (IColumn?) Activator.CreateInstance(type, new object[] { header, getter, setter, width, null });
    }

    private LambdaExpression CreateGetterLambdaExpression(Type modelType, PropertyInfo property)
    {
        var valueType = property.PropertyType;
        var modelParameter = Expression.Parameter(modelType, "model");
        var propertyAccess = Expression.Property(modelParameter, property);
        var convertedPropertyAccess = Expression.Convert(propertyAccess, valueType);
        var lambdaType = typeof(Func<,>).MakeGenericType(modelType, valueType);
        return Expression.Lambda(lambdaType, convertedPropertyAccess, modelParameter);
    }

    private LambdaExpression CreateSetterLambdaExpression(Type modelType, PropertyInfo property)
    {
        var valueType = property.PropertyType;
        var modelParameter = Expression.Parameter(modelType, "model");
        var valueParameter = Expression.Parameter(valueType, "value");
        var propertyAccess = Expression.Property(modelParameter, property);
        var assign = Expression.Assign(propertyAccess, Expression.Convert(valueParameter, property.PropertyType));
        var lambdaType = typeof(Action<,>).MakeGenericType(modelType, valueType);
        return Expression.Lambda(lambdaType, assign, modelParameter, valueParameter);
    }
}
