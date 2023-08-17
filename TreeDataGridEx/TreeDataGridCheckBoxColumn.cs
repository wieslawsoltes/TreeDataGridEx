using System;
using System.Linq.Expressions;
using Avalonia;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Data;
using Avalonia.Metadata;

namespace TreeDataGridEx;

public class TreeDataGridCheckBoxColumn<TModel> : TreeDataGridColumnBase
    where TModel : class
{
    public static readonly DirectProperty<TreeDataGridCheckBoxColumn<TModel>, IBinding?> BindingProperty =
        AvaloniaProperty.RegisterDirect<TreeDataGridCheckBoxColumn<TModel>, IBinding?>(
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

    public Expression<Func<TModel, bool>>? Getter { get; set; }

    public Action<TModel, bool>? Setter { get; set; }

    public override IColumn? Create()
    {
        var options = new CheckBoxColumnOptions<TModel>
        {
            CanUserResizeColumn = CanUserResizeColumn,
            CanUserSortColumn = CanUserSortColumn,
            MinWidth = MinWidth,
            MaxWidth = MaxWidth,
            // TODO: CompareAscending = CompareAscending,
            // TODO: CompareDescending = CompareDescending,
            BeginEditGestures = BeginEditGestures,
            // TODO: IsTextSearchEnabled = IsTextSearchEnabled,
            // TODO: TextSearchValueSelector = TextSearchValueSelector,
        };

        var column = new CheckBoxColumn<TModel>(
            Header, 
            Getter, 
            Setter, 
            Width,
            options);

        return column;
    }
}
