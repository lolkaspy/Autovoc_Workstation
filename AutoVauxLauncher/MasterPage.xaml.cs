using Ookii.Dialogs.Wpf;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.Windows;
using System.Windows.Controls;
namespace AutoVauxLauncher
{
    /// <summary>
    /// Логика взаимодействия для TestPage.xaml
    /// </summary>
    public partial class MasterPage : UserControl
    {
        string document_Name;
        private void ShowOpenFileDialog()
        {
            // As of .Net 3.5 SP1, WPF's Microsoft.Win32.OpenFileDialog class still uses the old style
            VistaOpenFileDialog dialog = new VistaOpenFileDialog();
            dialog.Filter = "All files (*.*)|*.*";
            if (!VistaFileDialog.IsVistaFileDialogSupported)
            {
                MessageBoxUI mui = new MessageBoxUI("Because you are not using Windows Vista or later, the regular folder browser dialog will be used. Please use Windows Vista to see the new dialog.", MessageType.Warning, MessageButtons.Ok);
                mui.ShowDialog();
            }
            // if ((bool)dialog.ShowDialog(this))
            //   MessageBox.Show(this, "The selected file was: " + dialog.FileName, "Sample open file dialog");
        }
        private void SendToPrinter(object sender, RoutedEventArgs e)
        {
            PrintDocument pd = new PrintDocument();
            PrintDialog printDialog = new PrintDialog();
            pd.DocumentName = "";
            if (printDialog.ShowDialog() == true)
                pd.Print();
        }
        List<string> source;
        public MasterPage()
        {
            InitializeComponent();
            var da = new SqlDataAdapter("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME <> 'sysdiagrams'",
                   "Data Source=SQL6030.site4now.net,1433;Initial Catalog=db_a99962_dbautovoc;User Id=db_a99962_dbautovoc_admin;Password=qwerty27.99;");
            var dt = new DataTable();
            da.Fill(dt);
            int rows = dt.Rows.Count;
            source = new List<string>();
            for (int i = 0; i < rows; i++)
            {
                switch (dt.Rows[i].Field<string>("TABLE_NAME"))
                {
                    case "Timetable_records":
                        source.Add("Записи расписания");
                        break;
                    case "Bus_types":
                        source.Add("Типы автобусов");
                        break;
                    case "Profiles":
                        source.Add("Профили");
                        break;
                    case "Roles":
                        source.Add("Роли");
                        break;
                    case "Carriers":
                        source.Add("Компании-перевозчики");
                        break;
                    case "Schemas":
                        source.Add("Схемы автобусов");
                        break;
                    case "Timetable":
                        source.Add("Расписание");
                        break;
                    case "Bus_fleet":
                        source.Add("Парк автобусов");
                        break;
                    case "Routes":
                        source.Add("Маршруты");
                        break;
                    default:
                        source.Add(dt.Rows[i].Field<string>("TABLE_NAME"));
                        break;
                }
            }
            foreach (string item in source)
            {
                journals.Items.Add(item);
            }
        }
        private void AddOne(object sender, RoutedEventArgs e)
        {
            if (journals.SelectedItems.Count > 0)
            {
                journals2.Items.Add(journals.SelectedItem);
                journals.Items.RemoveAt(journals.SelectedIndex);
            }
        }
        private void AddAll(object sender, RoutedEventArgs e)
        {
            foreach (var items in journals.SelectedItems)
            {
                journals2.Items.Add(items);
            }
            for (int i = journals.SelectedItems.Count - 1; i >= 0; i--)
            {
                journals.Items.Remove(journals.SelectedItems[i]);
            }
        }
        private void RemoveOne(object sender, RoutedEventArgs e)
        {
            if (journals2.SelectedItems.Count > 0)
            {
                journals.Items.Add(journals2.SelectedItem);
                journals2.Items.RemoveAt(journals2.SelectedIndex);
            }
        }
        private void RemoveAll(object sender, RoutedEventArgs e)
        {
            foreach (var items in journals2.SelectedItems)
            {
                journals.Items.Add(items);
            }
            for (int i = journals2.SelectedItems.Count - 1; i >= 0; i--)
            {
                journals2.Items.Remove(journals2.SelectedItems[i]);
            }
        }
    }
}
