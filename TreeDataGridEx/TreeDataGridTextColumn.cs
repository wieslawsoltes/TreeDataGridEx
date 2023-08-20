using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Data;
using Avalonia.Markup.Xaml.MarkupExtensions;
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
