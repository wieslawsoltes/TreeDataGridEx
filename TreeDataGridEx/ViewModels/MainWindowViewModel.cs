using System.Collections.ObjectModel;
using TreeDataGridEx.Models;

namespace TreeDataGridEx.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly ObservableCollection<Country> _data;

    public MainWindowViewModel()
    {
        _data = new ObservableCollection<Country>(Countries.All);
    }
    
    public ObservableCollection<Country> Data => _data;
}
