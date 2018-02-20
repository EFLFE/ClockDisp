using System;

namespace ClockDisp.P543Data
{
    internal static class P543
    {
        public const int ROAD_COUNT = 27;
        public const int TOTAL_SEGMENT_COUNT = 8;
        public const int TOTAL_DISCHARGE_COUNT = 6;

        private static int timer;
        private static Random rnd = new Random();

        // TODO: маска чисел

        private static readonly bool[] roadPower = new bool[ROAD_COUNT];
        private static byte discharge0, discharge1, discharge2, discharge3, discharge4, discharge5;

        public static bool IsBitSet(byte value, int bitindex)
        {
            return (value & (1 << bitindex)) != 0;
        }

        public static void ParseSignal(byte segmentsValue, byte dischargeIndex)
        {
            // тут очень важна производительность, ибо метод будет принимать около 1000 вызовов в сек
            switch (dischargeIndex)
            {
                case 0:
                    discharge0 = segmentsValue;
                    break;
                case 1:
                    discharge1 = segmentsValue;
                    break;
                case 2:
                    discharge2 = segmentsValue;
                    break;
                case 3:
                    discharge3 = segmentsValue;
                    break;
                case 4:
                    discharge4 = segmentsValue;
                    break;
                case 5:
                    discharge5 = segmentsValue;
                    break;
            }
        }

        public static byte[] GetAndReset()
        {
            // в аналоге память в регистрах перезаписыватся.
            // тут будет симуляция динамической индексации:
            // сбрасываем данные разрядов после отрисовки

            if (Compot.PortIsOpen)
            {
                byte[] disArray = new byte[TOTAL_DISCHARGE_COUNT]
                {
                    discharge0,
                    discharge1,
                    discharge2,
                    discharge3,
                    discharge4,
                    discharge5,
                };
                discharge0 = 0;
                discharge1 = 0;
                discharge2 = 0;
                discharge3 = 0;
                discharge4 = 0;
                discharge5 = 0;
                return disArray;
            }
            else
            {
                // demo (visual testing)
                timer++;
                if (timer == 10)
                {
                    timer = 0;
                    var by = new byte[TOTAL_DISCHARGE_COUNT];
                    rnd.NextBytes(by);
                    return by;
                }
                else
                {
                    return new byte[TOTAL_DISCHARGE_COUNT]
                    {
                        0b0_0000000,
                        0b0_0000000,
                        0b0_0000000,
                        0b0_0000000,
                        0b0_0000000,
                        0b0_0000000,
                    };
                }
            }
        }

    }
}
