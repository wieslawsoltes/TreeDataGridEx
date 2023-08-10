using Avalonia;
using Avalonia.Controls;

namespace TreeDataGridEx.Controls;

public abstract class TreeDataGridColumnBase : TreeDataGridColumn
{
    public static readonly DirectProperty<TreeDataGridColumnBase, object?> HeaderProperty =
        AvaloniaProperty.RegisterDirect<TreeDataGridColumnBase, object?>(
            nameof(Header),
            o => o.Header,
            (o, v) => o.Header = v);

    public static readonly StyledProperty<GridLength> WidthProperty = 
        AvaloniaProperty.Register<TreeDataGridColumnBase, GridLength>(nameof(Width), GridLength.Auto);

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
