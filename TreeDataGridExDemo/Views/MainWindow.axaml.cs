using Avalonia.Controls;
using Avalonia.Rendering;

namespace TreeDataGridEx.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // RendererDiagnostics.DebugOverlays = RendererDebugOverlays.Fps |RendererDebugOverlays.LayoutTimeGraph | RendererDebugOverlays.RenderTimeGraph;
    }
}