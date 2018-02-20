using System;

namespace ClockDisp.P543Data
{
    // психанул
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
            // когда мы ждём два байта как данные, и ещё два, как разделитель
            for (int i = 0; i < Capacity - 1; i++)
            {
                Data[i] = Data[i + 1];
            }
            Data[Capacity - 1] = value;

            // 13 10 (\n\r)
            if (value == 13)
            {
                newLineCheck = true;
            }
            else if (value == 10 && newLineCheck)
            {
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
}
