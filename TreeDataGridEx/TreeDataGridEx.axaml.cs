using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Primitives;

namespace TreeDataGridEx;

/// <summary>
/// Represents a control that displays hierarchical data in a grid format.
/// </summary>
public class TreeDataGridEx : TemplatedControl
{
    /// <summary>
    /// Identifies the <see cref="AutoDragDropRows"/> dependency property.
    /// </summary>
    public static readonly StyledProperty<bool> AutoDragDropRowsProperty =
        TreeDataGrid.AutoDragDropRowsProperty.AddOwner<TreeDataGridEx>();

    /// <summary>
    /// Identifies the <see cref="CanUserResizeColumns"/> dependency property.
    /// </summary>
    public static readonly StyledProperty<bool> CanUserResizeColumnsProperty =
        TreeDataGrid.CanUserResizeColumnsProperty.AddOwner<TreeDataGridEx>();

    /// <summary>
    /// Identifies the <see cref="CanUserSortColumns"/> dependency property.
    /// </summary>
    public static readonly StyledProperty<bool> CanUserSortColumnsProperty =
        TreeDataGrid.CanUserSortColumnsProperty.AddOwner<TreeDataGridEx>();

    /// <summary>
    /// Identifies the <see cref="Columns"/> dependency property.
    /// </summary>
    public static readonly StyledProperty<ObservableCollection<TreeDataGridColumn>?> ColumnsProperty = 
        AvaloniaProperty.Register<TreeDataGridEx, ObservableCollection<TreeDataGridColumn>?>(nameof(Columns));

    /// <summary>
    /// Identifies the <see cref="ShowColumnHeaders"/> dependency property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowColumnHeadersProperty =
        TreeDataGrid.ShowColumnHeadersProperty.AddOwner<TreeDataGridEx>();

    /// <summary>
    /// Identifies the <see cref="ItemsSource"/> dependency property.
    /// </summary>
    public static readonly StyledProperty<IEnumerable> ItemsSourceProperty =
        AvaloniaProperty.Register<TreeDataGridEx, IEnumerable>(nameof(ItemsSource));

    private ITreeDataGridSource? _source;
    private TreeDataGrid? _treeDataGrid;

    /// <summary>
    /// Gets or sets a value that indicates whether rows can be dragged and dropped.
    /// </summary>
    public bool AutoDragDropRows
    {
        get => GetValue(AutoDragDropRowsProperty);
        set => SetValue(AutoDragDropRowsProperty, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates whether columns can be resized.
    /// </summary>
    public bool CanUserResizeColumns
    {
        get => GetValue(CanUserResizeColumnsProperty);
        set => SetValue(CanUserResizeColumnsProperty, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates whether columns can be sorted.
    /// </summary>
    public bool CanUserSortColumns
    {
        get => GetValue(CanUserSortColumnsProperty);
        set => SetValue(CanUserSortColumnsProperty, value);
    }

    /// <summary>
    /// Gets or sets the columns displayed in the grid.
    /// </summary>
    public ObservableCollection<TreeDataGridColumn>? Columns
    {
        get => GetValue(ColumnsProperty);
        set => SetValue(ColumnsProperty, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates whether column headers are visible.
    /// </summary>
    public bool ShowColumnHeaders
    {
        get => GetValue(ShowColumnHeadersProperty);
        set => SetValue(ShowColumnHeadersProperty, value);
    }

    /// <summary>
    /// Gets or sets the data source for the grid.
    /// </summary>
    public IEnumerable ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    /// <summary>
    /// Gets the data source used for hierarchical data in the grid.
    /// </summary>
    public ITreeDataGridSource? Source => _source;

    /// <summary>
    /// Gets the underlying TreeDataGrid control.
    /// </summary>
    public TreeDataGrid? TreeDataGrid => _treeDataGrid;

    /// <summary>
    /// Initializes a new instance of the <see cref="TreeDataGridEx"/> class.
    /// </summary>
    public TreeDataGridEx()
    {
        SetCurrentValue(ColumnsProperty, new ObservableCollection<TreeDataGridColumn>());
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _treeDataGrid = e.NameScope.Find<TreeDataGrid>("PART_TreeDataGrid");

        Initialize();
    }

    private void Initialize()
    {
        (_source as IDisposable)?.Dispose();

        var itemsSource = ItemsSource;
        var columns = Columns;

        if (_treeDataGrid is null || columns is null)
        {
            return;
        }

        var source = CreateSource(itemsSource, columns);
        if (source is not null)
        {
            _source = source;
            _treeDataGrid.Source = _source;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ItemsSourceProperty || change.Property == ColumnsProperty)
        {
            Initialize();
        }
    }

    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ColumnList<>))]
    private static ITreeDataGridSource? CreateSource(IEnumerable items, ObservableCollection<TreeDataGridColumn> columns)
    {
        var modelType = items.GetType().GenericTypeArguments[0];

        var source = columns.Any(x => x is TreeDataGridHierarchicalExpanderColumn)
            ? CreateHierarchicalSource(modelType, items)
            : CreateFlatSource(modelType, items);
        if (source is null)
        {
            return null;
        }

        var columnsType = typeof(ColumnList<>).MakeGenericType(modelType);
        var add = columnsType.GetMethod("Add");
        if (add is null)
        {
            return null;
        }

        foreach (var column in columns)
        {
            try
            {
                var c = column.Create(column.DataType ?? modelType);
                if (c is not null)
                {
                    add.Invoke(source.Columns, new object[] { c });
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        return source;
    }

    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(FlatTreeDataGridSource<>))]
    private static ITreeDataGridSource? CreateFlatSource(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] 
        Type modelType,
        IEnumerable items)
    {
        var type = typeof(FlatTreeDataGridSource<>).MakeGenericType(modelType);

        return (ITreeDataGridSource?)Activator.CreateInstance(type, items);
    }

    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(HierarchicalExpanderColumn<>))]
    private static ITreeDataGridSource? CreateHierarchicalSource(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] 
        Type modelType,
        IEnumerable items)
    {
        var type = typeof(HierarchicalTreeDataGridSource<>).MakeGenericType(modelType);

        return (ITreeDataGridSource?)Activator.CreateInstance(type, items);
    }
}
