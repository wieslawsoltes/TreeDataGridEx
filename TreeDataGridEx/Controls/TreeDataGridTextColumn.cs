using Avalonia;
using Avalonia.Controls;
using Avalonia.Metadata;

namespace TreeDataGridEx.Controls;

public class TreeDataGridTextColumn : TreeDataGridColumn
{
    public static readonly DirectProperty<TreeDataGridTextColumn, string?> NameProperty =
        AvaloniaProperty.RegisterDirect<TreeDataGridTextColumn, string?>(
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
