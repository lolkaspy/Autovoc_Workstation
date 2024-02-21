using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
namespace AutoVauxLauncher
{
    /// <summary>
    /// Логика взаимодействия для TimePicker.xaml
    /// </summary>
    public partial class TimePicker : UserControl
    {
        public TimePicker()
        {
            InitializeComponent();
            timerMinuteIncrease = new DispatcherTimer();
            timerMinuteIncrease.Interval = TimeSpan.FromMilliseconds(120);
            timerMinuteIncrease.Tick += TimerMinuteIncrease_Tick;
            timerMinuteDecrease = new DispatcherTimer();
            timerMinuteDecrease.Interval = TimeSpan.FromMilliseconds(120);
            timerMinuteDecrease.Tick += TimerMinuteDecrease_Tick;
            timerHourIncrease = new DispatcherTimer();
            timerHourIncrease.Interval = TimeSpan.FromMilliseconds(120);
            timerHourIncrease.Tick += TimerHourIncrease_Tick;
            timerHourDecrease = new DispatcherTimer();
            timerHourDecrease.Interval = TimeSpan.FromMilliseconds(120);
            timerHourDecrease.Tick += TimerHourDecrease_Tick; 
        }
        private void TimerHourDecrease_Tick(object sender, EventArgs e)
        {
            if (Hour.Text != "")
            {
                if (Int32.Parse(Hour.Text) < 11 && Int32.Parse(Hour.Text) > 1)
                {
                    Hour.Text = "0" + (Int32.Parse(Hour.Text) - 1).ToString();
                }
                else if (Int32.Parse(Hour.Text) == 0)
                {
                    Hour.Text = "23";
                }
                else
                {
                    Hour.Text = (Int32.Parse(Hour.Text) - 1).ToString();
                }
            }
        }
        private void TimerHourIncrease_Tick(object sender, EventArgs e)
        {
            if (Hour.Text != "")
            {
                if (Int32.Parse(Hour.Text) < 9)
                {
                    Hour.Text = "0" + (Int32.Parse(Hour.Text) + 1).ToString();
                }
                else
                {
                    if (Int32.Parse(Hour.Text) == 24)
                    {
                        Hour.Text = "0";
                    }
                    else
                    {
                        Hour.Text = (Int32.Parse(Hour.Text) + 1).ToString();
                    }
                }
            }
        }
        private void TimerMinuteDecrease_Tick(object sender, EventArgs e)
        {
            if (Minute.Text != "")
            {
                if (Int32.Parse(Minute.Text) < 11 && Int32.Parse(Minute.Text) != 0)
                {
                    Minute.Text = "0" + (Int32.Parse(Minute.Text) - 1).ToString();
                }
                else if (Int32.Parse(Minute.Text) == 0)
                {
                    Minute.Text = "59";
                    if (Hour.Text == "0" || Hour.Text == "00")
                    {
                        Hour.Text = "23";
                    }
                    else
                    {
                        Hour.Text = (Int32.Parse(Hour.Text) - 1).ToString();
                    }
                }
                else
                {
                    Minute.Text = (Int32.Parse(Minute.Text) - 1).ToString();
                }
            }
        }
        private void TimerMinuteIncrease_Tick(object sender, EventArgs e)
        {
            if (Minute.Text != "")
            {
                if (Int32.Parse(Minute.Text) < 9)
                {
                    Minute.Text = "0" + (Int32.Parse(Minute.Text) + 1).ToString();
                }
                else
                {
                    Minute.Text = (Int32.Parse(Minute.Text) + 1).ToString();
                }
                if (Int32.Parse(Minute.Text) == 60)
                {
                    Minute.Text = "00";
                    Hour.Text = (Int32.Parse(Hour.Text) + 1).ToString();
                }
            }
        }
        private DispatcherTimer timerMinuteIncrease;
        private DispatcherTimer timerMinuteDecrease;
        private DispatcherTimer timerHourIncrease;
        private DispatcherTimer timerHourDecrease;
        private void Hour_Added(object sender, RoutedEventArgs e)
        {
            if (Hour.Text != "")
            {
                if (Int32.Parse(Hour.Text) < 9)
                {
                    Hour.Text = "0" + (Int32.Parse(Hour.Text) + 1).ToString();
                }
                else
                {
                    if (Int32.Parse(Hour.Text) == 24)
                    {
                        Hour.Text = "0";
                    }
                    else
                    {
                        Hour.Text = (Int32.Parse(Hour.Text) + 1).ToString();
                    }
                }
            }
        }
        private void Hour_Removed(object sender, RoutedEventArgs e)
        {
            if (Hour.Text != "")
            {
                if (Int32.Parse(Hour.Text) < 11 && Int32.Parse(Hour.Text)>1)
                {
                    Hour.Text = "0" + (Int32.Parse(Hour.Text) - 1).ToString();
                }
                else if (Int32.Parse(Hour.Text)==0)
                {
                    Hour.Text = "23";
                }
                else
                {
                    Hour.Text = (Int32.Parse(Hour.Text) - 1).ToString();
                }
            }
        }
        private void Minute_Added(object sender, RoutedEventArgs e)
        {
            if (Minute.Text != "")
            {
                if (Int32.Parse(Minute.Text) < 9)
                {
                    Minute.Text = "0" + (Int32.Parse(Minute.Text) + 1).ToString();
                }
                else
                {
                    Minute.Text = (Int32.Parse(Minute.Text) + 1).ToString();
                }
                if (Int32.Parse(Minute.Text) == 60)
                {
                    Minute.Text = "00";
                    Hour.Text = (Int32.Parse(Hour.Text) + 1).ToString();
                }
            }
        }
        private void Minute_Removed(object sender, RoutedEventArgs e)
        {
            if (Minute.Text != "") {
                if (Int32.Parse(Minute.Text) < 11 && Int32.Parse(Minute.Text) != 0 )
            {
                Minute.Text = "0" + (Int32.Parse(Minute.Text) - 1).ToString();
            }
                else if (Int32.Parse(Minute.Text) == 0)
                {
                    Minute.Text = "59";
                    if (Hour.Text == "0" || Hour.Text == "00")
                    {
                        Hour.Text = "23";
                    }
                    else
                    {
                        Hour.Text = (Int32.Parse(Hour.Text) - 1).ToString();
                    }
                }
            else
            {
                Minute.Text = (Int32.Parse(Minute.Text) - 1).ToString();
            }
            }
        }
        private void HourChanged(object sender, TextChangedEventArgs e)
        {
            if (Hour.Text != "")
            {
                if (Int32.Parse(Hour.Text) > 24 )
                {
                    Hour.Text = "23";
                }
                else if (Hour.Text == "24")
                {
                    Hour.Text = "0";
                }
                else if (Hour.Text == "00")
                {
                    Hour.Text = "0";
                }
            }
        }
        private void MinuteChanged(object sender, TextChangedEventArgs e)
        {
            if (Minute.Text != "")
            {
                if(Int32.Parse(Minute.Text) > 60 )
                {
                    Minute.Text = "59";
                }
                else if (Int32.Parse(Minute.Text)==60)
                {
                    Minute.Text = "00";
                    if (Hour.Text == "23")
                    {
                        Hour.Text = "0";
                    }
                    else
                    {
                        if (Int32.Parse(Hour.Text) < 9)
                        {
                            Hour.Text = "0" + (Int32.Parse(Hour.Text) + 1).ToString();
                        }
                        else
                        {
                            Hour.Text = (Int32.Parse(Hour.Text) + 1).ToString();
                        }
                    }  
                }
                else if(Int32.Parse(Minute.Text)>=10 && Int32.Parse(Minute.Text) / 10 >= 6)
                {
                    Minute.Text = "5" + Minute.Text[1];
                }
            }
        }
        private void HourInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = "0123456789".IndexOf(e.Text) < 0;
        }
        private void MinuteInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = "0123456789 ,".IndexOf(e.Text) < 0;
        }
        private void MinuteMultiadding(object sender, MouseButtonEventArgs e)
        {
            timerMinuteIncrease.Start();
        }
        private void MinuteMultiaddingStop(object sender, MouseButtonEventArgs e)
        {
            timerMinuteIncrease.Stop();
        }
        private void MinuteMultiremoving(object sender, MouseButtonEventArgs e)
        {
            timerMinuteDecrease.Start();
        }
        private void MinuteMultiremovingStop(object sender, MouseButtonEventArgs e)
        {
            timerMinuteDecrease.Stop();
        }
        private void HourMultiadding(object sender, MouseButtonEventArgs e)
        {
            timerHourIncrease.Start();
        }
        private void HourMultiaddingStop(object sender, MouseButtonEventArgs e)
        {
            timerHourIncrease.Stop();
        }
        private void HourMultiremoving(object sender, MouseButtonEventArgs e)
        {
            timerHourDecrease.Start();
        }
        private void HourMultiremovingStop(object sender, MouseButtonEventArgs e)
        {
            timerHourDecrease.Stop();
        }
    }
}
