using System.Windows;
namespace AutoVauxLauncher
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static MySettings settings;
        public static ReportsTable reports;
        public class MySettings
        {
            public string Login { get; set; }
            public string Password { get; set; }
            public string Fname { get; set; }
            public string Lname { get; set; }
            public int Role { get; set; }
        }
        public class ReportsTable
        {
            public string Filename { get; set; }
            public string Date { get; set; }
        }
        public App()
        {
        }
        private void AppStartup(object sender, StartupEventArgs e)
        {
        }
    }
}
