using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClockDisp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private sealed class LedData
        {
            public readonly bool D0;
            public readonly bool D1;
            public readonly bool D2;
            public readonly bool D3;
            public readonly bool D4;
            public readonly bool D5;
            public readonly bool D6;

            public LedData(bool d0, bool d1, bool d2, bool d3, bool d4, bool d5, bool d6)
            {
                D0 = d0;
                D1 = d1;
                D2 = d2;
                D3 = d3;
                D4 = d4;
                D5 = d5;
                D6 = d6;
            }

            //                        0 23 5 78 10`
            /// <summary> sample: 1: '0-01-0-01-0' </summary>
            public LedData(string data)
            {
                if (data.Length != "0-01-0-01-0".Length)
                    throw new Exception("LedData bad format. Example: '0-01-0-01-0'");

                D0 = data[0] == '1';
                D1 = data[2] == '1';
                D2 = data[3] == '1';
                D3 = data[5] == '1';
                D4 = data[7] == '1';
                D5 = data[8] == '1';
                D6 = data[10] == '1';
            }
        }

        private enum LedIndexEnum
        {
            Led_0 = 0,
            Led_1 = 1,
            Led_2 = 2,
            Led_3 = 3,
            // Led_All
        }

        private enum LedSetItemEnum
        {
            Disable,
            None,
            Enable,
        }

        // key for d_1_4 (led 1, item 4) = '1-4'
        private readonly Dictionary<string, Image> imageLedIndexSet;
        private readonly Dictionary<char, LedData> ledDataSet;

        // for easy code reading
        private const bool
            ON = true,
            OFF = false;

        public MainWindow()
        {
            InitializeComponent();

            ThreadPool.QueueUserWorkItem(new WaitCallback((_) =>
            {
                while (true)
                {
                    Thread.Sleep(1000);
                    Dispatcher.Invoke(() => { spl.Visibility = Visibility.Hidden; });
                    Thread.Sleep(1000);
                    Dispatcher.Invoke(() => { spl.Visibility = Visibility.Visible; });
                }
            }));

            // init refs for led index
            imageLedIndexSet = new Dictionary<string, Image>(28)
            {
                { "0-0", d_0_0 },
                { "0-1", d_0_1 },
                { "0-2", d_0_2 },
                { "0-3", d_0_3 },
                { "0-4", d_0_4 },
                { "0-5", d_0_5 },
                { "0-6", d_0_6 },

                { "1-0", d_1_0 },
                { "1-1", d_1_1 },
                { "1-2", d_1_2 },
                { "1-3", d_1_3 },
                { "1-4", d_1_4 },
                { "1-5", d_1_5 },
                { "1-6", d_1_6 },

                { "2-0", d_2_0 },
                { "2-1", d_2_1 },
                { "2-2", d_2_2 },
                { "2-3", d_2_3 },
                { "2-4", d_2_4 },
                { "2-5", d_2_5 },
                { "2-6", d_2_6 },

                { "3-0", d_3_0 },
                { "3-1", d_3_1 },
                { "3-2", d_3_2 },
                { "3-3", d_3_3 },
                { "3-4", d_3_4 },
                { "3-5", d_3_5 },
                { "3-6", d_3_6 },
            };

            // init led chars data
            ledDataSet = new Dictionary<char, LedData>()
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

                //{ '?', new LedData("") },
            };

            PrintTime();
        }

        // показывает время в формате HH:MM
        private void PrintTime()
        {
            string hour = DateTime.Now.Hour.ToString();
            if (hour.Length > 1)
            {
                Print(LedIndexEnum.Led_0, hour[0]);
                Print(LedIndexEnum.Led_1, hour[1]);
            }
            else
            {

                Print(LedIndexEnum.Led_0, '0');
                Print(LedIndexEnum.Led_1, hour[0]);
            }

            string min = DateTime.Now.Minute.ToString();
            if (min.Length > 1)
            {
                Print(LedIndexEnum.Led_2, min[0]);
                Print(LedIndexEnum.Led_3, min[1]);
            }
            else
            {

                Print(LedIndexEnum.Led_2, '0');
                Print(LedIndexEnum.Led_3, min[0]);
            }
        }

        /// <summary> Напечатать символ. </summary>
        /// <param name="ledIndex">Red index.</param>
        /// <param name="symbol">Symbol.</param>
        private void Print(LedIndexEnum ledIndex, char symbol)
        {
            int lednum = (int)ledIndex;

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
            if (ledIndex < -1 || ledIndex > 3)
                throw new IndexOutOfRangeException("Print: arg 'ledIndex' out of range (only from 0 to 3).");

            imageLedIndexSet[ledIndex + "-0"].Visibility = ledData.D0 == true ? Visibility.Visible : Visibility.Hidden;
            imageLedIndexSet[ledIndex + "-1"].Visibility = ledData.D1 == true ? Visibility.Visible : Visibility.Hidden;
            imageLedIndexSet[ledIndex + "-2"].Visibility = ledData.D2 == true ? Visibility.Visible : Visibility.Hidden;
            imageLedIndexSet[ledIndex + "-3"].Visibility = ledData.D3 == true ? Visibility.Visible : Visibility.Hidden;
            imageLedIndexSet[ledIndex + "-4"].Visibility = ledData.D4 == true ? Visibility.Visible : Visibility.Hidden;
            imageLedIndexSet[ledIndex + "-5"].Visibility = ledData.D5 == true ? Visibility.Visible : Visibility.Hidden;
            imageLedIndexSet[ledIndex + "-6"].Visibility = ledData.D6 == true ? Visibility.Visible : Visibility.Hidden;
        }

        /// <summary> Напечатать символ. </summary>
        /// <param name="ledIndex">Red index.</param>
        /// <param name="data">Strign index set (0-01-0-01-0).</param>
        private void Print(int ledIndex, string data)
        {
            if (ledIndex < -1 || ledIndex > 3)
                throw new IndexOutOfRangeException("Print: arg 'ledIndex' out of range (only from 0 to 3).");

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
        /// <param name="d0">Magenta index 0.</param>
        /// <param name="d1">Magenta index 1.</param>
        /// <param name="d2">Magenta index 2.</param>
        /// <param name="d3">Magenta index 3.</param>
        /// <param name="d4">Magenta index 4.</param>
        /// <param name="d5">Magenta index 5.</param>
        /// <param name="d6">Magenta index 6.</param>
        private void Print(int ledIndex, bool d0, bool d1, bool d2, bool d3, bool d4, bool d5, bool d6)
        {
            if (ledIndex < -1 || ledIndex > 3)
                throw new IndexOutOfRangeException("Print: arg 'ledIndex' out of range (only from 0 to 3).");

            imageLedIndexSet[ledIndex + "-0"].Visibility = d0 == ON ? Visibility.Visible : Visibility.Hidden;
            imageLedIndexSet[ledIndex + "-1"].Visibility = d1 == ON ? Visibility.Visible : Visibility.Hidden;
            imageLedIndexSet[ledIndex + "-2"].Visibility = d2 == ON ? Visibility.Visible : Visibility.Hidden;
            imageLedIndexSet[ledIndex + "-3"].Visibility = d3 == ON ? Visibility.Visible : Visibility.Hidden;
            imageLedIndexSet[ledIndex + "-4"].Visibility = d4 == ON ? Visibility.Visible : Visibility.Hidden;
            imageLedIndexSet[ledIndex + "-5"].Visibility = d5 == ON ? Visibility.Visible : Visibility.Hidden;
            imageLedIndexSet[ledIndex + "-6"].Visibility = d6 == ON ? Visibility.Visible : Visibility.Hidden;
        }

    }
}
