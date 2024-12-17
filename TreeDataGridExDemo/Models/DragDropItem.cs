using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ReactiveUI;

namespace TreeDataGridExDemo.Models
{
    public partial class DragDropItem(string name) : ReactiveObject
    {
        private static readonly Random s_random = new Random(0);

        public string Name { get; } = name;

        [Reactive]
        public partial bool AllowDrag { get; set; }
        
        [Reactive]
        public partial bool AllowDrop { get; set; }

        [field: AllowNull, MaybeNull]
        public ObservableCollection<DragDropItem> Children => field ??= CreateRandomItems();

        public static ObservableCollection<DragDropItem> CreateRandomItems()
        {
            var names = new Bogus.DataSets.Name();
            var count = s_random.Next(10);
            return new ObservableCollection<DragDropItem>(Enumerable.Range(0, count)
                .Select(_ => new DragDropItem(names.FullName())));
        }
    }
}
