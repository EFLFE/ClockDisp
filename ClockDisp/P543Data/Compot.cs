using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using System.Windows;

namespace ClockDisp.P543Data
{
    // чтение данных с порта
    internal static class Compot
    {
        private const int BUFFER_CAPACITY = 512;

        public static event Action OnPortCreated;
        public static event Action<Exception> OnPortFail;

        //public static readonly Queue<byte[]> OutBuffer = new Queue<byte[]>(BUFFER_CAPACITY);

        public static bool ThreadActive = true;
        private static Thread thread;
        private static SerialPort port;
        private static object[] data;

        public static bool PortIsOpen => port != null && port.IsOpen;

        public static void CreatePort(string portName, int baudrate, Parity parity, int databits,
            StopBits stopBits, Handshake handshake, int readTimeout, int writeTimeput)
        {
            if (thread == null)
            {
                thread = new Thread(ThreadMethod);
                thread.Start();
            }

            // пожалуй да, это реализовано по дебильному
            data = new object[]
            {
                portName, baudrate, parity, databits, stopBits, handshake, readTimeout, writeTimeput
            };
        }

        private static void ThreadMethod()
        {
            byte[] buffer = new byte[4];

            while (ThreadActive)
            {
                Thread.Sleep(5);

                if (data != null)
                {
                    // create port
                    try
                    {
                        FreePort();
                        port = new SerialPort((string)data[0], (int)data[1], (Parity)data[2], (int)data[3], (StopBits)data[4])
                        {
                            Handshake = (Handshake)data[5],
                            ReadTimeout = (int)data[6],
                            WriteTimeout = (int)data[7],

                            // https://www.arduino.cc/en/Serial/Println
                            // return character (ASCII 13, or '\r') and a newline character (ASCII 10, or '\n')
                            NewLine = "\r\n",
                        };
                        OpenPort();
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
                if (port != null && port.IsOpen)
                {
                    /* read data
                     * ждём два байта и перенос строки как разделитель (и того 4 байта)
                    */

                    if (port.BytesToRead > 1024)
                    {
                        ClosePort();
                        Application.Current.Dispatcher.Invoke(() => new MessageWindow(
                            "Error",
                            $"Compot: буффер переполнен. Программа не успевает их обработать. Порт закрыт.").ShowDialog());
                    }
                    while (port.BytesToRead > 3)
                    {
                        port.Read(buffer, 0, buffer.Length);

                        // ckeck new line
                        if (buffer[2] != '\r' || buffer[3] != '\n')
                        {
                            ClosePort();
                            Application.Current.Dispatcher.Invoke(() => new MessageWindow(
                                "Error",
                                $"Compot: неверный формат данных. Порт закрыт.").ShowDialog());
                        }

                        P543.ParseSignal(buffer[0], buffer[1]);
                    }
                }
            }
        }

        public static void ClearBuffer()
        {
            if (port != null && !port.IsOpen)
            {
                port.ReadExisting();
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
