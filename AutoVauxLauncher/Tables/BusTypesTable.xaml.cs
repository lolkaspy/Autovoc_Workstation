using ARMDatabase;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
namespace AutoVauxLauncher
{
    /// <summary>
    /// Логика взаимодействия для TestPage.xaml
    /// </summary>
    public partial class BusTypesTable : UserControl
    {
        AutovauxContext cs;
        public BusTypesTable()
        {
            InitializeComponent();
            using (cs = new AutovauxContext())
            {
                cs.Bus_types.Load();
                bustypes.ItemsSource = cs.Bus_types.Local.ToBindingList();
            }
        }
        private void AddRow(object sender, RoutedEventArgs e)
        {
            using (cs = new AutovauxContext())
            {
                Bus_types bt;
                if (hascard.IsChecked == true)
                {
                    bt = new Bus_types() { BUS_TYPENAME = bustype.Text, BUS_HASCARD = true };
                    cs.Bus_types.Add(bt);
                }
                else
                {
                    bt = new Bus_types() { BUS_TYPENAME = bustype.Text, BUS_HASCARD = false };
                    cs.Bus_types.Add(bt);
                }
                cs.SaveChanges();
                cs.Bus_types.Load();
                bustypes.ItemsSource = cs.Bus_types.Local.ToBindingList();
                bustypes.Items.Refresh();
            }
        }
        private void DelRow(object sender, RoutedEventArgs e)
        {
            using (cs = new AutovauxContext())
            {
                MessageBoxUI mui = new MessageBoxUI("Вы действительно хотите удалить запись? Удалению подлежат так же и записи, зависмые от неё.", MessageType.Warning, MessageButtons.YesNo);
                mui.ShowDialog();
                if (mui.DialogResult == true)
                {
                    int selectedindex = (bustypes.SelectedItem as Bus_types).BUS_TYPE_ID;
                    var rowrol = cs.Bus_types.ToList().Where(x => x.BUS_TYPE_ID == selectedindex).First();
                    cs.Bus_types.Attach(rowrol);
                    cs.Bus_types.Remove(rowrol);
                    cs.SaveChanges();
                }
                this.bustype.Text = "";
                this.hascard.IsChecked = false;
                cs.Bus_types.Load();
                bustypes.ItemsSource = cs.Bus_types.Local.ToBindingList();
            }
        }
        private void UpdRow(object sender, RoutedEventArgs e)
        {
            using (cs = new AutovauxContext())
            {
                int selectedindex = (bustypes.SelectedItem as Bus_types).BUS_TYPE_ID;
                var row = cs.Bus_types.ToList().Where(x => x.BUS_TYPE_ID == selectedindex).First();
                row.BUS_TYPENAME = bustype.Text;
                row.BUS_HASCARD = (bool)hascard.IsChecked;
                cs.SaveChanges();
                cs.Bus_types.Load();
                bustypes.ItemsSource = cs.Bus_types.Local.ToBindingList();
            }
        }
        private void RowSelected(object sender, SelectionChangedEventArgs e)
        {
            using (cs = new AutovauxContext())
            {
                if (bustypes.SelectedItem != null)
                {
                    int selectedindex = (bustypes.SelectedItem as Bus_types).BUS_TYPE_ID;
                    var row = cs.Bus_types.ToList().Where(x => x.BUS_TYPE_ID == selectedindex).ToList();
                    string bust = row.Select(x => x.BUS_TYPENAME).First();
                    bool hc = row.Select(x => x.BUS_HASCARD).First();
                    this.bustype.Text = bust;
                    this.hascard.IsChecked = hc;
                }
                cs.Bus_types.Load();
            }
        }
    }
}
