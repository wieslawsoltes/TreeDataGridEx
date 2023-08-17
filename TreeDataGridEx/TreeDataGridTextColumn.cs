using System;
using System.Linq.Expressions;
using Avalonia;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Metadata;

namespace TreeDataGridEx;

public class TreeDataGridTextColumn<TModel, TValue> : TreeDataGridColumnBase
    where TModel : class
{
    public static readonly StyledProperty<bool> IsTextSearchEnabledProperty = 
        AvaloniaProperty.Register<TreeDataGridTextColumn<TModel, TValue>, bool>(nameof(IsTextSearchEnabled));

    public static readonly StyledProperty<TextTrimming> TextTrimmingProperty = 
        AvaloniaProperty.Register<TreeDataGridTextColumn<TModel, TValue>, TextTrimming>(nameof(TextTrimming), TextTrimming.CharacterEllipsis);

    public static readonly StyledProperty<TextWrapping> TextWrappingProperty = 
        AvaloniaProperty.Register<TreeDataGridTextColumn<TModel, TValue>, TextWrapping>(nameof(TextWrapping), TextWrapping.NoWrap);

    public static readonly DirectProperty<TreeDataGridTextColumn<TModel, TValue>, IBinding?> BindingProperty =
        AvaloniaProperty.RegisterDirect<TreeDataGridTextColumn<TModel, TValue>, IBinding?>(
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

    public Expression<Func<TModel, TValue?>> Getter { get; set; }

    public Action<TModel, TValue?> Setter { get; set; }

    public override IColumn? Create()
    {
        var options = new TextColumnOptions<TModel>
        {
            CanUserResizeColumn = CanUserResizeColumn,
            CanUserSortColumn = CanUserSortColumn,
            MinWidth = MinWidth,
            MaxWidth = MaxWidth,
            // TODO: CompareAscending = CompareAscending,
            // TODO: CompareDescending = CompareDescending,
            BeginEditGestures = BeginEditGestures,
            IsTextSearchEnabled = IsTextSearchEnabled,
            TextTrimming = TextTrimming,
            TextWrapping = TextWrapping,
        };

        var column = new TextColumn<TModel, TValue>(
            Header, 
            Getter,
            Setter, 
            Width, null);

        return column;
    }
}
