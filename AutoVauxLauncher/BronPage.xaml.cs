using ARMDatabase;
using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
namespace AutoVauxLauncher
{
    /// <summary>
    /// Логика взаимодействия для TestPage.xaml
    /// </summary>
    public partial class BronPage : UserControl
    {
        AutovauxContext cs;
        TimePicker tp = new TimePicker();
        int max_seat;
        string conn = System.Configuration.ConfigurationManager.ConnectionStrings["AutoVauxLauncher.Properties.Settings.sqlconn"].ConnectionString;
        public BronPage()
        {
            InitializeComponent();
            tpicker.Content = tp;
            cs = new AutovauxContext();
            SqlConnection sqlconn = new SqlConnection(conn);
            sqlconn.Open();
            SqlCommand sql = new SqlCommand($"SELECT BUS_ID FROM Bus_fleet as bf JOIN Bus_types as bt on bf.BUS_TYPE_ID_FK = bt.BUS_TYPE_ID JOIN Schemas as ss on bf.SCHEME_ID_FK = ss.SCHEME_ID WHERE BUS_TYPENAME = N'Бронируемый'", sqlconn);
            SqlDataReader sdr = sql.ExecuteReader();
            while (sdr.Read())
            {
                num.Items.Add(sdr.GetInt32(0));
            }
            sqlconn.Close();
        }
        private void num_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (num.SelectedItem != null)
            {
                int n = Int32.Parse(num.SelectedItem.ToString());
                var l = cs.Bus_fleet.Where(x => x.BUS_ID == n).Select(x => x.SCHEME_ID_FK).First();
                max_seat = cs.Schemas.Where(x => x.SCHEME_ID == l).Select(x => x.SEAT_COUNT).First();
            }
        }
        private void UnloadedBron(object sender, RoutedEventArgs e)
        {
            cs.Dispose();
        }
        private void LoadedBron(object sender, RoutedEventArgs e)
        {
            cs = new AutovauxContext();
            UpdateGrid();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (cs = new AutovauxContext())
            {
                try
                {
                    if (date.Text != "" && ontour.Text != "" && num.SelectedItem != null && seat.Text != "")
                    {
                        string str_time1 = tp.Hour.Text + ":" + tp.Minute.Text + ":00";
                        if (tp.Hour.Text.Length != 2)
                        {
                            str_time1 = "0" + tp.Hour.Text + ":" + tp.Minute.Text + ":00";
                        }
                        DateTime s1 = DateTime.ParseExact(str_time1, "T", CultureInfo.InvariantCulture);
                        DateTime dep_time_dt;
                        DateTime curr_time = DateTime.Parse(date.Text);
                        dep_time_dt = curr_time.Date.Add(s1.TimeOfDay);
                        int bus = Int32.Parse(num.SelectedItem.ToString());
                        int c = Int32.Parse(seat.Text);
                        Bron bron = new Bron()
                        {
                            BUS = bus,
                            COUNT = c,
                            DEPART_TIME = dep_time_dt,
                            DATE = curr_time,
                            ONTOUR = ontour.Text
                        };
                        cs.Bron.Add(bron);
                        cs.SaveChanges();
                        UpdateGrid();
                    }
                }
                catch
                {
                    MessageBoxUI mui = new MessageBoxUI("Входная строка имела неверный формат", MessageType.Error, MessageButtons.Ok);
                    mui.ShowDialog();
                }
            }
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Int32.Parse(seat.Text) > max_seat)
            {
                MessageBoxUI mui = new MessageBoxUI($"Указанное количество мест превышает максимальное в автобусе {max_seat}. Рекомендуем забронировать полностью.", MessageType.Warning, MessageButtons.Ok);
                mui.ShowDialog();
            }
        }
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.seat.IsEnabled = false;
            this.seat.Text = max_seat.ToString();
        }
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.seat.IsEnabled = true;
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            bronDataGrid.Visibility = Visibility.Visible;
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            ad.Visibility = Visibility.Visible;
            ch.Visibility = Visibility.Hidden;
            bronDataGrid.Visibility = Visibility.Hidden;
        }
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            using (cs = new AutovauxContext())
            {
                int selectedindex = (bronDataGrid.SelectedItem as Bron).ID_BRON;
                var item = cs.Bron.ToList().Where(x => x.ID_BRON == selectedindex).First();
                cs.Bron.Attach(item);
                cs.Bron.Remove(item);
                cs.SaveChanges();
            }
        }
        int selectedindex;
        private void Change_Click(object sender, RoutedEventArgs e)
        {
            using (cs = new AutovauxContext())
            {
                selectedindex = (bronDataGrid.SelectedItem as Bron).ID_BRON;
                var item = cs.Bron.ToList().Where(x => x.ID_BRON == selectedindex).First();
                date.Text = item.DATE.ToString("yyyy-MM-dd");
                num.SelectedItem = item.BUS;
                if (item.DEPART_TIME.Hour.ToString().Length != 2)
                {
                    tp.Hour.Text = "0" + item.DEPART_TIME.Hour.ToString();
                }
                else
                {
                    tp.Hour.Text = item.DEPART_TIME.Hour.ToString();
                }
                if (item.DEPART_TIME.Minute.ToString().Length != 2)
                {
                    tp.Minute.Text = "0" + item.DEPART_TIME.Minute.ToString();
                }
                else
                {
                    tp.Minute.Text = item.DEPART_TIME.Minute.ToString();
                }
                seat.Text = item.COUNT.ToString();
                ontour.Text = item.ONTOUR;
                ad.Visibility = Visibility.Hidden;
                ch.Visibility = Visibility.Visible;
                bronDataGrid.Visibility = Visibility.Hidden;
            }
        }
        private void Change_Record(object sender, RoutedEventArgs e)
        {
            using (cs = new AutovauxContext())
            {
                try
            {      
            var item = cs.Bron.ToList().Where(x => x.ID_BRON == selectedindex).First();
            item.DATE = DateTime.Parse(date.Text);
            item.BUS = Int32.Parse(num.SelectedItem.ToString());
                item.ONTOUR = ontour.Text;
                item.COUNT = Int32.Parse(seat.Text);
                string str_time1 = tp.Hour.Text + ":" + tp.Minute.Text + ":00";
                if (tp.Hour.Text.Length != 2)
                {
                    str_time1 = "0" + tp.Hour.Text + ":" + tp.Minute.Text + ":00";
                }
                DateTime s1 = DateTime.ParseExact(str_time1, "T", CultureInfo.InvariantCulture);
                DateTime dep_time_dt;
                DateTime curr_time = DateTime.Parse(date.Text);
                dep_time_dt = curr_time.Date.Add(s1.TimeOfDay);
                item.DEPART_TIME = dep_time_dt;
                ontour.Text = "";
                num.SelectedItem = null;
                date.Text = "";
                seat.Text = "";
                tp.Hour.Text = "0";
                tp.Minute.Text = "00";
                full.IsChecked = false;
            ad.Visibility = Visibility.Visible;
            ch.Visibility = Visibility.Hidden;
            bronDataGrid.Visibility = Visibility.Visible;
            }
            catch
            {
                MessageBoxUI mui = new MessageBoxUI("Входная строка имела неверный формат", MessageType.Error, MessageButtons.Ok);
                mui.ShowDialog();
            }
            cs.SaveChanges();
            UpdateGrid();
            }
        }
        private void UpdateGrid()
        {
            cs.Bron.Load();
            bronDataGrid.ItemsSource = cs.Bron.Local.ToBindingList();
        }
    }
}
