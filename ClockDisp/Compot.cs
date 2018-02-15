using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using System.Windows;

namespace ClockDisp
{
    internal static class Compot
    {
        public static event Action OnPortCreated;
        public static event Action<Exception> OnPortFail;

        public static readonly Queue<string> OutBuffer = new Queue<string>();

        public static bool ThreadActive = true;
        private static Thread thread;
        private static SerialPort port;
        private static object[] data;

        public static void CreatePort(string portName, int baudrate, Parity parity, int databits,
            StopBits stopBits, Handshake handshake, int readTimeout, int writeTimeput)
        {
            if (thread == null)
            {
                thread = new Thread(ThreadMethod);
                thread.Start();
            }

            data = new object[]
            {
                portName, baudrate, parity, databits, stopBits, handshake, readTimeout, writeTimeput
            };
        }

        private static void ThreadMethod()
        {
            while (ThreadActive)
            {
                Thread.Sleep(250);

                if (data != null)
                {
                    try
                    {
                        FreePort();
                        port = new SerialPort((string)data[0], (int)data[1], (Parity)data[2], (int)data[3], (StopBits)data[4])
                        {
                            Handshake = (Handshake)data[5],
                            ReadTimeout = (int)data[6],
                            WriteTimeout = (int)data[7]
                        };
                        OpenPort();
                        port.DataReceived += Port_DataReceived;
                        OnPortCreated();
                    }
                    catch (Exception ex)
                    {
                        FreePort();
                        OnPortFail(ex);
                    }
                    finally
                    {
                        data = null;
                    }
                }
            }
        }

        private static void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = port.ReadExisting();
            if (data != null && data.Length > 0)
            {
                OutBuffer.Enqueue(data);
            }
        }

        public static void OpenPort()
        {
            if (port != null && !port.IsOpen)
            {
                try
                {
                    port.Open();
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(() => new MessageWindow(ex.Message, ex.ToString()).ShowDialog());
                }
            }
        }

        public static void ClosePort()
        {
            if (port != null && port.IsOpen)
            {
                port.Close();
            }
        }

        private static void FreePort()
        {
            if (port != null)
            {
                if (port.IsOpen)
                {
                    port.Close();
                }
                port.Dispose();
                port = null;
            }
        }

    }
}
