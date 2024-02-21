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
    public partial class SchemasTable : UserControl
    {
        AutovauxContext cs;
        public SchemasTable()
        {
            InitializeComponent();
            using (cs = new AutovauxContext())
            {
                cs.Schemas.Load();
                schemas.ItemsSource = cs.Schemas.Local.ToBindingList();
            }
        }
        private void AddRow(object sender, RoutedEventArgs e)
        {
            using (cs = new AutovauxContext())
            {
                Schemas sch = new Schemas() { SEAT_COUNT = Int32.Parse(seats.Text), SPEED = Decimal.Parse(speed.Text), COUNTRY_ABBR = country.Text };
                cs.Schemas.Add(sch);
                cs.SaveChanges();
                cs.Schemas.Load();
                schemas.ItemsSource = cs.Schemas.Local.ToBindingList();
                schemas.Items.Refresh();
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
                    int selectedindex = (schemas.SelectedItem as Schemas).SCHEME_ID;
                    var rowsch = cs.Schemas.ToList().Where(x => x.SCHEME_ID == selectedindex).First();
                    cs.Schemas.Attach(rowsch);
                    cs.Schemas.Remove(rowsch);
                    cs.SaveChanges();
                }
                this.seats.Text = "";
                this.speed.Text = "";
                this.country.Text = "";
                cs.Schemas.Load();
                schemas.ItemsSource = cs.Schemas.Local.ToBindingList();
            }
        }
        private void UpdRow(object sender, RoutedEventArgs e)
        {
            using (cs = new AutovauxContext())
            {
                int selectedindex = (schemas.SelectedItem as Schemas).SCHEME_ID;
                var row = cs.Schemas.ToList().Where(x => x.SCHEME_ID == selectedindex).First();
                row.SPEED = Decimal.Parse(speed.Text);
                row.SEAT_COUNT = Int32.Parse(seats.Text);
                row.COUNTRY_ABBR = country.Text;
                cs.SaveChanges();
                cs.Schemas.Load();
                schemas.ItemsSource = cs.Schemas.Local.ToBindingList();
            }
        }
        private void RowSelected(object sender, SelectionChangedEventArgs e)
        {
            using (cs = new AutovauxContext())
            {
                if (schemas.SelectedItem != null)
                {
                    int selectedindex = (schemas.SelectedItem as Schemas).SCHEME_ID;
                    var row = cs.Schemas.ToList().Where(x => x.SCHEME_ID == selectedindex).ToList();
                    int seatval = row.Select(x => x.SEAT_COUNT).First();
                    decimal speedval = row.Select(x => x.SPEED).First();
                    string countryval = row.Select(x => x.COUNTRY_ABBR).First();
                    this.seats.Text = seatval.ToString();
                    this.speed.Text = speedval.ToString();
                    this.country.Text = countryval.ToString();
                }
                cs.Schemas.Load();
            }
        }
    }
}
