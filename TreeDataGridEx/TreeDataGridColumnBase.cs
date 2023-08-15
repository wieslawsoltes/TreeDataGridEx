using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;

namespace TreeDataGridEx;

public abstract class TreeDataGridColumnBase : TreeDataGridColumn
{
    public static readonly StyledProperty<bool?> CanUserResizeColumnProperty = 
        AvaloniaProperty.Register<TreeDataGridColumnBase, bool?>(nameof(CanUserResizeColumn));

    public static readonly StyledProperty<bool?> CanUserSortColumnProperty = 
        AvaloniaProperty.Register<TreeDataGridColumnBase, bool?>(nameof(CanUserSortColumn));

    public static readonly StyledProperty<GridLength> MinWidthProperty = 
        AvaloniaProperty.Register<TreeDataGridColumnBase, GridLength>(nameof(MinWidth), new GridLength(30, GridUnitType.Pixel));

    public static readonly StyledProperty<GridLength?> MaxWidthProperty = 
        AvaloniaProperty.Register<TreeDataGridColumnBase, GridLength?>(nameof(MaxWidth));

    public static readonly StyledProperty<BeginEditGestures> BeginEditGesturesProperty = 
        AvaloniaProperty.Register<TreeDataGridColumnBase, BeginEditGestures>(nameof(BeginEditGestures), BeginEditGestures.Default);

    public static readonly DirectProperty<TreeDataGridColumnBase, object?> HeaderProperty =
        AvaloniaProperty.RegisterDirect<TreeDataGridColumnBase, object?>(
            nameof(Header),
            o => o.Header,
            (o, v) => o.Header = v);

    public static readonly StyledProperty<GridLength> WidthProperty = 
        AvaloniaProperty.Register<TreeDataGridColumnBase, GridLength>(nameof(Width), GridLength.Auto);

    private object? _header;

    public bool? CanUserResizeColumn
    {
        get => GetValue(CanUserResizeColumnProperty);
        set => SetValue(CanUserResizeColumnProperty, value);
    }

    public bool? CanUserSortColumn
    {
        get => GetValue(CanUserSortColumnProperty);
        set => SetValue(CanUserSortColumnProperty, value);
    }

    public GridLength MinWidth
    {
        get => GetValue(MinWidthProperty);
        set => SetValue(MinWidthProperty, value);
    }

    public GridLength? MaxWidth
    {
        get => GetValue(MaxWidthProperty);
        set => SetValue(MaxWidthProperty, value);
    }

    public BeginEditGestures BeginEditGestures
    {
        get => GetValue(BeginEditGesturesProperty);
        set => SetValue(BeginEditGesturesProperty, value);
    }

    public object? Header
    {
        get => _header;
        set => SetAndRaise(HeaderProperty, ref _header, value);
    }

    public GridLength Width
    {
        get => GetValue(WidthProperty);
        set => SetValue(WidthProperty, value);
    }
}
