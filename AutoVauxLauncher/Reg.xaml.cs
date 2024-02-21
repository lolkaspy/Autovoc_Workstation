using ARMDatabase;
using AutoVauxLauncher.HelpClasses;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
namespace AutoVauxLauncher
{
    public partial class Reg : Window
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
        bool State;
        int Role_id;
        private static readonly string appData = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MyLoginSaver");
        public Reg()
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
            State = false;
            Role_id = 3;
            Password = pwd.Password;
            Password2 = pwd2.Password;
            Salt = hs.Saltate();
            using (AutovauxContext lp = new AutovauxContext())
            {
                if (Login != "" && Password != "" && Fname != "" && Lname != "" && Password == Password2)
                {
                    if (email_validation.IsMatch(Login) && password_validation.IsMatch(Password))
                    {
                        pwd.Background = (Brush)bc.ConvertFrom("#D9D9D9");
                        txtLogin.Background = (Brush)bc.ConvertFrom("#D9D9D9");
                        lp.Profiles.Load();
                        var emails = (from em in lp.Profiles
                                      select em.EMAIL).Where(x => x == Login).ToList();
                        if (emails.Count > 0)
                        {
                            MessageBoxUI mui = new MessageBoxUI("Аккаунт с такой почтой уже есть", MessageType.Warning, MessageButtons.Ok);
                            mui.ShowDialog();
                        }
                        else
                        {
                            Profiles pf = new Profiles
                            {
                                LNAME = Lname,
                                FNAME = Fname,
                                EMAIL = Login,
                                HASH_PASSWORD = hs.SHA512M(Password, Salt),
                                SALT = Salt,
                                PROFILE_STATE = State,
                                ROLE_ID_FK = Role_id,
                                BIRTH_DATE = BirthDate,
                                PHONE = Phone,
                                MNAME = Mname
                            };
                            lp.Profiles.Add(pf);
                            lp.SaveChanges();
                            MessageBoxUI mui = new MessageBoxUI("Профиль успешно зарегистрирован. Ожидайте активации", MessageType.Success, MessageButtons.Ok);
                            mui.ShowDialog();
                            this.Close();
                        }
                    }
                }
                else
                {
                    MessageBoxUI mui = new MessageBoxUI("Не введены все необходимые данные или не совпадают пароли", MessageType.Warning, MessageButtons.Ok);
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
            // Begin dragging the window
            this.DragMove();
        }
    }
}