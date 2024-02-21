using ARMDatabase;
using AutoVauxLauncher.HelpClasses;
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
    public partial class ProfilesTable : UserControl
    {
        AutovauxContext cs;
        HashSetter hs = new HashSetter();
        public ProfilesTable()
        {
            InitializeComponent();
            using (cs = new AutovauxContext())
            {
                cs.Profiles.Load();
                role.ItemsSource = cs.Roles.Select(x => x.ROLE).ToList();
                profiles.ItemsSource = cs.Profiles.Local.ToBindingList();
            }
        }
        private void AddRow(object sender, RoutedEventArgs e)
        {
            using (cs = new AutovauxContext())
            {
                try
                {
                    string salt, hash;
                    salt = hs.Saltate();
                    hash = hs.SHA512M(pwd.Password, salt);
                    Profiles prof;
                    if (role.SelectedItem != null)
                    {
                        prof = new Profiles()
                        {
                            FNAME = fname.Text,
                            LNAME = lname.Text,
                            MNAME = mname.Text,
                            PHONE = phone.Text.Trim('_'),
                            EMAIL = email.Text,
                            BIRTH_DATE = DateTime.Parse(bday.Text),
                            HASH_PASSWORD = hash,
                            SALT = salt,
                            PROFILE_STATE = (bool)state.IsChecked,
                            ROLE_ID_FK = cs.Roles.Where(x => x.ROLE == role.SelectedItem.ToString()).Select(x => x.ROLE_ID).First()
                        };
                    }
                    else
                    {
                        prof = new Profiles()
                        {
                            FNAME = fname.Text,
                            LNAME = lname.Text,
                            MNAME = mname.Text,
                            PHONE = phone.Text.Trim('_'),
                            EMAIL = email.Text,
                            BIRTH_DATE = DateTime.Parse(bday.Text),
                            HASH_PASSWORD = hash,
                            SALT = salt,
                            PROFILE_STATE = (bool)state.IsChecked,
                            ROLE_ID_FK = 3
                        };
                    }
                    cs.Profiles.Add(prof);
                    cs.SaveChanges();
                    pwd.Password = "";
                    role.SelectedItem = null;
                    cs.Profiles.Load();
                    profiles.ItemsSource = cs.Profiles.Local.ToBindingList();
                    profiles.Items.Refresh();
                }
                catch
                {
                    MessageBoxUI mui = new MessageBoxUI("Не введены данные", MessageType.Error, MessageButtons.Ok);
                    mui.ShowDialog();
                }
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
                    int selectedindex = (profiles.SelectedItem as Profiles).PROFILE_ID;
                    var rowsch = cs.Profiles.ToList().Where(x => x.PROFILE_ID == selectedindex).First();
                    cs.Profiles.Attach(rowsch);
                    cs.Profiles.Remove(rowsch);
                    cs.SaveChanges();
                    cs.Profiles.Load();
                    profiles.ItemsSource = cs.Profiles.Local.ToBindingList();
                }
                this.fname.Text = "";
                this.lname.Text = "";
                this.mname.Text = "";
                this.email.Text = "";
                this.phone.Text = "";
                this.pwd.Password = "";
                this.bday.Text = "";
                state.IsChecked = false;
                role.SelectedItem = null;
            }
        }
        private void UpdRow(object sender, RoutedEventArgs e)
        {
            using (cs = new AutovauxContext())
            {
                int selectedindex = (profiles.SelectedItem as Profiles).PROFILE_ID;
                var row = cs.Profiles.ToList().Where(x => x.PROFILE_ID == selectedindex).First();
                string salt, hash;
                row.FNAME = fname.Text;
                row.LNAME = lname.Text;
                row.MNAME = mname.Text;
                row.EMAIL = email.Text;
                row.PHONE = phone.Text.Trim('_');
                row.PROFILE_STATE = (bool)state.IsChecked;
                row.BIRTH_DATE = DateTime.Parse(bday.Text);
                salt = hs.Saltate();
                hash = hs.SHA512M(pwd.Password, salt);
                row.HASH_PASSWORD = hash;
                row.SALT = salt;
                if (role.SelectedItem == null)
                {
                    row.ROLE_ID_FK = 3;
                }
                else
                {
                    row.ROLE_ID_FK = cs.Roles.Where(x => x.ROLE == role.SelectedItem.ToString()).Select(x => x.ROLE_ID).First();
                }
                cs.SaveChanges();
                this.fname.Text = "";
                this.lname.Text = "";
                this.mname.Text = "";
                this.email.Text = "";
                this.phone.Text = "";
                this.pwd.Password = "";
                this.bday.Text = "";
                state.IsChecked = false;
                role.SelectedItem = null;
                cs.Profiles.Load();
                profiles.ItemsSource = cs.Profiles.Local.ToBindingList();
            }
        }
        private void RowSelected(object sender, SelectionChangedEventArgs e)
        {
            using (cs = new AutovauxContext())
            {
                if (profiles.SelectedItem != null)
                {
                    int selectedindex = (profiles.SelectedItem as Profiles).PROFILE_ID;
                    var row = cs.Profiles.ToList().Where(x => x.PROFILE_ID == selectedindex).ToList();
                    string fnamerow = row.Select(x => x.FNAME).First();
                    string lnamerow = row.Select(x => x.LNAME).First();
                    string mnamerow = row.Select(x => x.MNAME).First();
                    string phonerow = row.Select(x => x.PHONE).First();
                    string emailrow = row.Select(x => x.EMAIL).First();
                    bool staterow = row.Select(x => x.PROFILE_STATE).First();
                    DateTime bdayrow = (DateTime)row.Select(x => x.BIRTH_DATE).First();
                    fname.Text = fnamerow;
                    lname.Text = lnamerow;
                    mname.Text = mnamerow;
                    phone.Text = phonerow;
                    email.Text = emailrow;
                    bday.Text = bdayrow.ToString("yyyy-MM-dd");
                    state.IsChecked = staterow;
                    pwd.Password = "";
                }
                cs.Profiles.Load();
            }
        }
        private void Profiles_Unloaded(object sender, RoutedEventArgs e)
        {
            cs.Dispose();
        }
        private void Profiles_Loaded(object sender, RoutedEventArgs e)
        {
            cs = new AutovauxContext();
            cs.Profiles.Load();
            role.ItemsSource = cs.Roles.Select(x => x.ROLE).ToList();
            profiles.ItemsSource = cs.Profiles.Local.ToBindingList();
        }
        private void ShowInfo_Checked(object sender, RoutedEventArgs e)
        {
            pwdhash.Visibility = Visibility.Visible;
            pwdsalt.Visibility = Visibility.Visible;
            roleid.Visibility = Visibility.Visible;
        }
        private void ShowInfo_Unchecked(object sender, RoutedEventArgs e)
        {
            pwdhash.Visibility = Visibility.Hidden;
            pwdsalt.Visibility = Visibility.Hidden;
            roleid.Visibility = Visibility.Hidden;
        }
    }
}
