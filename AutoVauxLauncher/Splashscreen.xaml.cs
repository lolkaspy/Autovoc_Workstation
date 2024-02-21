using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
namespace AutoVauxLauncher
{
    /// <summary>
    /// Логика взаимодействия для Splashscreen.xaml
    /// </summary>
    public partial class Splashscreen : Window
    {
        public Splashscreen()
        {
            InitializeComponent();
        }
        private static readonly string appData = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MyLoginSaver");
        private void SplashRendered(object sender, EventArgs e)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerAsync();
        }
        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbar.Value = e.ProgressPercentage;
            state.Text = "Загрузка...";
            if (pbar.Value == 100)
            {
                //если файла с настройками нет, авторизация
                if (!File.Exists(System.IO.Path.Combine(appData, "settings.json")))
                {
                    Auth auth = new Auth();
                    Close();
                    auth.ShowDialog();
                }
                else
                {
                    MainWindow mw = new MainWindow();
                    Close();
                    mw.ShowDialog();
                }
            }
        }
        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 0; i <= 100; i++)
            {
                (sender as BackgroundWorker).ReportProgress(i);
                Thread.Sleep(50);
            }
        }
    }
}
