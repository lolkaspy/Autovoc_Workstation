using ARMDatabase;
using AutoVauxLauncher.HelpClasses;
using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using static AutoVauxLauncher.App;
namespace AutoVauxLauncher
{
    /// <summary>
    /// Interaction logic for MessageBoxCustom.xaml
    /// </summary>
    public partial class Auth : Window
    {
        string Login, Password;
        private static readonly string appData = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MyLoginSaver");
        string conn = System.Configuration.ConfigurationManager.ConnectionStrings["AutoVauxLauncher.Properties.Settings.sqlconn"].ConnectionString;
        public Auth()
        {
            try
            {
                SqlConnection sqlconn = new SqlConnection(conn);
                sqlconn.Open();
                if (sqlconn.State == 0)
                {
                    MessageBoxUI mui = new MessageBoxUI("Нет соединения с сервером", MessageType.Error, MessageButtons.Ok);
                    mui.ShowDialog();
                    if (mui.DialogResult == true)
                    {
                        this.Close();
                        sqlconn.Close();
                    }
                }
                else
                {
                    sqlconn.Close();
                    InitializeComponent();
                }
            }
            catch (System.IO.FileLoadException ex)
            {
                // Обработка ошибки, если необходимая версия .Net Framework не установлена
                MessageBoxUI mui = new MessageBoxUI("Ошибка: необходима версия .Net Framework 4.7.2 и выше для работы приложения", MessageType.Error, MessageButtons.Ok);
                mui.ShowDialog();
                if (mui.DialogResult == true)
                {
                    this.Close();
                }
            }
        }
        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            HashSetter hs = new HashSetter();
            Login = txtLogin.Text;
            Password = txtPwd.Password;
            string hash = "";
            using (AutovauxContext lp = new AutovauxContext())
            {
                if (Login != "" && Password != "")
                {
                    var profile = (from p in lp.Profiles
                                   select new
                                   {
                                       Email = p.EMAIL,
                                       Hash = p.HASH_PASSWORD,
                                       Status = p.PROFILE_STATE,
                                       Role = p.ROLE_ID_FK,
                                       Salt = p.SALT,
                                       Fname = p.FNAME,
                                       Lname = p.LNAME
                                   }).Where(x => x.Email == Login).ToList();
                    lp.Profiles.Load();
                    if (profile.Count() == 1)
                    {
                        hash = hs.SHA512M(Password, profile[0].Salt);
                        if (profile[0].Hash == hash)
                        {
                            if (profile[0].Status == true)
                            {
                                DirectoryInfo dirInfo = new DirectoryInfo(appData);
                                if (!dirInfo.Exists)
                                {
                                    dirInfo.Create();
                                }
                                settings = new MySettings { Login = Login, Password = Password, Fname = profile[0].Fname, Lname = profile[0].Lname, Role = profile[0].Role };
                                string settingsText = JsonSerializer.Serialize(settings);
                                File.WriteAllText(System.IO.Path.Combine(appData, "settings.json"), settingsText);
                                ProcessStartInfo Info = new ProcessStartInfo();
                                Splashscreen sc = new Splashscreen();
                                Close();
                                sc.Show();
                            }
                            else
                            {
                                MessageBoxUI mui = new MessageBoxUI("Учетная запись ещё не активирована", MessageType.Warning, MessageButtons.Ok);
                                mui.ShowDialog();
                            }
                        }
                        else
                        {
                            MessageBoxUI mui = new MessageBoxUI("Введены неверные данные", MessageType.Error, MessageButtons.Ok);
                            mui.ShowDialog();
                        }
                    }
                    else
                    {
                        MessageBoxUI mui = new MessageBoxUI("Учетная запись не найдена", MessageType.Error, MessageButtons.Ok);
                        mui.ShowDialog();
                    }
                }
                else
                {
                    MessageBoxUI mui = new MessageBoxUI("Вы не ввели данные", MessageType.Error, MessageButtons.Ok);
                    mui.ShowDialog();
                }
            }
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }
        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Reg regform = new Reg();
            regform.ShowDialog();
        }
    }
}