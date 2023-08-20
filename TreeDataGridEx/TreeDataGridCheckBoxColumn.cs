using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Data;
using Avalonia.Markup.Xaml.MarkupExtensions;
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

    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(CheckBoxColumn<>))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ColumnOptions<>))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(CheckBoxColumnOptions<>))]
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

        var getter = CreateGetterLambdaExpression(modelType, property);
        var type = typeof(CheckBoxColumn<>).MakeGenericType(modelType);

        var optionsType = typeof(CheckBoxColumnOptions<>).MakeGenericType(modelType);
        var options = Activator.CreateInstance(optionsType);

        // ColumnOptions
        optionsType.GetProperty("CanUserResizeColumn")?.SetValue(options, CanUserResizeColumn);
        optionsType.GetProperty("CanUserSortColumn")?.SetValue(options, CanUserSortColumn);
        optionsType.GetProperty("MinWidth")?.SetValue(options, MinWidth);
        optionsType.GetProperty("MaxWidth")?.SetValue(options, MaxWidth);
        // TODO: CompareAscending
        // TODO: CompareDescending
        optionsType.GetProperty("BeginEditGestures")?.SetValue(options, BeginEditGestures);

        // CheckBoxColumnOptions (none)

        if (!property.CanWrite || (property.SetMethod is not null && !property.SetMethod.IsPublic))
        {
            return (IColumn?) Activator.CreateInstance(type, header, getter, width, options);
        }

        var setter = CreateSetterLambdaExpression(modelType, property).Compile();

        return (IColumn?) Activator.CreateInstance(type, header, getter, setter, width, options);
    }
}
