using Avalonia;
using Avalonia.Data;
using Avalonia.Metadata;

namespace TreeDataGridEx;

public class TreeDataGridCheckBoxColumn : TreeDataGridColumnBase
{
    public static readonly DirectProperty<TreeDataGridCheckBoxColumn, IBinding?> BindingProperty =
        AvaloniaProperty.RegisterDirect<TreeDataGridCheckBoxColumn, IBinding?>(
            nameof(Binding),
            o => o.Binding,
            (o, v) => o.Binding = v);

    private IBinding? _binding;

    [Content]
    [InheritDataTypeFromItems(nameof(TreeDataGridEx.ItemsSource), AncestorType = typeof(TreeDataGridEx))]
    [AssignBinding]
    public IBinding? Binding
    {
        get { return _binding; }
        set { SetAndRaise(BindingProperty, ref _binding, value); }
    }
}
