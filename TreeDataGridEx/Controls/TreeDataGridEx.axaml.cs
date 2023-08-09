using System;
using System.Collections;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;

namespace TreeDataGridEx.Controls;

public class TreeDataGridEx : TemplatedControl
{
    public static readonly StyledProperty<IEnumerable> ItemsSourceProperty =
        AvaloniaProperty.Register<TreeDataGridEx, IEnumerable>(nameof(ItemsSource));

    public static readonly StyledProperty<ObservableCollection<TreeDataGridColumn>?> ColumnsProperty = 
        AvaloniaProperty.Register<TreeDataGridEx, ObservableCollection<TreeDataGridColumn>?>(nameof(Columns));

    private ITreeDataGridSource? _source;
    private TreeDataGrid? _treeDataGrid;

    public IEnumerable ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public ObservableCollection<TreeDataGridColumn>? Columns
    {
        get => GetValue(ColumnsProperty);
        set => SetValue(ColumnsProperty, value);
    }

    public TreeDataGridEx()
    {
        SetCurrentValue(ColumnsProperty, new ObservableCollection<TreeDataGridColumn>());
    }
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _treeDataGrid = e.NameScope.Find<TreeDataGrid>("PART_TreeDataGrid");

        if (_treeDataGrid is not null)
        {
            _source = CreateSource();

            var add = _source.Columns.GetType().GetMethod("Add");

            foreach (var column in Columns)
            {
                if (column is TreeDataGridTemplateColumn templateColumn)
                {
                    var c = CreateColumn(
                        templateColumn.Header,
                        templateColumn.CellTemplate,
                        templateColumn.CellEditingTemplate);

                    add.Invoke(_source.Columns, new object[] { c });
                }
            }

            _treeDataGrid.Source = _source;

/*
            _source = new FlatTreeDataGridSource<T>(ItemsSource);

            foreach (var column in Columns)
            {
                if (column is TreeDataGridTemplateColumn templateColumn)
                {
                    (_source.Columns as ColumnList<T>).Add(
                        new TemplateColumn<T>(
                            templateColumn.Header,
                            templateColumn.CellTemplate,
                            templateColumn.CellEditingTemplate));
                }
            }
//*/
        }
    }

    private ITreeDataGridSource? CreateSource()
    {
        var flatTreeDataGridSourceType = typeof(FlatTreeDataGridSource<>);
        var modelType = ItemsSource.GetType().GenericTypeArguments[0];
        var type = flatTreeDataGridSourceType.MakeGenericType(modelType);
        return (ITreeDataGridSource?)Activator.CreateInstance(type, new object[] { ItemsSource });
    }

    private IColumn? CreateColumn(
        object? header,
        IDataTemplate cellTemplate,
        IDataTemplate? cellEditingTemplate = null,
        GridLength? width = null)
    {
        var templateColumnType = typeof(TemplateColumn<>);
        var modelType = ItemsSource.GetType().GenericTypeArguments[0];
        var type = templateColumnType.MakeGenericType(modelType);
        return (IColumn?) Activator.CreateInstance(type, new object[] { header, cellTemplate, cellEditingTemplate, width, null });
    }
}
