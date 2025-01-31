using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;

namespace TreeDataGridEx;

/// <summary>
/// Represents a column that can display data by using specified templates.
/// </summary>
public class TreeDataGridTemplateColumn : TreeDataGridColumnBase
{
    // TODO: TemplateColumnOptions<>.IsTextSearchEnabled
    // TODO: TemplateColumnOptions<>.TextSearchValueSelector

    /// <summary>
    /// Identifies the <see cref="CellTemplate"/> dependency property.
    /// </summary>
    public static readonly DirectProperty<TreeDataGridTemplateColumn, IDataTemplate?> CellTemplateProperty =
        AvaloniaProperty.RegisterDirect<TreeDataGridTemplateColumn, IDataTemplate?>(
            nameof(CellTemplate),
            o => o.CellTemplate,
            (o, v) => o.CellTemplate = v);

    /// <summary>
    /// Identifies the <see cref="CellEditingTemplate"/> dependency property.
    /// </summary>
    public static readonly DirectProperty<TreeDataGridTemplateColumn, IDataTemplate?> CellEditingTemplateProperty =
        AvaloniaProperty.RegisterDirect<TreeDataGridTemplateColumn, IDataTemplate?>(
            nameof(CellEditingTemplate),
            o => o.CellEditingTemplate,
            (o, v) => o.CellEditingTemplate = v);

    private IDataTemplate? _cellTemplate;
    private IDataTemplate? _cellEditingCellTemplate;

    /// <summary>
    /// Gets or sets the data template used to display the contents of a cell in the column.
    /// </summary>
    /// <remarks>
    /// This property is marked with the <see cref="ContentAttribute"/> and <see cref="InheritDataTypeFromItemsAttribute"/>
    /// attributes to indicate that it is used as the template content and to specify the type of the template content.
    /// </remarks>
    [Content]
    [InheritDataTypeFromItems(nameof(TreeDataGridEx.ItemsSource), AncestorType = typeof(TreeDataGridEx))]
    public IDataTemplate? CellTemplate
    {
        get { return _cellTemplate; }
        set { SetAndRaise(CellTemplateProperty, ref _cellTemplate, value); }
    }

    /// <summary>
    /// Gets or sets the data template used to display the contents of a cell in edit mode.
    /// </summary>
    /// <remarks>
    /// This property is marked with the <see cref="InheritDataTypeFromItemsAttribute"/> attribute to specify
    /// the type of the items to be bound to the column.
    /// </remarks>
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

        var type = typeof(TemplateColumn<>).MakeGenericType(modelType);

        var optionsType = typeof(TemplateColumnOptions<>).MakeGenericType(modelType);
        var options = Activator.CreateInstance(optionsType);

        // ColumnOptions
        optionsType.GetProperty("CanUserResizeColumn")?.SetValue(options, CanUserResizeColumn);
        optionsType.GetProperty("CanUserSortColumn")?.SetValue(options, CanUserSortColumn);
        optionsType.GetProperty("MinWidth")?.SetValue(options, MinWidth);
        optionsType.GetProperty("MaxWidth")?.SetValue(options, MaxWidth);
        // TODO: CompareAscending
        // TODO: CompareDescending
        optionsType.GetProperty("BeginEditGestures")?.SetValue(options, BeginEditGestures);

        // TemplateColumnOptions
        // - IsTextSearchEnabled
        // - TextSearchValueSelector

        return (IColumn?) Activator.CreateInstance(type, header, cellTemplate, cellEditingTemplate, width, options);
    }
}
