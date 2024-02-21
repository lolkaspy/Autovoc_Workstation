using System.Collections.ObjectModel;
using System.ComponentModel;

namespace AutoVauxLauncher.HelpClasses
{
    public class ObservableTimetable : ObservableCollection<Custom>
    {
        public ObservableTimetable() : base() { }

        public void AddItem(Custom item)
        {
            this.Add(item);
            NotifyPropertyChanged("Items");
        }
        public void RemoveItem(Custom item)
        {
            this.Remove(item);
            NotifyPropertyChanged("Items");
        }

        public void UpdateItem(Custom item)
        {
            // Ищем элемент в коллекции и заменяем его на новый
            int index = this.IndexOf(item);
            if (index != -1)
            {
                this[index] = item;
                NotifyPropertyChanged("Items");
            }
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public new event PropertyChangedEventHandler PropertyChanged;
    }
}
