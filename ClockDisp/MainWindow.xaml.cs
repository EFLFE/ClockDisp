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
                    // TODO compare version
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

            //digits[0] = new UIElement[7] { a1, b1, c1, d1, e1, f1, g1 };
            //digits[1] = new UIElement[7] { a2, b2, c2, d2, e2, f2, g2 };
            //digits[3] = new UIElement[7] { a3, b3, c3, d3, e3, f3, g3 };
            //digits[4] = new UIElement[7] { a4, b4, c4, d4, e4, f4, g4 };
        }

        private void ReadingQueueDataPool(object _)
        {
            const int delay = 100;

            while (true)
            {
                Thread.Sleep(delay);
                /*
                buffer[0] = Exts.DigitToBit(d1);
                buffer[1] = Exts.DigitToBit(d2);
                buffer[2] = Exts.DigitToBit(d3);
                buffer[3] = Exts.DigitToBit(d4);

                Thread.Sleep(delay);
                Dispatcher.Invoke(() => SimulatePrintTime(0));

                Thread.Sleep(delay);
                Dispatcher.Invoke(() => SimulatePrintTime(1));

                Thread.Sleep(delay);
                Dispatcher.Invoke(() => SimulatePrintTime(2));

                Thread.Sleep(delay);
                Dispatcher.Invoke(() => SimulatePrintTime(3));

                while (Compot.OutBuffer.Count > 0)
                {
                    string outData = Compot.OutBuffer.Dequeue();
                }
                */
            }
        }

        private void SimulatePrintTime(int discharge)
        {
            /*
            // каждый разряд
            for (int i = 0; i < 4; i++)
            {
                // каждый сегмент
                for (int j = 0; j < 7; j++)
                {
                    if (i == discharge)
                    {
                        // каждый бит числа
                        for (int z = 0; z < 7; z++)
                        {
                            if (Exts.IsBitSet(buffer[i], z))
                                digits[i][6 - z].Show();
                            else
                                digits[i][6 - z].Hide();
                        }
                    }
                    else
                    {
                        digits[i][j].HideSmooth();
                    }
                }
            }
            */
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
