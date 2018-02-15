using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ClockDisp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Red index
        private enum DischargeEnum
        {
            Led_1 = 1,
            Led_2 = 2,
            Led_3 = 3,
            Led_4 = 4,
            // Led_All
        }

        // ссылки на изображения для включения/выкл отдельных палочек
        // key for d_1_4 (led 1, item 4) = '1-4'
        private readonly Dictionary<string, Image> imageLedIndexSet;

        // бинарные данные для отрисовки символов
        private readonly Dictionary<char, LedData> ledDataSet;

        // конструктор
        public MainWindow()
        {
            InitializeComponent();

            // мигание точек
            ThreadPool.QueueUserWorkItem(new WaitCallback((_) =>
            {
                while (true) // так как это пул поток, он завершится сам по закрытии программы
                {
                    // находясь в отдельном потоке
                    // в WPF изменения элементов из WPF формы нужно выполнять в методе Dispatcher.Invoke

                    Thread.Sleep(1000);
                    Dispatcher.Invoke(() => { spl.Opacity = 0.0; });
                    Thread.Sleep(1000);
                    Dispatcher.Invoke(() => { spl.Opacity = 1.0; });
                }
            }));

            // init refs for led index
            imageLedIndexSet = new Dictionary<string, Image>
            {
                { "4-0", d_0_0 },
                { "4-1", d_0_1 },
                { "4-2", d_0_2 },
                { "4-3", d_0_3 },
                { "4-4", d_0_4 },
                { "4-5", d_0_5 },
                { "4-6", d_0_6 },

                { "3-0", d_1_0 },
                { "3-1", d_1_1 },
                { "3-2", d_1_2 },
                { "3-3", d_1_3 },
                { "3-4", d_1_4 },
                { "3-5", d_1_5 },
                { "3-6", d_1_6 },

                { "2-0", d_2_0 },
                { "2-1", d_2_1 },
                { "2-2", d_2_2 },
                { "2-3", d_2_3 },
                { "2-4", d_2_4 },
                { "2-5", d_2_5 },
                { "2-6", d_2_6 },

                { "1-0", d_3_0 },
                { "1-1", d_3_1 },
                { "1-2", d_3_2 },
                { "1-3", d_3_3 },
                { "1-4", d_3_4 },
                { "1-5", d_3_5 },
                { "1-6", d_3_6 },
            };

            // создание бинарных данных (буквы только нижнего регистра)
            ledDataSet = new Dictionary<char, LedData>
            {
                { '0', new LedData("1-11-0-11-1") },
                { '1', new LedData("0-01-0-01-0") },
                { '2', new LedData("1-01-1-10-1") },
                { '3', new LedData("1-01-1-01-1") },
                { '4', new LedData("0-11-1-01-0") },
                { '5', new LedData("1-10-1-01-1") },
                { '6', new LedData("1-10-1-11-1") },
                { '7', new LedData("1-01-0-01-0") },
                { '8', new LedData("1-11-1-11-1") },
                { '9', new LedData("1-11-1-01-1") },

                { '-', new LedData("0-00-1-00-0") },
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

            PrintTime();
        }

        // показывает время в формате HH:MM
        private void PrintTime()
        {
            string hour = DateTime.Now.Hour.ToString();
            if (hour.Length > 1)
            {
                Print(DischargeEnum.Led_4, hour[0]);
                Print(DischargeEnum.Led_3, hour[1]);
            }
            else
            {
                Print(DischargeEnum.Led_4, '0');
                Print(DischargeEnum.Led_3, hour[0]);
            }

            string min = DateTime.Now.Minute.ToString();
            if (min.Length > 1)
            {
                Print(DischargeEnum.Led_2, min[0]);
                Print(DischargeEnum.Led_1, min[1]);
            }
            else
            {
                Print(DischargeEnum.Led_2, '0');
                Print(DischargeEnum.Led_1, min[0]);
            }
        }

        /// <summary> Напечатать символ. </summary>
        /// <param name="ledIndex">Разряд.</param>
        /// <param name="symbol">Symbol.</param>
        private void Print(DischargeEnum ledIndex, char symbol)
        {
            int lednum = (int)ledIndex;

            if (char.IsLetter(symbol))
                symbol = char.ToLower(symbol);

            if (ledDataSet.TryGetValue(symbol, out LedData ld))
            {
                Print(lednum, ld);
            }
            else
            {
                MessageBox.Show(
                    "Print symbol '" + symbol + "' not implemented.",
                    "Print error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        /// <summary> Напечатать символ. </summary>
        /// <param name="ledIndex">Red index.</param>
        /// <param name="data">Led data from 'ledDataSet'.</param>
        private void Print(int ledIndex, LedData ledData)
        {
            imageLedIndexSet[ledIndex + "-0"].Opacity = ledData.A ? 1.0 : 0.0;
            imageLedIndexSet[ledIndex + "-1"].Opacity = ledData.B ? 1.0 : 0.0;
            imageLedIndexSet[ledIndex + "-2"].Opacity = ledData.C ? 1.0 : 0.0;
            imageLedIndexSet[ledIndex + "-3"].Opacity = ledData.D ? 1.0 : 0.0;
            imageLedIndexSet[ledIndex + "-4"].Opacity = ledData.E ? 1.0 : 0.0;
            imageLedIndexSet[ledIndex + "-5"].Opacity = ledData.F ? 1.0 : 0.0;
            imageLedIndexSet[ledIndex + "-6"].Opacity = ledData.G ? 1.0 : 0.0;
        }

        /// <summary> Напечатать символ. </summary>
        /// <param name="ledIndex">Red index.</param>
        /// <param name="data">Strign index set (0-01-0-01-0).</param>
        private void Print(int ledIndex, string data)
        {
            imageLedIndexSet[ledIndex + "-0"].Visibility = data[0] == '0' ? Visibility.Visible : Visibility.Hidden;
            imageLedIndexSet[ledIndex + "-1"].Visibility = data[2] == '0' ? Visibility.Visible : Visibility.Hidden;
            imageLedIndexSet[ledIndex + "-2"].Visibility = data[3] == '0' ? Visibility.Visible : Visibility.Hidden;
            imageLedIndexSet[ledIndex + "-3"].Visibility = data[5] == '0' ? Visibility.Visible : Visibility.Hidden;
            imageLedIndexSet[ledIndex + "-4"].Visibility = data[7] == '0' ? Visibility.Visible : Visibility.Hidden;
            imageLedIndexSet[ledIndex + "-5"].Visibility = data[8] == '0' ? Visibility.Visible : Visibility.Hidden;
            imageLedIndexSet[ledIndex + "-6"].Visibility = data[10] == '0' ? Visibility.Visible : Visibility.Hidden;
        }

        /// <summary> Напечатать символ. </summary>
        /// <param name="ledIndex">Red index.</param>
        /// <param name="a">Magenta index 0.</param>
        /// <param name="d1">Magenta index 1.</param>
        /// <param name="d2">Magenta index 2.</param>
        /// <param name="d3">Magenta index 3.</param>
        /// <param name="d4">Magenta index 4.</param>
        /// <param name="d5">Magenta index 5.</param>
        /// <param name="d6">Magenta index 6.</param>
        private void Print(int ledIndex, bool a, bool b, bool c, bool d, bool e, bool f, bool g)
        {
            imageLedIndexSet[ledIndex + "-0"].Opacity = a ? 1.0 : 0.0;
            imageLedIndexSet[ledIndex + "-1"].Opacity = b ? 1.0 : 0.0;
            imageLedIndexSet[ledIndex + "-2"].Opacity = c ? 1.0 : 0.0;
            imageLedIndexSet[ledIndex + "-3"].Opacity = d ? 1.0 : 0.0;
            imageLedIndexSet[ledIndex + "-4"].Opacity = e ? 1.0 : 0.0;
            imageLedIndexSet[ledIndex + "-5"].Opacity = f ? 1.0 : 0.0;
            imageLedIndexSet[ledIndex + "-6"].Opacity = g ? 1.0 : 0.0;
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
    }
}
