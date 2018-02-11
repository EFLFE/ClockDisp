using System;

namespace ClockDisp
{
    // бинарные данные для ячеек одной сетки символа
    internal sealed class LedData
    {
        /* Marker:
         * -A-
         * B-C
         * -D-
         * E-F
         * -G-
         */

        public readonly bool A;
        public readonly bool B;
        public readonly bool C;
        public readonly bool D;
        public readonly bool E;
        public readonly bool F;
        public readonly bool G;

        public LedData(bool a, bool b, bool c, bool d, bool e, bool f, bool g)
        {
            A = a;
            B = b;
            C = c;
            D = d;
            E = e;
            F = f;
            G = g;
        }

        //                        0 23 5 78 10`
        /// <summary> sample: 1: '0-01-0-01-0' </summary>
        public LedData(string data)
        {
            if (data.Length != "0-01-0-01-0".Length)
                throw new Exception("LedData bad format. Example: '0-01-0-01-0'");

            A = data[0] == '1';
            B = data[2] == '1';
            C = data[3] == '1';
            D = data[5] == '1';
            E = data[7] == '1';
            F = data[8] == '1';
            G = data[10] == '1';
        }
    }
}
