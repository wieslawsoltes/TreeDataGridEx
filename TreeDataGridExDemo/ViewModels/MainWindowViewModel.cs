using System.Collections.ObjectModel;
using TreeDataGridExDemo.Models;

namespace TreeDataGridExDemo.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly ObservableCollection<Country> _countries = new(Models.Countries.All);
    private readonly ObservableCollection<DragDropItem> _dragAndDropItems = DragDropItem.CreateRandomItems();

    public ObservableCollection<Country> Countries => _countries;

    public ObservableCollection<DragDropItem> DragAndDropItems => _dragAndDropItems;
}
