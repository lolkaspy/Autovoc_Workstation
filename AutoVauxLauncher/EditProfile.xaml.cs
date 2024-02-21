using ARMDatabase;
using AutoVauxLauncher.HelpClasses;
using System;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static AutoVauxLauncher.App;
namespace AutoVauxLauncher
{
    public partial class EditProfile : Window
    {
        string Login = "", Password = "", Password2 = "", Fname = "", Lname = "", Mname = "", Phone = "", Salt = "";
        DateTime? BirthDate = null;
        Regex email_validation = new Regex("^\\S+@\\S+\\.\\S+$");
        Regex password_validation = new Regex("^(?=.*?[a-z])(?=.*?[0-9]).{8,}$");
        BrushConverter bc = new BrushConverter();
        HashSetter hs = new HashSetter();
        private void EmailChanged(object sender, TextChangedEventArgs e)
        {
            if (txtLogin.Text.Length != 0)
            {
                ecounter.Text = txtLogin.Text.Length.ToString();
                if (!email_validation.IsMatch(txtLogin.Text))
                {
                    txtLogin.Background = Brushes.PaleVioletRed;
                }
                else
                {
                    txtLogin.Background = (Brush)bc.ConvertFrom("#D9D9D9");
                }
            }
            else
            {
                txtLogin.Background = (Brush)bc.ConvertFrom("#D9D9D9");
                ecounter.Text = "0";
            }
        }
        private void pwd_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (pwd.Password.Length != 0)
            {
                pwdcounter.Text = pwd.Password.Length.ToString();
                if (!password_validation.IsMatch(pwd.Password))
                {
                    pwd.Background = Brushes.PaleVioletRed;
                }
                else
                {
                    pwd.Background = (Brush)bc.ConvertFrom("#D9D9D9");
                }
            }
            else
            {
                pwd.Background = (Brush)bc.ConvertFrom("#D9D9D9");
                pwdcounter.Text = "0";
            }
        }
        private void pwd2_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (pwd2.Password.Length != 0)
            {
                pwd2counter.Text = pwd2.Password.Length.ToString();
                if (pwd2.Password != pwd.Password)
                {
                    pwd2.Background = Brushes.PaleVioletRed;
                }
                else
                {
                    pwd2.Background = (Brush)bc.ConvertFrom("#D9D9D9");
                }
            }
            else
            {
                pwd2.Background = (Brush)bc.ConvertFrom("#D9D9D9");
                pwd2counter.Text = "0";
            }
        }
        private void fname_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (fname.Text != "")
            {
                fcounter.Text = fname.Text.Length.ToString();
            }
            else
            {
                fcounter.Text = "0";
            }
        }
        int Role;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lp = new AutovauxContext();
            string settingsText = File.ReadAllText(System.IO.Path.Combine(appData, "settings.json"));
            settings = JsonSerializer.Deserialize<MySettings>(settingsText);
            lp.Profiles.Load();
            email_this = settings.Login;
            txtLogin.Text = lp.Profiles.Where(x => x.EMAIL == settings.Login).Select(x => x.EMAIL).First();
            fname.Text = lp.Profiles.Where(x => x.EMAIL == settings.Login).Select(x => x.FNAME).First();
            lname.Text = lp.Profiles.Where(x => x.EMAIL == settings.Login).Select(x => x.LNAME).First();
            mname.Text = lp.Profiles.Where(x => x.EMAIL == settings.Login).Select(x => x.MNAME).First();
            birth.Value = lp.Profiles.Where(x => x.EMAIL == settings.Login).Select(x => x.BIRTH_DATE).First();
            phone.Text = lp.Profiles.Where(x => x.EMAIL == settings.Login).Select(x => x.PHONE).First();
            Role = lp.Profiles.Where(x => x.EMAIL == settings.Login).Select(x => x.ROLE_ID_FK).First();
            pwd.Password = settings.Password;
            pwd2.Password = settings.Password;
        }
        private void lname_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (lname.Text != "")
            {
                lcounter.Text = lname.Text.Length.ToString();
            }
            else
            {
                lcounter.Text = "0";
            }
        }
        private void mname_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (mname.Text != "")
            {
                mcounter.Text = mname.Text.Length.ToString();
            }
            else
            {
                mcounter.Text = "0";
            }
        }
        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            lp.Dispose();
        }
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            pwd.IsEnabled = false;
            pwd2.IsEnabled = false;
        }
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            pwd.IsEnabled = true;
            pwd2.IsEnabled = true;
        }
        string email_this;
        AutovauxContext lp;
        private static readonly string appData = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MyLoginSaver");
        public EditProfile()
        {
            InitializeComponent();
        }
        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            Login = txtLogin.Text;
            Fname = fname.Text;
            Lname = lname.Text;
            Mname = mname.Text;
            Phone = phone.Text;     
            Password = pwd.Password;
            Password2 = pwd2.Password;
            Salt = hs.Saltate();
                if (Login != "" && Password != "" && Fname != "" && Lname != "" && Password == Password2)
                {
                    if (email_validation.IsMatch(Login) && password_validation.IsMatch(Password))
                    {
                        pwd.Background = (Brush)bc.ConvertFrom("#D9D9D9");
                        txtLogin.Background = (Brush)bc.ConvertFrom("#D9D9D9");
                        lp.Profiles.Load();
                        var emails = (from em in lp.Profiles
                                      select em.EMAIL).Where(x => x == email_this).ToList();
                        if (emails.Count > 0)
                        {
                            var emails2 = (from em in lp.Profiles
                                          select em.EMAIL).Where(x => x != email_this && x==Login).ToList();
                            if (emails2.Count > 0)
                            {
                                MessageBoxUI mui = new MessageBoxUI("Аккаунт с такой почтой уже есть", MessageType.Warning, MessageButtons.Ok);
                                mui.ShowDialog();
                            }
                            else
                            {
                                try
                                {
                                    var row = lp.Profiles.ToList().Where(x => x.EMAIL == email_this).First();
                                    row.EMAIL = Login;
                                    row.FNAME = Fname;
                                    row.LNAME = Lname;
                                    row.MNAME = Mname;
                                    row.PHONE = Phone.Trim('_');
                                    row.BIRTH_DATE = DateTime.Parse(birth.Text);
                                    string hash = hs.SHA512M(pwd.Password, Salt);
                                    row.HASH_PASSWORD = hash;
                                    row.SALT = Salt;
                                    lp.SaveChanges();
                                    settings = new MySettings { Login = Login, Password = Password, Fname = Fname, Lname = Lname, Role = Role };
                                    string settingsText = JsonSerializer.Serialize(settings);
                                    File.WriteAllText(System.IO.Path.Combine(appData, "settings.json"), settingsText);
                                    MessageBoxUI mui = new MessageBoxUI("Данные профиля успешно обновлены.", MessageType.Success, MessageButtons.Ok);
                                    mui.ShowDialog();
                                    if (mui.DialogResult == true)
                                    {
                                    this.Close();
                                    }
                                }
                                catch
                                {
                                MessageBoxUI mui = new MessageBoxUI("Что-то пошло не так.", MessageType.Error, MessageButtons.Ok);
                                mui.ShowDialog();
                            }
                            }
                        }
                    }
                }
                else
                {
                    MessageBoxUI mui = new MessageBoxUI("Не введены все необходимые данные или не совпадают пароли", MessageType.Warning, MessageButtons.Ok);
                    mui.ShowDialog();
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
    }
}