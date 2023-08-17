using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;

namespace TreeDataGridEx;

/// <summary>
/// Represents an abstract base class for a tree data grid column with additional features.
/// </summary>
public abstract class TreeDataGridColumnBase : TreeDataGridColumn
{
    /// <summary>
    /// Identifies the <see cref="CanUserResizeColumn"/> dependency property.
    /// </summary>
    public static readonly StyledProperty<bool?> CanUserResizeColumnProperty = 
        AvaloniaProperty.Register<TreeDataGridColumnBase, bool?>(nameof(CanUserResizeColumn));

    /// <summary>
    /// Identifies the <see cref="CanUserSortColumn"/> dependency property.
    /// </summary>
    public static readonly StyledProperty<bool?> CanUserSortColumnProperty = 
        AvaloniaProperty.Register<TreeDataGridColumnBase, bool?>(nameof(CanUserSortColumn));

    /// <summary>
    /// Identifies the <see cref="MinWidth"/> dependency property.
    /// </summary>
    public static readonly StyledProperty<GridLength> MinWidthProperty = 
        AvaloniaProperty.Register<TreeDataGridColumnBase, GridLength>(nameof(MinWidth), new GridLength(30, GridUnitType.Pixel));

    /// <summary>
    /// Identifies the <see cref="MaxWidth"/> dependency property.
    /// </summary>
    public static readonly StyledProperty<GridLength?> MaxWidthProperty = 
        AvaloniaProperty.Register<TreeDataGridColumnBase, GridLength?>(nameof(MaxWidth));

    /// <summary>
    /// Identifies the <see cref="BeginEditGestures"/> dependency property.
    /// </summary>
    public static readonly StyledProperty<BeginEditGestures> BeginEditGesturesProperty = 
        AvaloniaProperty.Register<TreeDataGridColumnBase, BeginEditGestures>(nameof(BeginEditGestures), BeginEditGestures.Default);

    /// <summary>
    /// Identifies the <see cref="Header"/> dependency property.
    /// </summary>
    public static readonly DirectProperty<TreeDataGridColumnBase, object?> HeaderProperty =
        AvaloniaProperty.RegisterDirect<TreeDataGridColumnBase, object?>(
            nameof(Header),
            o => o.Header,
            (o, v) => o.Header = v);

    /// <summary>
    /// Identifies the <see cref="Width"/> dependency property.
    /// </summary>
    public static readonly StyledProperty<GridLength> WidthProperty = 
        AvaloniaProperty.Register<TreeDataGridColumnBase, GridLength>(nameof(Width), GridLength.Auto);

    private object? _header;

    /// <summary>
    /// Gets or sets a value indicating whether the user can resize the column.
    /// </summary>
    public bool? CanUserResizeColumn
    {
        get => GetValue(CanUserResizeColumnProperty);
        set => SetValue(CanUserResizeColumnProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the user can sort the column.
    /// </summary>
    public bool? CanUserSortColumn
    {
        get => GetValue(CanUserSortColumnProperty);
        set => SetValue(CanUserSortColumnProperty, value);
    }

    /// <summary>
    /// Gets or sets the minimum width of the column.
    /// </summary>
    public GridLength MinWidth
    {
        get => GetValue(MinWidthProperty);
        set => SetValue(MinWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum width of the column.
    /// </summary>
    public GridLength? MaxWidth
    {
        get => GetValue(MaxWidthProperty);
        set => SetValue(MaxWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the gestures that begin the cell editing process.
    /// </summary>
    public BeginEditGestures BeginEditGestures
    {
        get => GetValue(BeginEditGesturesProperty);
        set => SetValue(BeginEditGesturesProperty, value);
    }

    /// <summary>
    /// Gets or sets the header content of the column.
    /// </summary>
    public object? Header
    {
        get => _header;
        set => SetAndRaise(HeaderProperty, ref _header, value);
    }

    /// <summary>
    /// Gets or sets the width of the column.
    /// </summary>
    public GridLength Width
    {
        get => GetValue(WidthProperty);
        set => SetValue(WidthProperty, value);
    }
}
