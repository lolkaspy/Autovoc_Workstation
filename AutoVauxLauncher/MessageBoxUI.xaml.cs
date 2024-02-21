using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
namespace AutoVauxLauncher
{
    /// <summary>
    /// Interaction logic for MessageBoxCustom.xaml
    /// </summary>
    public partial class MessageBoxUI : Window
    {
        public MessageBoxUI(string Message, MessageType Type, MessageButtons Buttons)
        {
            InitializeComponent();
            txtMessage.Text = Message;
            switch (Type)
            {
                case MessageType.Info:
                    txtTitle.Text = "Информация";
                    var uriSource = new Uri(@"/Icons/icon_prog.png", UriKind.Relative);
                    iconMessage.Source = new BitmapImage(uriSource);
                    break;
                case MessageType.Confirmation:
                    txtTitle.Text = "Подтверждение";
                    uriSource = new Uri(@"/Icons/question.png", UriKind.Relative);
                    iconMessage.Source = new BitmapImage(uriSource);
                    break;
                case MessageType.ProgInfo:
                    txtTitle.Text = "О программе";
                    uriSource = new Uri(@"/Icons/arm_logo.png", UriKind.Relative);
                    iconMessage.Width = 115;
                    iconMessage.Source = new BitmapImage(uriSource);
                    break;
                case MessageType.Exit:
                    txtTitle.Text = "Завершение сессии";
                    uriSource = new Uri(@"/Icons/icon_exit.png", UriKind.Relative);
                    iconMessage.Source = new BitmapImage(uriSource);
                    break;
                case MessageType.Success:
                    {
                        txtTitle.Text = "Успешно";
                        uriSource = new Uri(@"/Icons/Ok.png", UriKind.Relative);
                        iconMessage.Source = new BitmapImage(uriSource);
                    }
                    break;
                case MessageType.Warning:
                    txtTitle.Text = "Предупреждение";
                    uriSource = new Uri(@"/Icons/warn.png", UriKind.Relative);
                    iconMessage.Source = new BitmapImage(uriSource);
                    break;
                case MessageType.Error:
                    {
                        txtTitle.Text = "Ошибка";
                        uriSource = new Uri(@"/Icons/icon_error.png", UriKind.Relative);
                        iconMessage.Source = new BitmapImage(uriSource);
                    }
                    break;
            }
            switch (Buttons)
            {
                case MessageButtons.OkCancel:
                    btnYes.Visibility = Visibility.Collapsed; btnNo.Visibility = Visibility.Collapsed;
                    break;
                case MessageButtons.YesNo:
                    btnOk.Visibility = Visibility.Collapsed; btnCancel.Visibility = Visibility.Collapsed;
                    break;
                case MessageButtons.Ok:
                    btnOk.Visibility = Visibility.Visible;
                    btnCancel.Visibility = Visibility.Collapsed;
                    btnYes.Visibility = Visibility.Collapsed; btnNo.Visibility = Visibility.Collapsed;
                    break;
            }
        }
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            // Begin dragging the window
            this.DragMove();
        }
        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
    public enum MessageType
    {
        Info,
        Confirmation,
        Success,
        Warning,
        Error,
        ProgInfo,
        Exit
    }
    public enum MessageButtons
    {
        OkCancel,
        YesNo,
        Ok,
    }
}