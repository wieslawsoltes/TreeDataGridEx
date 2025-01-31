using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Data;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using Avalonia.Metadata;

namespace TreeDataGridEx;

/// <summary>
/// Represents a column that can display text-based data.
/// </summary>
public class TreeDataGridTextColumn : TreeDataGridColumnBase
{
    /// <summary>
    /// Identifies the <see cref="IsTextSearchEnabled"/> dependency property.
    /// </summary>
    public static readonly StyledProperty<bool> IsTextSearchEnabledProperty = 
        AvaloniaProperty.Register<TreeDataGridTextColumn, bool>(nameof(IsTextSearchEnabled));

    /// <summary>
    /// Identifies the <see cref="TextTrimming"/> dependency property.
    /// </summary>
    public static readonly StyledProperty<TextTrimming> TextTrimmingProperty = 
        AvaloniaProperty.Register<TreeDataGridTextColumn, TextTrimming>(nameof(TextTrimming), TextTrimming.CharacterEllipsis);

    /// <summary>
    /// Identifies the <see cref="TextWrapping"/> dependency property.
    /// </summary>
    public static readonly StyledProperty<TextWrapping> TextWrappingProperty = 
        AvaloniaProperty.Register<TreeDataGridTextColumn, TextWrapping>(nameof(TextWrapping), TextWrapping.NoWrap);

    /// <summary>
    /// Identifies the <see cref="Binding"/> dependency property.
    /// </summary>
    public static readonly DirectProperty<TreeDataGridTextColumn, IBinding?> BindingProperty =
        AvaloniaProperty.RegisterDirect<TreeDataGridTextColumn, IBinding?>(
            nameof(Binding),
            o => o.Binding,
            (o, v) => o.Binding = v);

    private IBinding? _binding;

    /// <summary>
    /// Gets or sets a value that indicates whether text search is enabled for this column.
    /// </summary>
    public bool IsTextSearchEnabled
    {
        get => GetValue(IsTextSearchEnabledProperty);
        set => SetValue(IsTextSearchEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets a value that specifies how text in the column cells is trimmed when it overflows the cell boundaries.
    /// </summary>
    public TextTrimming TextTrimming
    {
        get => GetValue(TextTrimmingProperty);
        set => SetValue(TextTrimmingProperty, value);
    }

    /// <summary>
    /// Gets or sets a value that specifies how text wrapping is handled in the column cells.
    /// </summary>
    public TextWrapping TextWrapping
    {
        get => GetValue(TextWrappingProperty);
        set => SetValue(TextWrappingProperty, value);
    }

    /// <summary>
    /// Gets or sets the binding that associates the column with a property in the data source.
    /// </summary>
    /// <remarks>
    /// This property is marked with the <see cref="ContentAttribute"/> and <see cref="InheritDataTypeFromItemsAttribute"/>
    /// attributes to indicate that it is used as the content of the column and to specify the type of the template content.
    /// </remarks>
    [Content]
    [InheritDataTypeFromItems(nameof(TreeDataGridEx.ItemsSource), AncestorType = typeof(TreeDataGridEx))]
    [AssignBinding]
    public IBinding? Binding
    {
        get { return _binding; }
        set { SetAndRaise(BindingProperty, ref _binding, value); }
    }

    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(TextColumn<,>))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ColumnOptions<>))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(TextColumnOptions<>))]
    internal override IColumn? Create(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] 
        Type modelType)
    {
        var header = Header;
        var binding = Binding;
        var width = Width;

        if (binding is null)
        {
            return null;
        }

        var path = (binding as Binding)?.Path ?? (binding as CompiledBindingExtension)?.Path.ToString();
        if (path is null)
        {
            return null;
        }

        var property = modelType.GetProperty(path);
        if (property is null)
        {
            return null;
        }

        var propertyType = property.PropertyType;
        var getter = CreateGetterLambdaExpression(modelType, property);
        var type = typeof(TextColumn<,>).MakeGenericType(modelType, propertyType);

        var optionsType = typeof(TextColumnOptions<>).MakeGenericType(modelType);
        var options = Activator.CreateInstance(optionsType);

        // ColumnOptions
        optionsType.GetProperty("CanUserResizeColumn")?.SetValue(options, CanUserResizeColumn);
        optionsType.GetProperty("CanUserSortColumn")?.SetValue(options, CanUserSortColumn);
        optionsType.GetProperty("MinWidth")?.SetValue(options, MinWidth);
        optionsType.GetProperty("MaxWidth")?.SetValue(options, MaxWidth);
        // TODO: CompareAscending
        // TODO: CompareDescending
        optionsType.GetProperty("BeginEditGestures")?.SetValue(options, BeginEditGestures);

        // TextColumnOptions
        optionsType.GetProperty("IsTextSearchEnabled")?.SetValue(options, IsTextSearchEnabled);
        optionsType.GetProperty("TextTrimming")?.SetValue(options, TextTrimming);
        optionsType.GetProperty("TextWrapping")?.SetValue(options, TextWrapping);
    
        if (!property.CanWrite || (property.SetMethod is not null && !property.SetMethod.IsPublic))
        {
            return (IColumn?) Activator.CreateInstance(type, header, getter, width, options);
        }

        var setter = CreateSetterLambdaExpression(modelType, property).Compile();

        return (IColumn?) Activator.CreateInstance(type, header, getter, setter, width, options);
    }
}
