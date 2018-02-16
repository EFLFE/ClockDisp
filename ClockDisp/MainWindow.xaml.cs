using System.Threading;
using System.Windows;
using System.Windows.Input;
using ClockDisp.P543Data;

namespace ClockDisp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private P543 p543;

        // конструктор
        public MainWindow()
        {
            InitializeComponent();
            p543 = new P543();
            p543.OnRoadSignal += P543_OnRoadSignal;
            p543.OnCellSignal += P543_OnCellSignal;

            ThreadPool.QueueUserWorkItem(ReadingQueueDataPool);

            Compot.OnPortCreated += () =>
            {
                Dispatcher.Invoke(() =>
                {
                    menuComOpen.IsEnabled = true;
                    menuComClose.IsEnabled = true;
                });
            };

            Updater.OnDetectedNewVersion += (data) =>
            {
                Dispatcher.Invoke(() =>
                {
                    // TODO
                    menuGetUpdate.Header = "Get update";

                    MessageBoxResult rez = MessageBox.Show(
                        $"{data.Name} {data.Ver.ToString(3)}\r\n\r\nDownload now?",
                        "New version detected",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Information);

                    if (rez == MessageBoxResult.Yes)
                    {
                        OnDownloadNewVersion(null, null);
                    }
                });
            };

            // отключить все кнопки
            week_day.Hide();
            monday.Hide();
            tuesday.Hide();
            wendesday.Hide();
            thursday.Hide();
            friday.Hide();
            saturday.Hide();
            sunday.Hide();
            bell.Hide();
            program.Hide();
            timer.Hide();
        }

        private void ReadingQueueDataPool(object _)
        {
            while (true)
            {
                Thread.Sleep(10);

                while (Compot.OutBuffer.Count > 0)
                {
                    string outData = Compot.OutBuffer.Dequeue();
                }
            }
        }

        private void P543_OnCellSignal(int arg1, int arg2, bool arg3)
        {
            // TODO
            throw new System.NotImplementedException();
        }

        private void P543_OnRoadSignal(int arg1, bool arg2)
        {
            // TODO
            throw new System.NotImplementedException();
        }

        // [БУД]
        private void OnWeekDayClick(object sender, MouseButtonEventArgs e)
        {
            week_day.ToggleOpacity();
        }

        // [ПН]
        private void OnMondayClick(object sender, MouseButtonEventArgs e)
        {
            monday.ToggleOpacity();
        }

        // [ВТ]
        private void OnTuesdayClick(object sender, MouseButtonEventArgs e)
        {
            tuesday.ToggleOpacity();
        }

        // [СР]
        private void OnWendesdayClick(object sender, MouseButtonEventArgs e)
        {
            wendesday.ToggleOpacity();
        }

        // [ЧТ]
        private void OnThursdayClick(object sender, MouseButtonEventArgs e)
        {
            thursday.ToggleOpacity();
        }

        // [ПТ]
        private void OnFridayClick(object sender, MouseButtonEventArgs e)
        {
            friday.ToggleOpacity();
        }

        // [СБ]
        private void OnSaturdayClick(object sender, MouseButtonEventArgs e)
        {
            saturday.ToggleOpacity();
        }

        // [ВС]
        private void OnSundayClick(object sender, MouseButtonEventArgs e)
        {
            sunday.ToggleOpacity();
        }

        // [колокольчик]
        private void OnBellClick(object sender, MouseButtonEventArgs e)
        {
            bell.ToggleOpacity();
        }

        // [ПРГ]
        private void OnProgramClick(object sender, MouseButtonEventArgs e)
        {
            program.ToggleOpacity();
        }

        // [таймер]
        private void OnTimerClick(object sender, MouseButtonEventArgs e)
        {
            timer.ToggleOpacity();
        }

        private void OnExitClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnOpenCOMSettings(object sender, RoutedEventArgs e)
        {
            new ComPortConfig().ShowDialog();
        }

        private void OnOpenPort(object sender, RoutedEventArgs e)
        {
            Compot.OpenPort();
        }

        private void OnClosePort(object sender, RoutedEventArgs e)
        {
            Compot.ClosePort();
        }

        private void OnDownloadNewVersion(object sender, RoutedEventArgs e)
        {
            if (Updater.LastData == null)
            {
                Updater.ResetTimer();
                return;
            }
            System.Diagnostics.Process.Start(Updater.LastData.AsserDownloadUrl);
        }
    }
}
