using System;
using Avalonia;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;

namespace TreeDataGridEx;

public class TreeDataGridTemplateColumn<TModel> : TreeDataGridColumnBase
    where TModel : class
{
    // TODO: TemplateColumnOptions<>.IsTextSearchEnabled

    // TODO: TemplateColumnOptions<>.TextSearchValueSelector

    public static readonly DirectProperty<TreeDataGridTemplateColumn<TModel>, IDataTemplate?> CellTemplateProperty =
        AvaloniaProperty.RegisterDirect<TreeDataGridTemplateColumn<TModel>, IDataTemplate?>(
            nameof(CellTemplate),
            o => o.CellTemplate,
            (o, v) => o.CellTemplate = v);

    public static readonly DirectProperty<TreeDataGridTemplateColumn<TModel>, IDataTemplate?> CellEditingTemplateProperty =
        AvaloniaProperty.RegisterDirect<TreeDataGridTemplateColumn<TModel>, IDataTemplate?>(
            nameof(CellEditingTemplate),
            o => o.CellEditingTemplate,
            (o, v) => o.CellEditingTemplate = v);

    private IDataTemplate? _cellTemplate;
    private IDataTemplate? _cellEditingCellTemplate;

    [Content]
    [InheritDataTypeFromItems(nameof(TreeDataGridEx.ItemsSource), AncestorType = typeof(TreeDataGridEx))]
    public IDataTemplate? CellTemplate
    {
        get { return _cellTemplate; }
        set { SetAndRaise(CellTemplateProperty, ref _cellTemplate, value); }
    }

    [InheritDataTypeFromItems(nameof(TreeDataGridEx.ItemsSource), AncestorType = typeof(TreeDataGridEx))]
    public IDataTemplate? CellEditingTemplate
    {
        get => _cellEditingCellTemplate;
        set => SetAndRaise(CellEditingTemplateProperty, ref _cellEditingCellTemplate, value);
    }

    public override IColumn? Create()
    {
       var options = new TemplateColumnOptions<TModel>()
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
 
        var column = new TemplateColumn<TModel>(
            Header, 
            CellTemplate, 
            CellEditingTemplate, 
            Width,
            options);

        return column;
    }
}
