using Avalonia;
using Avalonia.Data;
using Avalonia.Metadata;

namespace TreeDataGridEx;

/// <summary>
/// Represents a tree data grid column with check boxes.
/// </summary>
public class TreeDataGridCheckBoxColumn : TreeDataGridColumnBase
{
    /// <summary>
    /// Identifies the <see cref="Binding"/> dependency property.
    /// </summary>
    public static readonly DirectProperty<TreeDataGridCheckBoxColumn, IBinding?> BindingProperty =
        AvaloniaProperty.RegisterDirect<TreeDataGridCheckBoxColumn, IBinding?>(
            nameof(Binding),
            o => o.Binding,
            (o, v) => o.Binding = v);

    private IBinding? _binding;

    /// <summary>
    /// Gets or sets the binding associated with this column.
    /// </summary>
    /// <remarks>
    /// This property is marked with the <see cref="ContentAttribute"/>, <see cref="InheritDataTypeFromItemsAttribute"/> and <see cref="AssignBindingAttribute"/> attributes
    /// to indicate that it is used to specify the binding, inherits the data type from the parent control's ItemsSource property, and to assign the binding respectively.
    /// </remarks>
    [Content]
    [InheritDataTypeFromItems(nameof(TreeDataGridEx.ItemsSource), AncestorType = typeof(TreeDataGridEx))]
    [AssignBinding]
    public IBinding? Binding
    {
        get { return _binding; }
        set { SetAndRaise(BindingProperty, ref _binding, value); }
    }
}

