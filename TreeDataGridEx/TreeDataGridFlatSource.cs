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
using Avalonia.Metadata;

namespace TreeDataGridEx;

/// <summary>
/// Provides a XAML friendly wrapper around <see cref="FlatTreeDataGridSource{T}"/>.
/// The generic parameter can be provided from XAML using <c>x:TypeArguments</c>
/// which avoids the heavy reflection previously used to determine the model type.
/// </summary>
public class TreeDataGridFlatSource<T> : AvaloniaObject, ITreeDataGridSource, IDisposable
    where T : class
{
    public static readonly StyledProperty<IEnumerable<T>?> ItemsProperty =
        AvaloniaProperty.Register<TreeDataGridFlatSource<T>, IEnumerable<T>?>(nameof(Items));

    public static readonly StyledProperty<ObservableCollection<TreeDataGridColumn>?> ColumnsProperty =
        AvaloniaProperty.Register<TreeDataGridFlatSource<T>, ObservableCollection<TreeDataGridColumn>?>(nameof(Columns));

    private FlatTreeDataGridSource<T>? _source;

    public TreeDataGridFlatSource()
    {
        SetCurrentValue(ColumnsProperty, new ObservableCollection<TreeDataGridColumn>());
        this.GetPropertyChangedObservable(ItemsProperty).Subscribe(_ => Initialize());
        this.GetPropertyChangedObservable(ColumnsProperty).Subscribe(_ => Initialize());
    }

    public IEnumerable<T>? Items
    {
        get => GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    [Content]
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

        _source = new FlatTreeDataGridSource<T>(items);

        foreach (var column in columns)
        {
            try
            {
                var c = column.Create(column.DataType ?? typeof(T)) as IColumn<T>;
                if (c is not null)
                {
                    _source.Columns.Add(c);
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
