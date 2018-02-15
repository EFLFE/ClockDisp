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
        // конструктор
        public MainWindow()
        {
            InitializeComponent();

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
