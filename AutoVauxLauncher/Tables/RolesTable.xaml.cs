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
    public partial class RolesTable : UserControl
    {
        AutovauxContext cs;
        public RolesTable()
        {
            InitializeComponent();
            using (cs = new AutovauxContext())
            {
                cs.Roles.Load();
                roles.ItemsSource = cs.Roles.Local.ToBindingList();
            }
        }
        private void AddRow(object sender, RoutedEventArgs e)
        {
            using (cs = new AutovauxContext())
            {
                Roles rol = new Roles() { ROLE = nametxt.Text };
                cs.Roles.Add(rol);
                cs.SaveChanges();
                cs.Roles.Load();
                roles.ItemsSource = cs.Roles.Local.ToBindingList();
                roles.Items.Refresh();
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
                    int selectedindex = (roles.SelectedItem as Roles).ROLE_ID;
                    var rowrol = cs.Roles.ToList().Where(x => x.ROLE_ID == selectedindex).First();
                    cs.Roles.Attach(rowrol);
                    cs.Roles.Remove(rowrol);
                    cs.SaveChanges();
                }
                this.nametxt.Text = "";
                cs.Roles.Load();
                roles.ItemsSource = cs.Roles.Local.ToBindingList();
            }
        }
        private void UpdRow(object sender, RoutedEventArgs e)
        {
            using (cs = new AutovauxContext())
            {
                int selectedindex = (roles.SelectedItem as Roles).ROLE_ID;
                var row = cs.Roles.ToList().Where(x => x.ROLE_ID == selectedindex).First();
                row.ROLE = nametxt.Text;
                cs.SaveChanges();
                cs.Roles.Load();
                roles.ItemsSource = cs.Roles.Local.ToBindingList();
            }
        }
        private void RowSelected(object sender, SelectionChangedEventArgs e)
        {
            using (cs = new AutovauxContext())
            {
                if (roles.SelectedItem != null)
                {
                    int selectedindex = (roles.SelectedItem as Roles).ROLE_ID;
                    var row = cs.Roles.ToList().Where(x => x.ROLE_ID == selectedindex).ToList();
                    string role = row.Select(x => x.ROLE).First();
                    this.nametxt.Text = role;
                }
                cs.Roles.Load();
            }
        }
    }
}
