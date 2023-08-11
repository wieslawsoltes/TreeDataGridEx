using Avalonia;
using Avalonia.Metadata;

namespace TreeDataGridEx;

public class TreeDataGridHierarchicalExpanderColumn : TreeDataGridColumn
{
    public static readonly DirectProperty<TreeDataGridHierarchicalExpanderColumn, TreeDataGridColumn?> InnerProperty =
        AvaloniaProperty.RegisterDirect<TreeDataGridHierarchicalExpanderColumn, TreeDataGridColumn?>(
            nameof(Inner),
            o => o.Inner,
            (o, v) => o.Inner = v);

    public static readonly DirectProperty<TreeDataGridHierarchicalExpanderColumn, string?> ChildrenNameProperty =
        AvaloniaProperty.RegisterDirect<TreeDataGridHierarchicalExpanderColumn, string?>(
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
}
