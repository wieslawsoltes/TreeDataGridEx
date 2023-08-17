using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Avalonia;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Metadata;

namespace TreeDataGridEx;

public class TreeDataGridHierarchicalExpanderColumn<TModel> : TreeDataGridColumn
    where TModel : class
{
    // TODO: HierarchicalExpanderColumn<>.hasChildrenSelector

    // TODO: HierarchicalExpanderColumn<>.isExpandedSelector

    public static readonly DirectProperty<TreeDataGridHierarchicalExpanderColumn<TModel>, TreeDataGridColumn?> InnerProperty =
        AvaloniaProperty.RegisterDirect<TreeDataGridHierarchicalExpanderColumn<TModel>, TreeDataGridColumn?>(
            nameof(Inner),
            o => o.Inner,
            (o, v) => o.Inner = v);

    public static readonly DirectProperty<TreeDataGridHierarchicalExpanderColumn<TModel>, string?> ChildrenNameProperty =
        AvaloniaProperty.RegisterDirect<TreeDataGridHierarchicalExpanderColumn<TModel>, string?>(
            nameof(ChildrenName),
            o => o.ChildrenName,
            (o, v) => o.ChildrenName = v);

    private TreeDataGridColumn? _inner;
    private string? _childrenName;

    [Content]
    [InheritDataTypeFromItems(nameof(TreeDataGridEx.ItemsSource), AncestorType = typeof(TreeDataGridEx))]
    public TreeDataGridColumn? Inner
    {
        get { return _inner; }
        set { SetAndRaise(InnerProperty, ref _inner, value); }
    }

    [InheritDataTypeFromItems(nameof(TreeDataGridEx.ItemsSource), AncestorType = typeof(TreeDataGridEx))]
    public string? ChildrenName
    {
        get => _childrenName;
        set => SetAndRaise(ChildrenNameProperty, ref _childrenName, value);
    }

    public Func<TModel, IEnumerable<TModel>?>? ChildSelector { get; set; }

    public Expression<Func<TModel, bool>>? HasChildrenSelector { get; set; }

    public Expression<Func<TModel, bool>>? IsExpandedSelector { get; set; }

    public override IColumn? Create()
    {
        var innerColumn = Inner?.Create() as IColumn<TModel>;

        var column = new HierarchicalExpanderColumn<TModel>(
            innerColumn, 
            ChildSelector,
            HasChildrenSelector,
            IsExpandedSelector);

        return column;
    }
}
