using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using Avalonia;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Metadata;

namespace TreeDataGridEx;

/// <summary>
/// Represents a hierarchical expander column for a tree data grid.
/// </summary>
public class TreeDataGridHierarchicalExpanderColumn : TreeDataGridColumn
{
    // TODO: HierarchicalExpanderColumn<>.hasChildrenSelector
    // TODO: HierarchicalExpanderColumn<>.isExpandedSelector

    /// <summary>
    /// Identifies the <see cref="Inner"/> dependency property.
    /// </summary>
    public static readonly DirectProperty<TreeDataGridHierarchicalExpanderColumn, TreeDataGridColumn?> InnerProperty =
        AvaloniaProperty.RegisterDirect<TreeDataGridHierarchicalExpanderColumn, TreeDataGridColumn?>(
            nameof(Inner),
            o => o.Inner,
            (o, v) => o.Inner = v);

    /// <summary>
    /// Identifies the <see cref="ChildrenName"/> dependency property.
    /// </summary>
    public static readonly DirectProperty<TreeDataGridHierarchicalExpanderColumn, string?> ChildrenNameProperty =
        AvaloniaProperty.RegisterDirect<TreeDataGridHierarchicalExpanderColumn, string?>(
            nameof(ChildrenName),
            o => o.ChildrenName,
            (o, v) => o.ChildrenName = v);

    private TreeDataGridColumn? _inner;
    private string? _childrenName;

    /// <summary>
    /// Gets or sets the inner column that the expander column wraps.
    /// </summary>
    /// <remarks>
    /// This property is marked with the <see cref="ContentAttribute"/> and <see cref="InheritDataTypeFromItemsAttribute"/>
    /// attributes to indicate that it is used as the template content and to specify the type of the template content.
    /// </remarks>
    [Content]
    [InheritDataTypeFromItems(nameof(TreeDataGridEx.ItemsSource), AncestorType = typeof(TreeDataGridEx))]
    public TreeDataGridColumn? Inner
    {
        get { return _inner; }
        set { SetAndRaise(InnerProperty, ref _inner, value); }
    }

    /// <summary>
    /// Gets or sets the name of the property that identifies the children of the hierarchical data item.
    /// </summary>
    /// <remarks>
    /// This property is marked with the <see cref="InheritDataTypeFromItemsAttribute"/> attribute to specify
    /// the type of the items to be bound to the column.
    /// </remarks>
    [InheritDataTypeFromItems(nameof(TreeDataGridEx.ItemsSource), AncestorType = typeof(TreeDataGridEx))]
    public string? ChildrenName
    {
        get => _childrenName;
        set => SetAndRaise(ChildrenNameProperty, ref _childrenName, value);
    }

    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(HierarchicalExpanderColumn<>))]
    internal override IColumn? Create(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] 
        Type modelType)
    {
        var inner = Inner;
        var childrenName = ChildrenName;

        if (childrenName is null)
        {
            return null;
        }

        var innerColumn = inner?.Create(modelType);

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
        return (IColumn?) Activator.CreateInstance(type, innerColumn, childSelector, null, null);
    }

    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(IEnumerable<>))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Func<,>))]
    private LambdaExpression CreateChildSelectorLambdaExpression(
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
