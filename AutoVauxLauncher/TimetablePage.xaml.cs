using ARMDatabase;
using AutoVauxLauncher.HelpClasses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static AutoVauxLauncher.App;

namespace AutoVauxLauncher
{
    /// <summary>
    /// Логика взаимодействия для TestPage.xaml
    /// </summary>
    public partial class TimetablePage : UserControl
    {
        string conn = System.Configuration.ConfigurationManager.ConnectionStrings["AutoVauxLauncher.Properties.Settings.sqlconn"].ConnectionString;
        AutovauxContext cs;
        TimePicker tp1 = new TimePicker();
        TimePicker tp2 = new TimePicker();
        private static readonly string appData = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MyLoginSaver");
        public TimetablePage()
        {
            string settingsText = File.ReadAllText(System.IO.Path.Combine(appData, "settings.json"));
            settings = JsonSerializer.Deserialize<MySettings>(settingsText);
            InitializeComponent();
            this.time1.Content = tp1;
            this.time2.Content = tp2;
            List<string> ru_crit = new List<string>() { "Номер автобуса", "Маршрут", "Время отправления", "Время прибытия" };
            foreach (string item in ru_crit)
            {
                category.Items.Add(item);
            }
            cs = new AutovauxContext();
                var buses = (from busfleet in cs.Bus_fleet
                             select busfleet.BUS_ID).ToList();
                var routes = (from route in cs.Routes
                              select route.ROUTE).ToList();
             UpdateGrid();
                onroute.ItemsSource = routes;
        }
        private void UpdateGrid()
        {
            var timetables = (from timetable in cs.TimetableTemp
                              join route in cs.Routes on timetable.ROUTE_ID_FK equals route.ROUTE_ID
                              select new Custom
                              {
                                  TEMP_ID = timetable.TEMP_ID,
                                  DEPARTURE_TIME = timetable.DEPARTURE_TIME,
                                  ARRIVAL_TIME = timetable.ARRIVAL_TIME,
                                  ROUTE = route.ROUTE,
                                  BUS_ID = route.BUS_ID_FK
                              }).ToList();
            cs.TimetableTemp.Load();
            cs.Routes.Load();
            cs.Bus_fleet.Load();
            cs.GenialView.Load();
            LoginsTable.ItemsSource = timetables;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try {
                var timetables = (from timetable in cs.TimetableTemp
                                  join route in cs.Routes on timetable.ROUTE_ID_FK equals route.ROUTE_ID
                                  select new Custom
                                  {
                                      TEMP_ID = timetable.TEMP_ID,
                                      DEPARTURE_TIME = timetable.DEPARTURE_TIME,
                                      ARRIVAL_TIME = timetable.ARRIVAL_TIME,
                                      ROUTE = route.ROUTE,
                                      BUS_ID = route.BUS_ID_FK
                                  }).ToList();
                string str_time1 = tp1.Hour.Text + ":" + tp1.Minute.Text + ":00";
                string str_time2 = tp2.Hour.Text + ":" + tp2.Minute.Text + ":00";
                if (tp1.Hour.Text.Length != 2)
                {
                    str_time1 = "0" + tp1.Hour.Text + ":" + tp1.Minute.Text + ":00";
                }
                if (tp2.Hour.Text.Length != 2)
                {
                    str_time2 = "0" + tp2.Hour.Text + ":" + tp2.Minute.Text + ":00";
                }
                DateTime s1 = DateTime.ParseExact(str_time1, "T", CultureInfo.InvariantCulture);
                DateTime s2 = DateTime.ParseExact(str_time2, "T", CultureInfo.InvariantCulture);
                DateTime dep_time_dt;
                DateTime arr_time_dt;
                DateTime curr_time = DateTime.Parse(date.Text);
                dep_time_dt = curr_time.Date.Add(s1.TimeOfDay);
                arr_time_dt = curr_time.Date.Add(s2.TimeOfDay);
                string r;
                if (onroute.SelectedItem != null)
                {
                    r = onroute.SelectedItem.ToString();
                    TimetableTemp tr = new TimetableTemp
                    {
                        ARRIVAL_TIME = arr_time_dt,
                        DEPARTURE_TIME = dep_time_dt,
                        ROUTE_ID_FK = cs.Routes.Where(x => x.ROUTE == r).Select(x => x.ROUTE_ID).First()
                    };
                    var newlist = new List<TimetableTemp>();
                    if (period.IsChecked == true)
                    {
                        double delay_minutes;
                        double.TryParse(delay.Text, out delay_minutes);
                        for (int i = 0; i <= Int32.Parse(count_rec.Text) - 1; i++)
                        {
                            arr_time_dt = arr_time_dt.AddMinutes(delay_minutes);
                            dep_time_dt = dep_time_dt.AddMinutes(delay_minutes);
                            newlist.Add(new TimetableTemp
                            {
                                ARRIVAL_TIME = arr_time_dt,
                                DEPARTURE_TIME = dep_time_dt,
                                ROUTE_ID_FK = cs.Routes.Where(x => x.ROUTE == r).Select(x => x.ROUTE_ID).First()
                            });
                        }
                        foreach (TimetableTemp tabletemp in newlist)
                        {
                            cs.TimetableTemp.Add(tabletemp);
                        }
                        MessageBoxUI mui = new MessageBoxUI($"Успешно добавлено {count_rec.Text} записи", MessageType.Success, MessageButtons.Ok);
                        mui.ShowDialog();
                        cs.SaveChanges();
                    }
                    else
                    {
                        cs.TimetableTemp.Add(tr);
                        MessageBoxUI mui = new MessageBoxUI($"Запись успешно добавлена", MessageType.Success, MessageButtons.Ok);
                        mui.ShowDialog();
                        cs.SaveChanges();
                    }
                }
                UpdateGrid();
            }
            catch
            {
                MessageBoxUI mui = new MessageBoxUI("Не выбрана дата", MessageType.Warning, MessageButtons.Ok);
                mui.ShowDialog();
            }
        }
        private void period_Unchecked(object sender, RoutedEventArgs e)
        {
            count_rec.Text = "";
            delay.Text = "";
        }
        private void Delete_TimeTable(object sender, RoutedEventArgs e)
        {
        }
        private void Save_TimeTable(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime dt1;
                dt1 = DateTime.Parse(date.Text);
                string name = $"Расписание от {dt1.Date.ToString("dd-MM-yyyy")}";
                var timetables = (from timetable in cs.TimetableTemp
                                  join route in cs.Routes on timetable.ROUTE_ID_FK equals route.ROUTE_ID
                                  select new Custom
                                  {
                                      TEMP_ID = timetable.TEMP_ID,
                                      DEPARTURE_TIME = timetable.DEPARTURE_TIME,
                                      ARRIVAL_TIME = timetable.ARRIVAL_TIME,
                                      ROUTE = route.ROUTE,
                                      BUS_ID = route.BUS_ID_FK
                                  });
                var dp_times = timetables.Select(x => x.DEPARTURE_TIME).ToList();
                var ar_times = timetables.Select(x => x.ARRIVAL_TIME).ToList();
                var routes = timetables.Select(x => x.ROUTE).ToList();
                var busids = timetables.Select(x => x.BUS_ID).ToList();
                cs.TimetableTemp.Load();
                var list = new List<TimetableList>();
                for (int i = 0; i < timetables.Count(); i++)
                {
                    string r2 = routes[i];
                    var selectrouteindex = cs.Routes.Where(x => x.ROUTE == r2).Select(x => x.ROUTE_ID).First();
                    list.Add(new TimetableList()
                    {
                        DATE = dt1,
                        DEPARTURE_TIME = dp_times[i],
                        ARRIVAL_TIME = ar_times[i],
                        ROUTE_ID_FK = selectrouteindex
                    });
                }
                foreach (TimetableList item in list)
                {
                    cs.TimetableList.Add(item);
                }
                cs.Timetables.Add(new Timetables() { DATE = dt1, NAME = name });
                MessageBoxUI mui = new MessageBoxUI("Сохранено", MessageType.Success, MessageButtons.Ok);
                mui.ShowDialog();
                cs.SaveChanges();
                cs.Database.ExecuteSqlCommand("DELETE FROM TimetableTemp");
                cs.SaveChanges();
                UpdateGrid();
            }
            catch
            {
                MessageBoxUI mui = new MessageBoxUI("Некорректная дата", MessageType.Error, MessageButtons.Ok);
                mui.ShowDialog();
            }
        }
        private void Add_TimeTable(object sender, RoutedEventArgs e)
        {
              LoginsTable.SelectionMode = DataGridSelectionMode.Single;
              tp1.IsEnabled = true;
              tp2.IsEnabled = true;
              period.IsEnabled = true;
              count_rec.IsEnabled = true;
              delay.IsEnabled = true;
              onroute.IsEnabled = true;
              add.IsEnabled = true;
              del.IsEnabled = true;
              ch.IsEnabled = true;
            RaspTable.Visibility = Visibility.Hidden;
            LoginsTable.Visibility = Visibility.Visible;
        }
        private void Search(object sender, RoutedEventArgs e)
        {
            if (searchBox.Text == "")
            {
                UpdateGrid();
            }
            else
            {
                switch (category.SelectedItem.ToString())
                {
                    case "Номер автобуса":
                        try
                        {
                            int searchitem = Int32.Parse(searchBox.Text);
                            var found = (from timetable in cs.TimetableTemp
                                        join route in cs.Routes on timetable.ROUTE_ID_FK equals route.ROUTE_ID
                                        where route.BUS_ID_FK == searchitem
                                        select new Custom
                                        {
                                            TEMP_ID = timetable.TEMP_ID,
                                            DEPARTURE_TIME = timetable.DEPARTURE_TIME,
                                            ARRIVAL_TIME = timetable.ARRIVAL_TIME,
                                            ROUTE = route.ROUTE,
                                            BUS_ID = route.BUS_ID_FK
                                        }).ToList();
                            LoginsTable.ItemsSource = found.ToList();
                        }
                        catch
                        {
                            MessageBoxUI mui = new MessageBoxUI("Вы ввели не число", MessageType.Error, MessageButtons.Ok);
                            mui.ShowDialog();
                        }
                        break;
                    case "Маршрут":
                        try
                        {
                            string searchitem2 = searchBox.Text;
                            var found = (from timetable in cs.TimetableTemp
                                        join route in cs.Routes on timetable.ROUTE_ID_FK equals route.ROUTE_ID
                                        where route.ROUTE.Contains(searchitem2)
                                        select new Custom
                                        {
                                            TEMP_ID = timetable.TEMP_ID,
                                            DEPARTURE_TIME = timetable.DEPARTURE_TIME,
                                            ARRIVAL_TIME = timetable.ARRIVAL_TIME,
                                            ROUTE = route.ROUTE,
                                            BUS_ID = route.BUS_ID_FK
                                        }).ToList();
                            LoginsTable.ItemsSource = found.ToList();
                        }
                        catch
                        {
                            MessageBoxUI mui = new MessageBoxUI("Вы ввели число, ожидалась строка", MessageType.Error, MessageButtons.Ok);
                            mui.ShowDialog();
                        }
                        break;
                    case "Время отправления":
                        try
                        {
                            if (searchBox.Text.Contains(">"))
                            {
                                int searchitem3 = Int32.Parse(searchBox.Text.Remove(searchBox.Text.IndexOf(">"), 1));
                                var found = (from timetable in cs.TimetableTemp
                                             join route in cs.Routes on timetable.ROUTE_ID_FK equals route.ROUTE_ID
                                             where timetable.DEPARTURE_TIME.Hour>searchitem3
                                            select new Custom
                                            {
                                                TEMP_ID = timetable.TEMP_ID,
                                                DEPARTURE_TIME = timetable.DEPARTURE_TIME,
                                                ARRIVAL_TIME = timetable.ARRIVAL_TIME,
                                                ROUTE = route.ROUTE,
                                                BUS_ID = route.BUS_ID_FK
                                            }).ToList();
                                LoginsTable.ItemsSource = found.ToList();
                            }
                            else if (searchBox.Text.Contains("<"))
                            {
                                int searchitem4 = Int32.Parse(searchBox.Text.Remove(searchBox.Text.IndexOf("<"), 1));
                                var found = (from timetable in cs.TimetableTemp
                                             join route in cs.Routes on timetable.ROUTE_ID_FK equals route.ROUTE_ID
                                             where timetable.DEPARTURE_TIME.Hour < searchitem4
                                             select new Custom
                                             {
                                                 TEMP_ID = timetable.TEMP_ID,
                                                 DEPARTURE_TIME = timetable.DEPARTURE_TIME,
                                                 ARRIVAL_TIME = timetable.ARRIVAL_TIME,
                                                 ROUTE = route.ROUTE,
                                                 BUS_ID = route.BUS_ID_FK
                                             }).ToList();
                                LoginsTable.ItemsSource = found.ToList();
                            }
                            else
                            {
                                throw new Exception();
                            }
                        }
                        catch
                        {
                            MessageBoxUI mui = new MessageBoxUI("Вы ввели критерий неправильного формата, правильный: \">час\"", MessageType.Error, MessageButtons.Ok);
                            mui.ShowDialog();
                        }
                        break;
                    case "Время прибытия":
                        try
                        {
                            if (searchBox.Text.Contains(">"))
                            {
                                int searchitem5 = Int32.Parse(searchBox.Text.Remove(searchBox.Text.IndexOf(">"), 1));
                                var found = (from timetable in cs.TimetableTemp
                                             join route in cs.Routes on timetable.ROUTE_ID_FK equals route.ROUTE_ID
                                             where timetable.ARRIVAL_TIME.Hour > searchitem5
                                             select new Custom
                                             {
                                                 TEMP_ID = timetable.TEMP_ID,
                                                 DEPARTURE_TIME = timetable.DEPARTURE_TIME,
                                                 ARRIVAL_TIME = timetable.ARRIVAL_TIME,
                                                 ROUTE = route.ROUTE,
                                                 BUS_ID = route.BUS_ID_FK
                                             }).ToList();
                                LoginsTable.ItemsSource = found.ToList();
                            }
                            else if (searchBox.Text.Contains("<"))
                            {
                                int searchitem6 = Int32.Parse(searchBox.Text.Remove(searchBox.Text.IndexOf("<"), 1));
                                var found = (from timetable in cs.TimetableTemp
                                             join route in cs.Routes on timetable.ROUTE_ID_FK equals route.ROUTE_ID
                                             where timetable.ARRIVAL_TIME.Hour < searchitem6
                                             select new Custom
                                             {
                                                 TEMP_ID = timetable.TEMP_ID,
                                                 DEPARTURE_TIME = timetable.DEPARTURE_TIME,
                                                 ARRIVAL_TIME = timetable.ARRIVAL_TIME,
                                                 ROUTE = route.ROUTE,
                                                 BUS_ID = route.BUS_ID_FK
                                             }).ToList();
                                LoginsTable.ItemsSource = found.ToList();
                            }
                            else
                            {
                                throw new Exception();
                            }
                        }
                        catch
                        {
                            MessageBoxUI mui = new MessageBoxUI("Вы ввели критерий неправильного формата, правильный: \">час\"", MessageType.Error, MessageButtons.Ok);
                            mui.ShowDialog();
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        private void Change_Record(object sender, RoutedEventArgs e)
        {
            LoginsTable.SelectionMode = DataGridSelectionMode.Single;
            ch.Visibility = Visibility.Visible;
            add.Visibility = Visibility.Hidden;
            del.Visibility = Visibility.Hidden;
        }
        Custom table = new Custom();
        private void Change(object sender, RoutedEventArgs e)
        {
            try
            {
                var timetables = (from timetable in cs.TimetableTemp
                                  join route in cs.Routes on timetable.ROUTE_ID_FK equals route.ROUTE_ID
                                  select new Custom
                                  {
                                      TEMP_ID = timetable.TEMP_ID,
                                      DEPARTURE_TIME = timetable.DEPARTURE_TIME,
                                      ARRIVAL_TIME = timetable.ARRIVAL_TIME,
                                      ROUTE = route.ROUTE,
                                      BUS_ID = route.BUS_ID_FK
                                  }).ToList();
                if (LoginsTable.SelectedItems.Count > 0)
                {
                    if (date.Text != "")
                    {
                        table = (Custom)LoginsTable.SelectedItem;
                        int ind = Int32.Parse(table.TEMP_ID.ToString());
                        var timetable = cs.TimetableTemp.Where(x => x.TEMP_ID == ind).FirstOrDefault();
                        string str_time1 = tp1.Hour.Text + ":" + tp1.Minute.Text + ":00";
                        string str_time2 = tp2.Hour.Text + ":" + tp2.Minute.Text + ":00";
                        if (tp1.Hour.Text.Length != 2)
                        {
                            str_time1 = "0" + tp1.Hour.Text + ":" + tp1.Minute.Text + ":00";
                        }
                        if (tp2.Hour.Text.Length != 2)
                        {
                            str_time2 = "0" + tp2.Hour.Text + ":" + tp2.Minute.Text + ":00";
                        }
                        DateTime s1 = DateTime.ParseExact(str_time1, "T", CultureInfo.InvariantCulture);
                        DateTime s2 = DateTime.ParseExact(str_time2, "T", CultureInfo.InvariantCulture);
                        DateTime dep_time_dt;
                        DateTime arr_time_dt;
                        DateTime curr_time = DateTime.Parse(date.Text);
                        dep_time_dt = curr_time.Date.Add(s1.TimeOfDay);
                        arr_time_dt = curr_time.Date.Add(s2.TimeOfDay);
                        try
                        {
                            timetable.DEPARTURE_TIME = dep_time_dt;
                            timetable.ARRIVAL_TIME = arr_time_dt;
                            var routeid = cs.Routes.Where(x => x.ROUTE == onroute.SelectedItem.ToString()).Select(x => x.BUS_ID_FK).FirstOrDefault();
                            var route = cs.Routes.Where(x => x.ROUTE == onroute.SelectedItem.ToString()).Select(x => x.ROUTE_ID).FirstOrDefault();
                            timetable.ROUTE_ID_FK = route;
                        }
                        catch
                        {
                        }
                        cs.SaveChanges();
                    }
                }
                UpdateGrid();
            }
            catch
            {
                MessageBoxUI mui = new MessageBoxUI("Не выбрана дата", MessageType.Warning, MessageButtons.Ok);
                mui.ShowDialog();
            }
        }
        private void Delete_Record(object sender, RoutedEventArgs e)
        {
                if (LoginsTable.SelectedItems.Count > 0)
                {
                    try
                {
                    Custom table = (Custom)LoginsTable.SelectedItem;
                    if (table != null)
                    {
                        var timetable = cs.TimetableTemp.Where(x => x.TEMP_ID == table.TEMP_ID).FirstOrDefault();
                        cs.TimetableTemp.Attach(timetable);
                        cs.TimetableTemp.Remove(timetable);
                        cs.SaveChanges();
                    }
                }
                catch
                {
                    MessageBoxUI mui = new MessageBoxUI("К сожалению, что-то пошло не так при удалении записи, попробуйте ещё", MessageType.Error, MessageButtons.Ok);
                    mui.ShowDialog();
                }
                cs.GenialView.Load();
                cs.TimetableTemp.Load();
                cs.Routes.Load();
                cs.Bus_fleet.Load();
                UpdateGrid();
            }
        }
        private void Update_Click(object sender, RoutedEventArgs e)
        {
            UpdateGrid();
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            RaspTable.Visibility = Visibility.Visible;
            LoginsTable.Visibility = Visibility.Hidden;
                   tp1.IsEnabled = false;
                   tp2.IsEnabled = false;
                   period.IsEnabled = false;
                   count_rec.IsEnabled = false;
                   delay.IsEnabled = false;
                   onroute.IsEnabled = false;
                   add.IsEnabled = false;
                   del.IsEnabled = false;
                   ch.IsEnabled = false;
            var tt = cs.Timetables.Select(x => x).ToList();
                RaspTable.ItemsSource = tt;
        }
        private void TableLoaded(object sender, RoutedEventArgs e)
        {
            cs = new AutovauxContext();
            UpdateGrid();
        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            cs.Dispose();
        }
        private void DelayInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = "0123456789".IndexOf(e.Text) < 0;
        }
        private void CountInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = "0123456789".IndexOf(e.Text) < 0;
        }
        private void RowSelected(object sender, SelectionChangedEventArgs e)
        {
            if (LoginsTable.SelectedItem != null)
            {
                int selectedindex = (LoginsTable.SelectedItem as Custom).TEMP_ID;
                var row  = (from timetable in cs.TimetableTemp
                                        join route in cs.Routes on timetable.ROUTE_ID_FK equals route.ROUTE_ID
                                        where timetable.TEMP_ID==selectedindex
                                        select new Custom
                                        {
                                            TEMP_ID = timetable.TEMP_ID,
                                            DEPARTURE_TIME = timetable.DEPARTURE_TIME,
                                            ARRIVAL_TIME = timetable.ARRIVAL_TIME,
                                            ROUTE = route.ROUTE,
                                            BUS_ID = route.BUS_ID_FK
                                        }).ToList();
                string r = row.Select(x => x.ROUTE).First();
                DateTime dt = row.Select(x => x.DEPARTURE_TIME).First();
                DateTime at = row.Select(x => x.ARRIVAL_TIME).First();
                int b = row.Select(x => x.BUS_ID).First();
                onroute.SelectedItem = r;
                if (dt.Hour.ToString().Length != 2)
                {
                    tp1.Hour.Text = "0" + dt.Hour.ToString();
                }
                else
                {
                    tp1.Hour.Text = dt.Hour.ToString();
                }
                if (at.Hour.ToString().Length != 2)
                {
                    tp2.Hour.Text = "0" + at.Hour.ToString();
                }
                else
                {
                    tp2.Hour.Text = at.Hour.ToString();
                }
                if (dt.Minute.ToString().Length != 2)
                {
                    tp1.Minute.Text = "0" + dt.Minute.ToString();
                }
                else
                {
                    tp1.Minute.Text = dt.Minute.ToString();
                }
                if (at.Minute.ToString().Length != 2)
                {
                    tp2.Minute.Text = "0" + at.Minute.ToString();
                }
                else
                {
                    tp2.Minute.Text = at.Minute.ToString();
                }
            }
            cs.TimetableTemp.Load();
        }
    }
}
