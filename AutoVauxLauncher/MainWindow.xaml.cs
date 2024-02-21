using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using static AutoVauxLauncher.App;
namespace AutoVauxLauncher
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //pages
        TimetablePage page = new TimetablePage();
        TicketPage ticket = new TicketPage();
        BronPage bron = new BronPage();
        Reports master = new Reports();
        SprPage spr = new SprPage();
        private static readonly string appData = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MyLoginSaver");
        public MainWindow()
        {
            InitializeComponent();
            Page.Content = page;
            //butrasp.Background = (Brush)bc.ConvertFrom("#CAD0C8");
        }
        //BrushConverter bc= new BrushConverter();
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            switch (WindowState)
            {
                case (WindowState.Minimized):
                    WindowState = WindowState.Normal;
                    break;
                case (WindowState.Normal):
                    WindowState = WindowState.Minimized;
                    break;
            }
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.GetCurrentProcess().Kill();
            }
            catch
            {
            }
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            bool? result = new MessageBoxUI("Вы действительно хотите выйти?", MessageType.Exit, MessageButtons.YesNo).ShowDialog();
            if (result.Value)
            {
                if (File.Exists(System.IO.Path.Combine(appData, "settings.json")))
                {
                    File.Delete(System.IO.Path.Combine(appData, "settings.json"));
                }
                try
                {
                    Process.GetCurrentProcess().Kill();
                }
                catch
                {
                }
            }
        }
        private void ProgInfo_Click(object sender, RoutedEventArgs e)
        {
            bool? result = new MessageBoxUI("Версия 1.0001\r\nСоздатель: ...\r\nПравообладатель: ООО “Автовок”", MessageType.ProgInfo, MessageButtons.Ok).ShowDialog();
        }
        private void ticket_Click(object sender, RoutedEventArgs e)
        {
            Page.Content = ticket;
        }
        private void Rasp_Click(object sender, RoutedEventArgs e)
        {
            Page.Content = page;
        }
        private void Bron_Click(object sender, RoutedEventArgs e)
        {
            Page.Content = bron;
        }
        private void Report_Click(object sender, RoutedEventArgs e)
        {
            Page.Content = master;
        }
        private void Spr_Click(object sender, RoutedEventArgs e)
        {
            Page.Content = spr;
        }
        private void Backup_Click(object sender, RoutedEventArgs e)
        {
            BackUpUI bkup = new BackUpUI();
            bkup.ShowDialog();
        }
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }
        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            EditProfile ep = new EditProfile();
            ep.ShowDialog();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
            string settingsText = File.ReadAllText(System.IO.Path.Combine(appData, "settings.json"));
            settings = JsonSerializer.Deserialize<MySettings>(settingsText);
            if (settings.Fname.Length > 20 || settings.Lname.Length > 20)
            {
                login_txt.FontSize = 14;
            }
            login_txt.Text = settings.Fname + " " + settings.Lname;
            }
            catch
            {
                MessageBoxUI mui = new MessageBoxUI("К сожалению, файл настроек не найден, авторизуйтесь заново.", MessageType.Error, MessageButtons.Ok);
                mui.ShowDialog();
                if (mui.DialogResult == true)
                {
                    this.Close();
                }
            }
        }
        private void Window_Activated(object sender, EventArgs e)
        {
            try
            {
                string settingsText = File.ReadAllText(System.IO.Path.Combine(appData, "settings.json"));
                settings = JsonSerializer.Deserialize<MySettings>(settingsText);
                if (settings.Fname.Length > 20 || settings.Lname.Length > 20)
                {
                    login_txt.FontSize = 14;
                }
                login_txt.Text = settings.Fname + " " + settings.Lname;
            }
            catch
            {
                MessageBoxUI mui = new MessageBoxUI("К сожалению, файл настроек не найден, авторизуйтесь заново.", MessageType.Error, MessageButtons.Ok);
                mui.ShowDialog();
                if (mui.DialogResult == true)
                {
                    this.Close();
                }
            }
        }
    }
}
