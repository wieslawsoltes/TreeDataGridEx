using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Rendering;
using TreeDataGridExDemo.Models;

namespace TreeDataGridExDemo.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // RendererDiagnostics.DebugOverlays = RendererDebugOverlays.Fps |RendererDebugOverlays.LayoutTimeGraph | RendererDebugOverlays.RenderTimeGraph;
        
        TreeDataGridExDragDrop.Loaded += TreeDataGridExDragDropOnLoaded;
    }

    private void TreeDataGridExDragDropOnLoaded(object? sender, RoutedEventArgs e)
    {
        if (TreeDataGridExDragDrop.TreeDataGrid is { } treeDataGrid)
        {
            treeDataGrid.AutoDragDropRows = true;
            treeDataGrid.RowDragStarted += DragDrop_RowDragStarted;
            treeDataGrid.RowDragOver += DragDrop_RowDragOver;
        }
    }

    private void DragDrop_RowDragStarted(object? sender, TreeDataGridRowDragStartedEventArgs e)
    {
        foreach (DragDropItem i in e.Models)
        {
            if (!i.AllowDrag)
                e.AllowedEffects = DragDropEffects.None;
        }
    }

    private void DragDrop_RowDragOver(object? sender, TreeDataGridRowDragEventArgs e)
    {
        if (e.Position == TreeDataGridRowDropPosition.Inside &&
            e.TargetRow.Model is DragDropItem i &&
            !i.AllowDrop)
            e.Inner.DragEffects = DragDropEffects.None;
    }

}
