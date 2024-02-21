using ARMDatabase;
using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
namespace AutoVauxLauncher
{
    /// <summary>
    /// Логика взаимодействия для TestPage.xaml
    /// </summary>
    public partial class RoutesTable : UserControl
    {
        AutovauxContext cs;
        public RoutesTable()
        {
            InitializeComponent();
            using (cs = new AutovauxContext())
            {
                cs.Routes.Load();
                cs.Bus_fleet.Load();
                var buses = cs.Bus_fleet.Select(x => x.BUS_ID).ToList();
                busid.ItemsSource = buses;
                routes.ItemsSource = cs.Routes.Local.ToBindingList();
            }
        }
        private void AddRow(object sender, RoutedEventArgs e)
        {
            using (cs = new AutovauxContext())
            {
                if (busid.SelectedItem != null)
                {
                    try
                    {
                        decimal d = Decimal.Parse(costtxt.Text);
                        Routes rou = new Routes() { ROUTE = routetxt.Text, TRAVEL_COSTS = d, BUS_ID_FK = Int32.Parse(busid.SelectedItem.ToString()) };
                        cs.Routes.Add(rou);
                    }
                    catch
                    {
                        MessageBoxUI mui = new MessageBoxUI("Входная строка имела неверный формат", MessageType.Error, MessageButtons.Ok);
                    }
                    cs.SaveChanges();
                }
                cs.Routes.Load();
                routes.ItemsSource = cs.Routes.Local.ToBindingList();
                routes.Items.Refresh();
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
                    int selectedindex = (routes.SelectedItem as Routes).ROUTE_ID;
                    var rowrou = cs.Routes.ToList().Where(x => x.ROUTE_ID == selectedindex).First();
                    cs.Routes.Attach(rowrou);
                    cs.Routes.Remove(rowrou);
                    cs.SaveChanges();
                }
                this.routetxt.Text = "";
                this.costtxt.Text = "0,00";
                this.busid.SelectedItem = null;
                cs.Routes.Load();
                routes.ItemsSource = cs.Routes.Local.ToBindingList();
            }
        }
        private void UpdRow(object sender, RoutedEventArgs e)
        {
            using (cs = new AutovauxContext())
            {
                try
                {
                    int selectedindex = (routes.SelectedItem as Routes).ROUTE_ID;
                    var row = cs.Routes.ToList().Where(x => x.ROUTE_ID == selectedindex).First();
                    row.ROUTE = routetxt.Text;
                    row.TRAVEL_COSTS = Decimal.Parse(costtxt.Text);
                    if (busid.SelectedItem != null)
                    {
                        int bus = Int32.Parse(busid.SelectedItem.ToString());
                        var id = cs.Bus_fleet.Where(x => x.BUS_ID == bus).Select(x => x.BUS_ID).First();
                        row.BUS_ID_FK = id;
                    }
                    cs.SaveChanges();
                }
                catch
                {
                    MessageBoxUI mui = new MessageBoxUI("Входная строка имела неверный формат", MessageType.Error, MessageButtons.Ok);
                }
                cs.Routes.Load();
                routes.ItemsSource = cs.Routes.Local.ToBindingList();
            }
        }
        private void RowSelected(object sender, SelectionChangedEventArgs e)
        {
            using (cs = new AutovauxContext())
            {
                if (routes.SelectedItem != null)
                {
                    int selectedindex = (routes.SelectedItem as Routes).ROUTE_ID;
                    var row = cs.Routes.ToList().Where(x => x.ROUTE_ID == selectedindex).ToList();
                    string route = row.Select(x => x.ROUTE).First();
                    decimal costs = row.Select(x => x.TRAVEL_COSTS).First();
                    int bus_id = row.Select(x => x.BUS_ID_FK).First();
                    this.routetxt.Text = route;
                    this.costtxt.Text = costs.ToString();
                    this.busid.SelectedItem = bus_id;
                }
                cs.Routes.Load();
            }
        }
    }
}
