using System.Threading;
using System.Windows;
using System;
using ClockDisp.P543Data;

namespace ClockDisp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int renderDelayTime = 100;
        private readonly UIElement[][] discharges;

        public MainWindow()
        {
            InitializeComponent();

            Title = "П-543 эмулятор  v" + App.VERSION;
#if DEBUG
            Title += " DEBUG";
#endif

            ConfigData.Load();
            SetRenderDelay(ConfigData.ClockDisplayDelay);

            ThreadPool.QueueUserWorkItem(ReadingQueueDataPool);

            menuRenderTime0.Click += (_, __) => renderDelayTime = -1; // pause
            menuRenderTime001.Click += (_, __) => SetRenderDelay(2); // (!!)
            menuRenderTime01.Click += (_, __) => SetRenderDelay(5);  // (!)
            menuRenderTime1.Click += (_, __) => SetRenderDelay(10);
            menuRenderTime2.Click += (_, __) => SetRenderDelay(25);
            menuRenderTime3.Click += (_, __) => SetRenderDelay(50);
            menuRenderTime4.Click += (_, __) => SetRenderDelay(100);
            menuRenderTime5.Click += (_, __) => SetRenderDelay(250);
            menuRenderTime6.Click += (_, __) => SetRenderDelay(500);

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
                    menuGetUpdate.Header = "Get update";

                    MessageBoxResult rez = MessageBox.Show(
                        $"{data.Name} ({data.Ver.ToString(3)})\r\n\r\nDownload now?",
                        "New version detected",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (rez == MessageBoxResult.Yes)
                    {
                        OnDownloadNewVersion(null, null);
                    }
                });
            };

            discharges = new UIElement[P543.TOTAL_DISCHARGE_COUNT][];
            //discharges[0] = new UIElement[] { a1, b1, c1, d1, e1, f1, g1, timer };
            //discharges[1] = new UIElement[] { a2, b2, c2, d2, e2, f2, g2, program };
            //discharges[2] = new UIElement[] { null, null, null, null, null, null, null, dotdot };
            //discharges[3] = new UIElement[] { a3, b3, c3, d3, e3, f3, g3, bell };
            //discharges[4] = new UIElement[] { a4, b4, c4, d4, e4, f4, g4 };
            //discharges[5] = new UIElement[] { monday, tuesday, wednesday, thursday, friday, saturday, sunday, week_day };

            discharges[0] = new UIElement[P543.TOTAL_SEGMENT_COUNT] { d0a, d0b, d0c, d0d, d0e, d0f, d0g, d0h };
            discharges[1] = new UIElement[P543.TOTAL_SEGMENT_COUNT] { d1a, d1b, d1c, d1d, d1e, d1f, d1g, d1h };
            discharges[2] = new UIElement[P543.TOTAL_SEGMENT_COUNT] { null, null, null, null, null, null, null, d2h };
            discharges[3] = new UIElement[P543.TOTAL_SEGMENT_COUNT] { d3a, d3b, d3c, d3d, d3e, d3f, d3g, d3h };
            discharges[4] = new UIElement[P543.TOTAL_SEGMENT_COUNT] { d4a, d4b, d4c, d4d, d4e, d4f, d4g, null };
            discharges[5] = new UIElement[P543.TOTAL_SEGMENT_COUNT] { d5a, d5b, d5c, d5d, d5e, d5f, d5g, d5h };

            // отключить все кнопки
            HideAll();
        }

        private void InitDischarges()
        {

        }

        private void ReadingQueueDataPool(object _)
        {
            while (true)
            {
                while (renderDelayTime == -1)
                {
                    Thread.Sleep(1);
                }
                byte[] disData = P543.GetAndReset();
                Dispatcher.Invoke(() =>
                {
                    Print(disData);
                });
                Thread.Sleep(renderDelayTime);
            }
        }

        private void Print(byte[] disData)
        {
            // каждый разряд
            for (int i = 0; i < P543.TOTAL_DISCHARGE_COUNT; i++)
            {
                // каждый сегмент
                for (int j = discharges[i].Length - 1; j >= 0; j--)
                {
                    if (discharges[i][j] == null)
                        continue;

                    if (P543.IsBitSet(disData[i], j))
                        discharges[i][j].Show();
                    else
                        discharges[i][j].HideSmooth();
                }
            }
        }

        private void HideAll()
        {
            for (int i = 0; i < P543.TOTAL_DISCHARGE_COUNT; i++)
            {
                for (int j = 0; j < discharges[i].Length; j++)
                {
                    if (discharges[i][j] != null)
                        discharges[i][j].Hide();
                }
            }
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

        private void SetRenderDelay(int value)
        {
            renderDelayMenu.Header = $"Render delay ({value})";
            renderDelayTime = value;
            ConfigData.ClockDisplayDelay = value;
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

        private void OnShowBuffer(object sender, RoutedEventArgs e)
        {
            Compot.ShowBuffer();
        }

        private void OnAboutClick(object sender, RoutedEventArgs e)
        {
            new MessageWindow(
                "About",
                $"{Title}\nBy EFLFE\nGitHub: https://github.com/EFLFE/ClockDisp ")
                .ShowDialog();
        }
    }
}
