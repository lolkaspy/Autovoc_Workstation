using ARMDatabase;
using AutoVauxLauncher.Tables;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text.Json;
using System.Windows.Controls;
using static AutoVauxLauncher.App;
namespace AutoVauxLauncher
{
    /// <summary>
    /// Логика взаимодействия для TestPage.xaml
    /// </summary>
    public partial class SprPage : UserControl
    {
        string conn = System.Configuration.ConfigurationManager.ConnectionStrings["AutoVauxLauncher.Properties.Settings.sqlconn"].ConnectionString;
        AutovauxContext cs;
        TimePicker tp1 = new TimePicker();
        TimePicker tp2 = new TimePicker();
        private static readonly string appData = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MyLoginSaver");
        public SprPage()
        {
            InitializeComponent();
            table.Content = wel;
            using (cs = new AutovauxContext())
            {
                var da = new SqlDataAdapter("select TABLE_NAME from information_schema.tables\r\nWHERE TABLE_NAME NOT LIKE '%sysdiagrams%'\r\norder by table_name asc\r\noffset 1 rows;", conn);
                string settingsText = File.ReadAllText(System.IO.Path.Combine(appData, "settings.json"));
                settings = JsonSerializer.Deserialize<MySettings>(settingsText);
                switch (settings.Role)
                {
                    case 1:
                        cbox1.IsEnabled = true;
                        List<string> l = new List<string>();
                        var dt = new DataTable();
                        da.Fill(dt);
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            switch (dt.Rows[i][0].ToString())
                            {
                                case "Routes":
                                    cbox1.Items.Add("Маршруты");
                                    break;
                                case "Bus_types":
                                    cbox1.Items.Add("Типы автобусов");
                                    break;
                                case "Bus_fleet":
                                    cbox1.Items.Add("Автобусы");
                                    break;
                                case "Roles":
                                    cbox1.Items.Add("Роли");
                                    break;
                                case "Schemas":
                                    cbox1.Items.Add("Схемы автобусов");
                                    break;
                                case "Profiles":
                                    cbox1.Items.Add("Профили");
                                    break;
                                case "Carriers":
                                    cbox1.Items.Add("Компании-перевозчики");
                                    break;
                                default:
                                    break;
                            }
                            l.Add(dt.Rows[i][0].ToString());
                        }
                        break;
                    case 2:
                        cbox1.IsEnabled = true;
                        var dt2 = new DataTable();
                        da.Fill(dt2);
                        for (int i = 0; i < dt2.Rows.Count; i++)
                        {
                            switch (dt2.Rows[i][0].ToString())
                            {
                                case "Routes":
                                    cbox1.Items.Add("Маршруты");
                                    break;
                                case "Bus_types":
                                    cbox1.Items.Add("Типы автобусов");
                                    break;
                                case "Bus_fleet":
                                    cbox1.Items.Add("Автобусы");
                                    break;
                                case "Roles":
                                    break;
                                case "Schemas":
                                    cbox1.Items.Add("Схемы автобусов");
                                    break;
                                case "Profiles":
                                    break;
                                case "Carriers":
                                    cbox1.Items.Add("Компании-перевозчики");
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    case 3:
                        cbox1.IsEnabled = false;
                        break;
                    default:
                        break;
                }
            }
        }
        CarriersTable ct = new CarriersTable();
        ProfilesTable pt = new ProfilesTable();
        BusFleetTable bft = new BusFleetTable();
        BusTypesTable btt = new BusTypesTable();
        RoutesTable rt = new RoutesTable();
        SchemasTable st = new SchemasTable();
        RolesTable rlst = new RolesTable();
        Welcoming wel = new Welcoming();
        private void cbox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (cs = new AutovauxContext())
            {
                switch (cbox1.SelectedItem.ToString())
                {
                    case "Автобусы":
                        table.Content = bft;
                        break;
                    case "Типы автобусов":
                        table.Content = btt;
                        break;
                    case "Компании-перевозчики":
                        table.Content = ct;
                        break;
                    case "Профили":
                        table.Content = pt;
                        break;
                    case "Роли":
                        table.Content = rlst;
                        break;
                    case "Маршруты":
                        table.Content = rt;
                        break;
                    case "Схемы автобусов":
                        table.Content = st;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
