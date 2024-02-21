using ARMDatabase;
using EasyDox;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
namespace AutoVauxLauncher
{
    /// <summary>
    /// Логика взаимодействия для TestPage.xaml
    /// </summary>
    public partial class TicketPage : UserControl
    {
        string conn = System.Configuration.ConfigurationManager.ConnectionStrings["AutoVauxLauncher.Properties.Settings.sqlconn"].ConnectionString;
        public TicketPage()
        {
            InitializeComponent();
            cs = new AutovauxContext();
            var r = cs.Routes.Select(x => x.ROUTE).ToList();
            route.ItemsSource = r;
            var dat = cs.Timetables.Select(x => x.DATE).ToList();
            List<string> dates = new List<string>();
            foreach (DateTime dt in dat)
            {
                dates.Add(dt.ToString("yyyy-MM-dd"));
            }
            var bus = cs.Routes.Select(x => x.BUS_ID_FK).ToList();
            num.ItemsSource = bus;
            var plans = cs.Rate_plans.Select(x => x.PLAN_NAME).ToList();
            rate.ItemsSource = plans;
        }
        AutovauxContext cs;
        private void BusSelect(object sender, SelectionChangedEventArgs e)
        {
            cs.Routes.Load();
            int selection = Int32.Parse(num.SelectedItem.ToString());
            var r = cs.Routes.Where(x => x.BUS_ID_FK == selection).Select(x => x.ROUTE).First();
            route.SelectedItem = r;
            int p = Int32.Parse(num.SelectedItem.ToString());
            var a = cs.Routes.Where(x => x.BUS_ID_FK == p).Select(x => x.TRAVEL_COSTS).FirstOrDefault();
            var da = new SqlDataAdapter($"SELECT DEPARTURE_TIME FROM TimetableList as tl LEFT JOIN Timetables as ts ON tl.DATE=ts.DATE RIGHT JOIN Routes as rs ON tl.ROUTE_ID_FK=rs.ROUTE_ID WHERE tl.DATE='{date.Text}' and rs.BUS_ID_FK={num.SelectedItem.ToString()};", conn);
            System.Data.DataTable dt = new System.Data.DataTable();
            da.Fill(dt);
            List<string> dts = new List<string>();
            foreach (DataRow rv in dt.Rows)
            {
                dts.Add(DateTime.Parse(rv[0].ToString()).ToString("t"));
            }
            time.ItemsSource = dts;
            SqlConnection sqlconn = new SqlConnection(conn);
            sqlconn.Open();
            SqlCommand sql = new SqlCommand($"SELECT BUS_HASCARD FROM Routes as r JOIN Bus_fleet as bf ON r.BUS_ID_FK = bf.BUS_ID JOIN Bus_types as bt ON bf.BUS_TYPE_ID_FK=bt.BUS_TYPE_ID WHERE r.BUS_ID_FK = {selection}", sqlconn);
            SqlDataReader sdr = sql.ExecuteReader();
            while (sdr.Read())
            {
                if ((bool)sdr.GetValue(0) == true)
                {
                    tcard.IsEnabled = true;
                }
                else
                {
                    tcard.IsEnabled = false;
                    tcard.IsChecked = false;
                    rate.IsEnabled = false;
                }
            }
            back.IsChecked = false;
            Price.Text = a.ToString();
            sqlconn.Close();
        }
        private void PrintCheck(object sender, RoutedEventArgs e)
        {
            try
            {
                if (rate.SelectedItem == null)
                {
                    if ((bool)back.IsChecked == false)
                    {
                        var fieldValues1 = new Dictionary<string, string> {
    {"Маршрут", route.Text},
    {"Цена",  Price.Text},
    {"Дата",  (DateTime.Now).ToString("d")},
    {"Время", (DateTime.Now).ToString("t")},
    {"Отправление", (DateTime.Parse(time.Text)).ToString("t")},
    };
                        var engine1 = new Engine();
                        string path1 = Environment.CurrentDirectory + "\\chek.docx";
                        string path2 = Environment.CurrentDirectory + "\\For_print1.docx";
                        engine1.Merge(path1, fieldValues1, path2);
                    }
                    else
                    {
                        string[] parts = route.Text.Split(new[] { " - " }, StringSplitOptions.None);
                        string backroute = parts[1] + " - " + parts[0];
                        int p = Int32.Parse(num.SelectedItem.ToString());
                        var a = cs.Routes.Where(x => x.BUS_ID_FK == p).Select(x => x.TRAVEL_COSTS).FirstOrDefault();
                        var fieldValues2 = new Dictionary<string, string> {
    {"Маршрут", route.Text},
    {"Цена",  Price.Text},
    {"Цена1",  a.ToString()},
    {"Цена2",  a.ToString()},
    {"ОбрМаршрут",  backroute},
    {"Дата",  (DateTime.Now).ToString("d")},
    {"Время", (DateTime.Now).ToString("t")},
    {"Отправление", (DateTime.Parse(time.Text)).ToString("t")},
    };
                        var engine2 = new Engine();
                        string path3 = Environment.CurrentDirectory + "\\chek3.docx";
                        string path4 = Environment.CurrentDirectory + "\\For_print3.docx";
                        engine2.Merge(path3, fieldValues2, path4);
                    }
                }
                else
                {
                    var fieldValues3 = new Dictionary<string, string> {
    {"Билет", rate.SelectedItem.ToString()},
    {"Цена",  Price.Text},
    {"Дата",  (DateTime.Now).ToString("d")},
    {"Время", (DateTime.Now).ToString("t")},
    {"Отпр", "с "+DateTime.Parse(date.Text).ToString("d") }
    };
                    var engine3 = new Engine();
                    string path5 = Environment.CurrentDirectory + "\\chek2.docx";
                    string path6 = Environment.CurrentDirectory + "\\For_print2.docx";
                    engine3.Merge(path5, fieldValues3, path6);
                }
            }
            catch
            {
                MessageBoxUI mui = new MessageBoxUI("Файлы шаблонов не найдены", MessageType.Error, MessageButtons.Ok);
                mui.ShowDialog();
            }
        }
        private void BackChecked(object sender, RoutedEventArgs e)
        {
            tcard.IsChecked = false;
            tcard.IsEnabled = false;
            if (Price.Text != "0,00")
            {
                Price.Text = (Decimal.Parse(Price.Text) * 2).ToString();
            }
            sum.Text = $"{(Decimal.Parse(Price.Text) / 2).ToString()}+{(Decimal.Parse(Price.Text) / 2).ToString()}";
        }
        private void BackUnchecked(object sender, RoutedEventArgs e)
        {
            sum.Text = "";
            if (Price.Text != "0,00")
            {
                Price.Text = (Decimal.Parse(Price.Text) / 2).ToString();
            }
            int selection = Int32.Parse(num.SelectedItem.ToString());
            SqlConnection sqlconn = new SqlConnection(conn);
            sqlconn.Open();
            SqlCommand sql = new SqlCommand($"SELECT BUS_HASCARD FROM Routes as r JOIN Bus_fleet as bf ON r.BUS_ID_FK = bf.BUS_ID JOIN Bus_types as bt ON bf.BUS_TYPE_ID_FK=bt.BUS_TYPE_ID WHERE r.BUS_ID_FK = {selection}", sqlconn);
            SqlDataReader sdr = sql.ExecuteReader();
            while (sdr.Read())
            {
                if ((bool)sdr.GetValue(0) == true)
                {
                    tcard.IsEnabled = true;
                }
                else
                {
                    tcard.IsEnabled = false;
                    tcard.IsChecked = false;
                    rate.IsEnabled = false;
                }
            }
            sqlconn.Close();
        }
        private void RouteSelect(object sender, SelectionChangedEventArgs e)
        {
            cs.Routes.Load();
            string selection = route.SelectedItem.ToString();
            var r = cs.Routes.Where(x => x.ROUTE == selection).Select(x => x.BUS_ID_FK).First();
            num.SelectedItem = r;
            int p = Int32.Parse(num.SelectedItem.ToString());
            var a = cs.Routes.Where(x => x.BUS_ID_FK == p).Select(x => x.TRAVEL_COSTS).FirstOrDefault();
            var da = new SqlDataAdapter($"SELECT DEPARTURE_TIME FROM TimetableList as tl LEFT JOIN Timetables as ts ON tl.DATE=ts.DATE RIGHT JOIN Routes as rs ON tl.ROUTE_ID_FK=rs.ROUTE_ID WHERE tl.DATE='{date.Text}' and rs.ROUTE=N'{route.SelectedItem.ToString()}';", conn);
            System.Data.DataTable dt = new System.Data.DataTable();
            da.Fill(dt);
            List<string> dts = new List<string>();
            foreach (DataRow rv in dt.Rows)
            {
                dts.Add(DateTime.Parse(rv[0].ToString()).ToString("t"));
            }
            time.ItemsSource = dts;
            back.IsChecked = false;
            Price.Text = a.ToString();
        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            cs.Dispose();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            cs = new AutovauxContext();
        }
        private void tcardChecked(object sender, RoutedEventArgs e)
        {
            back.IsEnabled = false;
            cs.Rate_plans.Load();
            back.IsChecked = false;
            rate.IsEnabled = true;
        }
        private void tcardUnchecked(object sender, RoutedEventArgs e)
        {
            back.IsEnabled = true;
            rate.IsEnabled = false;
            rate.SelectedItem = null;
            int p = Int32.Parse(num.SelectedItem.ToString());
            var a = cs.Routes.Where(x => x.BUS_ID_FK == p).Select(x => x.TRAVEL_COSTS).FirstOrDefault();
            Price.Text = a.ToString();
        }
        private void rate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (rate.SelectedItem != null)
            {
                string selection = rate.SelectedItem.ToString();
                var r = cs.Rate_plans.Where(x => x.PLAN_NAME == selection).Select(x => x.RATE_COSTS).First();
                Price.Text = r.ToString();
            }
        }
    }
}
