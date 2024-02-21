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
    public partial class BusFleetTable : UserControl
    {
        AutovauxContext cs;
        public BusFleetTable()
        {
            InitializeComponent();
            using (cs = new AutovauxContext())
            {
                cs.Bus_fleet.Load();
                cs.Carriers.Load();
                cs.Schemas.Load();
                cs.Bus_types.Load();
                var orgs = cs.Carriers.Select(x => x.ORG_NAME).ToList();
                var bustypes = cs.Bus_types.Select(x => x.BUS_TYPENAME).ToList();
                var schemas = cs.Schemas.Select(x => x.SCHEME_ID).ToList();
                org.ItemsSource = orgs;
                bustype.ItemsSource = bustypes;
                scheme.ItemsSource = schemas;
                busfleet.ItemsSource = cs.Bus_fleet.Local.ToBindingList();
            }
        }
        private void AddRow(object sender, RoutedEventArgs e)
        {
            using (cs = new AutovauxContext())
            {
                if (org.SelectedItem != null && bustype.SelectedItem != null && scheme.SelectedItem != null)
                {
                    string oname = org.SelectedItem.ToString();
                    int sid = Int32.Parse(scheme.SelectedItem.ToString());
                    string btname = bustype.SelectedItem.ToString();
                    var org_id = cs.Carriers.Where(x => x.ORG_NAME == oname).Select(x => x.ORG_ID).First();
                    var sc = cs.Schemas.Where(x => x.SCHEME_ID == sid).Select(x => x.SCHEME_ID).First();
                    var bid = cs.Bus_types.Where(x => x.BUS_TYPENAME == btname).Select(x => x.BUS_TYPE_ID).First();
                    Bus_fleet bt = new Bus_fleet() { REG_ID = regnum.Text, BUS_TYPE_ID_FK = bid, ORG_ID_FK = org_id, SCHEME_ID_FK = sc };
                    cs.Bus_fleet.Add(bt);
                    cs.SaveChanges();
                }
                cs.Bus_fleet.Load();
                busfleet.ItemsSource = cs.Bus_fleet.Local.ToBindingList();
                busfleet.Items.Refresh();
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
                    int selectedindex = (busfleet.SelectedItem as Bus_fleet).BUS_ID;
                    var rowrol = cs.Bus_fleet.ToList().Where(x => x.BUS_ID == selectedindex).First();
                    cs.Bus_fleet.Attach(rowrol);
                    cs.Bus_fleet.Remove(rowrol);
                    cs.SaveChanges();
                }
                this.regnum.Text = "";
                this.org.SelectedItem = null;
                this.bustype.SelectedItem = null;
                this.scheme.SelectedItem = null;
                cs.Bus_fleet.Load();
                busfleet.ItemsSource = cs.Bus_fleet.Local.ToBindingList();
            }
        }
        private void UpdRow(object sender, RoutedEventArgs e)
        {
            using (cs = new AutovauxContext())
            {
                int selectedindex = (busfleet.SelectedItem as Bus_fleet).BUS_ID;
                var row = cs.Bus_fleet.ToList().Where(x => x.BUS_ID == selectedindex).First();
                if (org.SelectedItem != null && bustype.SelectedItem != null && scheme.SelectedItem != null)
                {
                    string oname = org.SelectedItem.ToString();
                    int sid = Int32.Parse(scheme.SelectedItem.ToString());
                    string btname = bustype.SelectedItem.ToString();
                    var org_id = cs.Carriers.Where(x => x.ORG_NAME == oname).Select(x => x.ORG_ID).First();
                    var sc = cs.Schemas.Where(x => x.SCHEME_ID == sid).Select(x => x.SCHEME_ID).First();
                    var bid = cs.Bus_types.Where(x => x.BUS_TYPENAME == btname).Select(x => x.BUS_TYPE_ID).First();
                    row.REG_ID = regnum.Text;
                    row.ORG_ID_FK = org_id;
                    row.SCHEME_ID_FK = sc;
                    row.BUS_TYPE_ID_FK = bid;
                    cs.SaveChanges();
                }
                cs.Bus_fleet.Load();
                busfleet.ItemsSource = cs.Bus_fleet.Local.ToBindingList();
            }
        }
        private void RowSelected(object sender, SelectionChangedEventArgs e)
        {
            using (cs = new AutovauxContext())
            {
                if (busfleet.SelectedItem != null)
                {
                    int selectedindex = (busfleet.SelectedItem as Bus_fleet).BUS_ID;
                    var row = cs.Bus_fleet.ToList().Where(x => x.BUS_ID == selectedindex).ToList();
                    int bust = row.Select(x => x.BUS_TYPE_ID_FK).First();
                    int sc = row.Select(x => x.SCHEME_ID_FK).First();
                    string reg = row.Select(x => x.REG_ID).First();
                    int oid = row.Select(x => x.ORG_ID_FK).First();
                    var orgn = cs.Carriers.Where(x => x.ORG_ID == oid).Select(x => x.ORG_NAME).First();
                    var bid = cs.Bus_types.Where(x => x.BUS_TYPE_ID == bust).Select(x => x.BUS_TYPENAME).First();
                    this.regnum.Text = reg;
                    this.org.SelectedItem = orgn;
                    this.scheme.SelectedItem = sc;
                    this.bustype.SelectedItem = bid;
                }
                cs.Bus_fleet.Load();
            }
        }
    }
}
