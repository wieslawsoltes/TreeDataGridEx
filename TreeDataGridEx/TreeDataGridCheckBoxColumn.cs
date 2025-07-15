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

        return ColumnReflectionFactory.CreateCheckBoxColumn(
            modelType,
            property,
            header,
            width,
            CanUserResizeColumn,
            CanUserSortColumn,
            MinWidth,
            MaxWidth,
            BeginEditGestures);
    }
}
