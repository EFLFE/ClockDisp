using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using System.Windows;

namespace ClockDisp.P543Data
{
    internal struct StaticQueue
    {
        public readonly byte[] Data;
        public readonly int Capacity;

        private bool newLineCheck;

        public StaticQueue(int capacity)
        {
            Data = new byte[capacity];
            Capacity = capacity;
            newLineCheck = false;
        }

        public bool Pulse(byte value)
        {
            for (int i = 0; i < Capacity - 1; i++)
            {
                Data[i] = Data[i + 1];
            }
            Data[Capacity - 1] = value;

            // 13 10
            if (value == 13)
            {
                newLineCheck = true;
            }
            else if (newLineCheck && value == 10)
            {
                // TRIGGERET!
                newLineCheck = false;
                return true;
            }
            else
            {
                newLineCheck = false;
            }
            return false;
        }

    }

    // чтение данных с порта
    internal static class Compot
    {
        private const int BUFFER_CAPACITY = 512;

        public static event Action OnPortCreated;
        public static event Action<Exception> OnPortFail;

        private static StaticQueue staticQueue = new StaticQueue(4);

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

            // пожалуй да, это реализовано по тупому
            data = new object[]
            {
                portName, baudrate, parity, databits, stopBits, handshake, readTimeout, writeTimeput
            };
        }

        private static void ThreadMethod()
        {
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
                        continue;
                    }
                    GetData();
                }
            }
        }

        private static void GetData()
        {
            if (port.BytesToRead == 0)
                return;

            byte[] buffer = new byte[port.BytesToRead];

            port.Read(buffer, 0, buffer.Length);

            for (int i = 0; i < buffer.Length; i++)
            {
                if (staticQueue.Pulse(buffer[i]))
                {
                    P543.ParseSignal(staticQueue.Data[0], staticQueue.Data[1]);
                }
            }

            /* ckeck new line
            if (buffer[2] != '\r' || buffer[3] != '\n')
            {
                ClosePort();
                Application.Current.Dispatcher.Invoke(() => new MessageWindow(
                    "Error",
                    "Compot: неверный формат данных. Порт закрыт.\n\r\n\rБуффер:\n\r" +
                    $"{buffer[0]} ({(char)buffer[0]})\n" +
                    $"{buffer[1]} ({(char)buffer[1]})\n" +
                    $"{buffer[2]} ({(char)buffer[2]})\n" +
                    $"{buffer[3]} ({(char)buffer[3]})\n"
                    ).ShowDialog());
                break;
            }
            */
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
                    // clear buffer?
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
                ClearBuffer();
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
