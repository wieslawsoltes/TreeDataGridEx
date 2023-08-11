using Avalonia;
using Avalonia.Metadata;

namespace TreeDataGridEx;

public class TreeDataGridCheckBoxColumn : TreeDataGridColumnBase
{
    public static readonly DirectProperty<TreeDataGridCheckBoxColumn, string?> NameProperty =
        AvaloniaProperty.RegisterDirect<TreeDataGridCheckBoxColumn, string?>(
            nameof(Name),
            o => o.Name,
            (o, v) => o.Name = v);

    private string? _name;

    [Content]
    [InheritDataTypeFromItems(nameof(TreeDataGridEx.ItemsSource), AncestorType = typeof(TreeDataGridEx))]
    public string? Name
    {
        get { return _name; }
        set { SetAndRaise(NameProperty, ref _name, value); }
    }
}
