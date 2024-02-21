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
    public partial class CarriersTable : UserControl
    {
        AutovauxContext cs;
        public CarriersTable()
        {
            InitializeComponent();
            using (cs = new AutovauxContext())
            {
                cs.Carriers.Load();
                carriers.ItemsSource = cs.Carriers.Local.ToBindingList();
            }
        }
        private void AddRow(object sender, RoutedEventArgs e)
        {
            using (cs = new AutovauxContext())
            {
                Carriers sch = new Carriers()
                {
                    ORG_NAME = txtname.Text,
                    ORG_ADDRESS = address.Text,
                    ORG_PHONE = phone.Text,
                    ORG_EMAIL = email.Text
                };
                cs.Carriers.Add(sch);
                cs.SaveChanges();
                cs.Carriers.Load();
                carriers.ItemsSource = cs.Carriers.Local.ToBindingList();
                carriers.Items.Refresh();
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
                    int selectedindex = (carriers.SelectedItem as Carriers).ORG_ID;
                    var rowsch = cs.Carriers.ToList().Where(x => x.ORG_ID == selectedindex).First();
                    cs.Carriers.Attach(rowsch);
                    cs.Carriers.Remove(rowsch);
                    cs.SaveChanges();
                }
                this.txtname.Text = "";
                this.address.Text = "";
                this.phone.Text = "";
                this.email.Text = "";
                cs.Carriers.Load();
                carriers.ItemsSource = cs.Carriers.Local.ToBindingList();
            }
        }
        private void UpdRow(object sender, RoutedEventArgs e)
        {
            using (cs = new AutovauxContext())
            {
                int selectedindex = (carriers.SelectedItem as Carriers).ORG_ID;
                var row = cs.Carriers.ToList().Where(x => x.ORG_ID == selectedindex).First();
                row.ORG_PHONE = phone.Text;
                row.ORG_EMAIL = email.Text;
                row.ORG_NAME = txtname.Text;
                row.ORG_ADDRESS = address.Text;
                cs.SaveChanges();
                cs.Carriers.Load();
                carriers.ItemsSource = cs.Carriers.Local.ToBindingList();
            }
        }
        private void RowSelected(object sender, SelectionChangedEventArgs e)
        {
            using (cs = new AutovauxContext())
            {
                if (carriers.SelectedItem != null)
                {
                    int selectedindex = (carriers.SelectedItem as Carriers).ORG_ID;
                    var row = cs.Carriers.ToList().Where(x => x.ORG_ID == selectedindex).ToList();
                    string ph = row.Select(x => x.ORG_PHONE).First();
                    string em = row.Select(x => x.ORG_EMAIL).First();
                    string ad = row.Select(x => x.ORG_ADDRESS).First();
                    string na = row.Select(x => x.ORG_NAME).First();
                    this.txtname.Text = na;
                    this.address.Text = ad;
                    this.phone.Text = ph;
                    this.email.Text = em;
                }
                cs.Carriers.Load();
            }
        }
    }
}
