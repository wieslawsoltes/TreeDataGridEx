using Avalonia;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Metadata;

namespace TreeDataGridEx;

public class TreeDataGridTextColumn : TreeDataGridColumnBase
{
    public static readonly StyledProperty<bool> IsTextSearchEnabledProperty = 
        AvaloniaProperty.Register<TreeDataGridTextColumn, bool>(nameof(IsTextSearchEnabled));

    public static readonly StyledProperty<TextTrimming> TextTrimmingProperty = 
        AvaloniaProperty.Register<TreeDataGridTextColumn, TextTrimming>(nameof(TextTrimming), TextTrimming.CharacterEllipsis);

    public static readonly StyledProperty<TextWrapping> TextWrappingProperty = 
        AvaloniaProperty.Register<TreeDataGridTextColumn, TextWrapping>(nameof(TextWrapping), TextWrapping.NoWrap);

    public static readonly DirectProperty<TreeDataGridTextColumn, IBinding?> BindingProperty =
        AvaloniaProperty.RegisterDirect<TreeDataGridTextColumn, IBinding?>(
            nameof(Binding),
            o => o.Binding,
            (o, v) => o.Binding = v);

    private IBinding? _binding;

    public bool IsTextSearchEnabled
    {
        get => GetValue(IsTextSearchEnabledProperty);
        set => SetValue(IsTextSearchEnabledProperty, value);
    }

    public TextTrimming TextTrimming
    {
        get => GetValue(TextTrimmingProperty);
        set => SetValue(TextTrimmingProperty, value);
    }

    public TextWrapping TextWrapping
    {
        get => GetValue(TextWrappingProperty);
        set => SetValue(TextWrappingProperty, value);
    }

    [Content]
    [InheritDataTypeFromItems(nameof(TreeDataGridEx.ItemsSource), AncestorType = typeof(TreeDataGridEx))]
    [AssignBinding]
    public IBinding? Binding
    {
        get { return _binding; }
        set { SetAndRaise(BindingProperty, ref _binding, value); }
    }
}
