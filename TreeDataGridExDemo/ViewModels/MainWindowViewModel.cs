using System.Collections.ObjectModel;
using TreeDataGridExDemo.Models;

namespace TreeDataGridExDemo.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly ObservableCollection<Country> _data;
    private ObservableCollection<DragDropItem> _dragAndDropData;

    public MainWindowViewModel()
    {
        _data = new ObservableCollection<Country>(Countries.All);
        _dragAndDropData = DragDropItem.CreateRandomItems();
    }

    public ObservableCollection<Country> Data => _data;

    public ObservableCollection<DragDropItem> DragAndDropData => _dragAndDropData;
}
