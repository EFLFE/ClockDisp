using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using ClockDisp.P543Data;
using ClockDisp.Register;

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

            digits = new UIElement[4][];
            digits[0] = new UIElement[7] { a1, b1, c1, d1, e1, f1, g1 };
            digits[1] = new UIElement[7] { a2, b2, c2, d2, e2, f2, g2 };
            digits[2] = new UIElement[7] { a3, b3, c3, d3, e3, f3, g3 };
            digits[3] = new UIElement[7] { a4, b4, c4, d4, e4, f4, g4 };

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    digits[i][j].Hide();
                }
            }
        }

        private void ReadingQueueDataPool(object _)
        {
            const int delay = 500;
            int d1 = 0;
            int d2 = 0;
            int d3 = 0;
            int d4 = 0;

            while (true)
            {
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

                if (++d1 > 9)
                {
                    d1 = 0;
                    if (++d2 > 9)
                    {
                        d2 = 0;
                        if (++d3 > 9)
                        {
                            d3 = 0;
                            if (++d4 > 9)
                            {
                                d4 = 0;
                            }
                        }
                    }
                }

                while (Compot.OutBuffer.Count > 0)
                {
                    string outData = Compot.OutBuffer.Dequeue();
                }
            }
        }

        private void P543_OnCellSignal(int arg1, int arg2, bool arg3)
        {
            // TODO P543_OnCellSignal
            throw new System.NotImplementedException();
        }

        private void P543_OnRoadSignal(int arg1, bool arg2)
        {
            // TODO P543_OnRoadSignal
            throw new System.NotImplementedException();
        }

        private UIElement[][] digits;
        // 13:46
        private byte[] buffer = new byte[4];

        private void SimulatePrintTime(int discharge)
        {
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
