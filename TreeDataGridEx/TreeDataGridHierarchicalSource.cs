using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Input;
using Avalonia.Controls.Selection;

namespace TreeDataGridEx;

public class TreeDataGridHierarchicalSource : AvaloniaObject, ITreeDataGridSource, IDisposable
{
    public static readonly StyledProperty<IEnumerable> ItemsProperty =
        AvaloniaProperty.Register<TreeDataGridHierarchicalSource, IEnumerable>(nameof(Items));

    public static readonly StyledProperty<ObservableCollection<TreeDataGridColumn>?> ColumnsProperty =
        AvaloniaProperty.Register<TreeDataGridHierarchicalSource, ObservableCollection<TreeDataGridColumn>?>(nameof(Columns));

    private ITreeDataGridSource? _source;

    public TreeDataGridHierarchicalSource()
    {
        SetCurrentValue(ColumnsProperty, new ObservableCollection<TreeDataGridColumn>());
        this.GetPropertyChangedObservable(ItemsProperty).Subscribe(_ => Initialize());
        this.GetPropertyChangedObservable(ColumnsProperty).Subscribe(_ => Initialize());
    }

    public IEnumerable Items
    {
        get => GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    public ObservableCollection<TreeDataGridColumn>? Columns
    {
        get => GetValue(ColumnsProperty);
        set => SetValue(ColumnsProperty, value);
    }

    private void Initialize()
    {
        (_source as IDisposable)?.Dispose();

        var items = Items;
        var columns = Columns;
        if (items is null || columns is null)
        {
            _source = null;
            return;
        }

        var modelType = items.GetType().GenericTypeArguments.FirstOrDefault();
        if (modelType is null)
        {
            _source = null;
            return;
        }

        var type = typeof(HierarchicalTreeDataGridSource<>).MakeGenericType(modelType);
        _source = (ITreeDataGridSource?)Activator.CreateInstance(type, items);
        if (_source is null)
            return;

        var columnsProperty = type.GetProperty("Columns");
        var targetColumns = columnsProperty?.GetValue(_source);
        var addMethod = targetColumns?.GetType().GetMethod("Add");

        foreach (var column in columns)
        {
            try
            {
                var c = column.Create(column.DataType ?? modelType);
                if (c is not null)
                {
                    addMethod?.Invoke(targetColumns, new[] { c });
                }
            }
            catch
            {
                // ignored
            }
        }
    }

    private ITreeDataGridSource Source => _source ?? throw new InvalidOperationException("Source not initialized.");

    public IColumns ColumnsCollection => Source.Columns;
    IColumns ITreeDataGridSource.Columns => ColumnsCollection;
    public IRows Rows => Source.Rows;
    public ITreeDataGridSelection? Selection
    {
        get => Source.Selection;
        set => Source.Selection = value;
    }
    public bool IsHierarchical => Source.IsHierarchical;
    public bool IsSorted => Source.IsSorted;
    public event Action? Sorted
    {
        add => Source.Sorted += value;
        remove => Source.Sorted -= value;
    }
    public void DragDropRows(ITreeDataGridSource source, IEnumerable<IndexPath> indexes, IndexPath targetIndex, TreeDataGridRowDropPosition position, DragDropEffects effects)
        => Source.DragDropRows(source, indexes, targetIndex, position, effects);
    IEnumerable<object> ITreeDataGridSource.Items => Source.Items;
    public IEnumerable<object>? GetModelChildren(object model) => Source.GetModelChildren(model);
    public bool SortBy(IColumn column, ListSortDirection direction) => Source.SortBy(column, direction);

    public void Dispose()
    {
        (_source as IDisposable)?.Dispose();
    }
}
