using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;

namespace TreeDataGridEx.Controls;

public class TreeDataGridTemplateColumn : TreeDataGridColumnBase
{
    public static readonly DirectProperty<DataGridTemplateColumn, IDataTemplate?> CellTemplateProperty =
        AvaloniaProperty.RegisterDirect<DataGridTemplateColumn, IDataTemplate?>(
            nameof(CellTemplate),
            o => o.CellTemplate,
            (o, v) => o.CellTemplate = v);

    public static readonly DirectProperty<DataGridTemplateColumn, IDataTemplate?> CellEditingTemplateProperty =
        AvaloniaProperty.RegisterDirect<DataGridTemplateColumn, IDataTemplate?>(
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
}
