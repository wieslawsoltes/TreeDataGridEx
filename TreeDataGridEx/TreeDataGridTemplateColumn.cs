using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;

namespace TreeDataGridEx;

public class TreeDataGridTemplateColumn : TreeDataGridColumnBase
{
    // TODO: TemplateColumnOptions<>.IsTextSearchEnabled

    // TODO: TemplateColumnOptions<>.TextSearchValueSelector

    public static readonly DirectProperty<TreeDataGridTemplateColumn, IDataTemplate?> CellTemplateProperty =
        AvaloniaProperty.RegisterDirect<TreeDataGridTemplateColumn, IDataTemplate?>(
            nameof(CellTemplate),
            o => o.CellTemplate,
            (o, v) => o.CellTemplate = v);

    public static readonly DirectProperty<TreeDataGridTemplateColumn, IDataTemplate?> CellEditingTemplateProperty =
        AvaloniaProperty.RegisterDirect<TreeDataGridTemplateColumn, IDataTemplate?>(
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

    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(TemplateColumn<>))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ColumnOptions<>))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(TemplateColumnOptions<>))]
    internal override IColumn? Create(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        Type modelType)
    {
        var header = Header;
        var cellTemplate = CellTemplate;
        var cellEditingTemplate = CellEditingTemplate;
        var width = Width;

        if (cellTemplate is null)
        {
            return null;
        }

        return ColumnReflectionFactory.CreateTemplateColumn(
            modelType,
            cellTemplate,
            cellEditingTemplate,
            header,
            width,
            CanUserResizeColumn,
            CanUserSortColumn,
            MinWidth,
            MaxWidth,
            BeginEditGestures);
    }
}
