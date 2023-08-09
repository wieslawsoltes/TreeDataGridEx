using Avalonia;
using Avalonia.Controls;

namespace TreeDataGridEx.Controls;

public abstract class TreeDataGridColumn : AvaloniaObject
{
    public static readonly DirectProperty<TreeDataGridColumn, object?> HeaderProperty =
        AvaloniaProperty.RegisterDirect<TreeDataGridColumn, object?>(
            nameof(Header),
            o => o.Header,
            (o, v) => o.Header = v);

    public static readonly StyledProperty<GridLength> WidthProperty = 
        AvaloniaProperty.Register<TreeDataGridColumn, GridLength>(nameof(Width));

    private object? _header;

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
