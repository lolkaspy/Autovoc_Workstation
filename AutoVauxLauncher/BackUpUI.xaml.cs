using Ookii.Dialogs.Wpf;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;
namespace AutoVauxLauncher
{
    /// <summary>
    /// Логика взаимодействия для BackUpUI.xaml
    /// </summary>
    public partial class BackUpUI : Window
    {
        public BackUpUI()
        {
            InitializeComponent();
        }
        private void ShowFolderBrowserDialog()
        {
            var dialog = new VistaFolderBrowserDialog();
            dialog.Description = "Выберите каталог.";
            dialog.UseDescriptionForTitle = true; // This applies to the Vista style dialog only, not the old dialog.
            if (!VistaFolderBrowserDialog.IsVistaFolderDialogSupported)
            {
                MessageBoxUI mui = new MessageBoxUI("Because you are not using Windows Vista or later, the regular folder browser dialog will be used. Please use Windows Vista to see the new dialog.", MessageType.Warning, MessageButtons.Ok);
                mui.ShowDialog();
            }
            if ((bool)dialog.ShowDialog(this))
            {
                backup_path.Text = dialog.SelectedPath;
            }
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
        private void ShowFolderDialog(object sender, RoutedEventArgs e)
        {
            ShowFolderBrowserDialog();
        }
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            // Begin dragging the window
            this.DragMove();
        }
        private void Save_BackupFile(object sender, RoutedEventArgs e)
        {
            string connectionString = "Data Source=SQL6030.site4now.net,1433;Initial Catalog=db_a99962_dbautovoc;User Id=db_a99962_dbautovoc_admin;Password=qwerty27.99;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string backupQuery = "BACKUP DATABASE db_a99962_dbautovoc TO DISK=N'" + backup_path.Text + "/backup.bak'";
                using (SqlCommand command = new SqlCommand(backupQuery, connection))
                {
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == 262)
                        {
                            MessageBoxUI mui = new MessageBoxUI("У вас нет прав на создание резервной копии базы данных.", MessageType.Error, MessageButtons.Ok);
                            mui.ShowDialog();
                        }
                        if (ex.Number == 3201)
                        {
                            MessageBoxUI mui = new MessageBoxUI("Невозможно сохранить копию, так как устройство недоступно, либо указан неверный путь", MessageType.Error, MessageButtons.Ok);
                            mui.ShowDialog();
                        }
                    }
                }
            }
        }
    }
}
