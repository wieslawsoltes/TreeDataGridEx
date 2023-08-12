using System.Collections.ObjectModel;
using TreeDataGridExDemo.Models;

namespace TreeDataGridExDemo.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly ObservableCollection<Country> _countries;
    private ObservableCollection<DragDropItem> _dragAndDropItems;

    public MainWindowViewModel()
    {
        _countries = new ObservableCollection<Country>(Models.Countries.All);
        _dragAndDropItems = DragDropItem.CreateRandomItems();
    }

    public ObservableCollection<Country> Countries => _countries;

    public ObservableCollection<DragDropItem> DragAndDropItems => _dragAndDropItems;
}
