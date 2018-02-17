using System;
using System.ComponentModel;
using System.IO.Ports;
using System.Windows;
using ClockDisp.P543Data;

namespace ClockDisp
{
    /// <summary>
    /// Interaction logic for ComPortConfig.xaml
    /// </summary>
    public partial class ComPortConfig : Window
    {
        public ComPortConfig()
        {
            InitializeComponent();

            string[] avaports = SerialPort.GetPortNames();
            if (avaports != null && avaports.Length > 0)
            {
                for (int i = 0; i < avaports.Length; i++)
                {
                    portBox.Items.Add(avaports[i]);
                }
            }

            Compot.OnPortCreated += Compot_OnPortCreated;
            Compot.OnPortFail += Compot_OnPortFail;
        }

        private void Compot_OnPortFail(Exception ex)
        {
            Dispatcher.Invoke(() =>
            {
                new MessageWindow(ex.Message, ex.ToString()).ShowDialog();
                window.IsEnabled = true;
            });
        }

        private void Compot_OnPortCreated()
        {
            Dispatcher.Invoke(() =>
            {
                window.IsEnabled = true;
                Close();
            });
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!window.IsEnabled)
            {
                e.Cancel = true;
            }
            else
            {
                Compot.OnPortCreated -= Compot_OnPortCreated;
                Compot.OnPortFail -= Compot_OnPortFail;
            }

            base.OnClosing(e);
        }

        private void apply_Click(object sender, RoutedEventArgs e)
        {
            if (portBox.Text.Length == 0)
                return;

            window.IsEnabled = false;

            try
            {
                Compot.CreatePort(portBox.Text, 9600, (Parity)parityBox.SelectedIndex, dataBitsBox.SelectedIndex + 5,
                    (StopBits)stopBitsBox.SelectedIndex, Handshake.None, int.Parse(readTimeoutBox.Text), int.Parse(writeTimeoutBox.Text));
            }
            catch (Exception ex)
            {
                new MessageWindow(ex.Message, ex.ToString()).ShowDialog();
                window.IsEnabled = true;
            }
        }
    }
}
