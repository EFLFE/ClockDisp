using System;

namespace ClockDisp
{
    internal sealed class P543
    {
        public const int
            ROADS_COUNT = 27,
            CLR_YELLOW_ROAD = 5,
            CLR_GRAY_ROAD = 6,
            CLR_GREEN_ROAD = 8,
            CLR_CYAN_ROAD = 10,
            CLR_BLUE_ROAD = 12,
            CLR_ORANGE_ROAD = 19,
            CLR_MAGENTA_ROAD = 21,
            CLR_RED_ROAD = 22,
            CLR_PURPURE_ROAD = 24;

        private readonly bool[] roads;

        public P543()
        {
            roads = new bool[ROADS_COUNT + 1];
        }

        public void SendSignal(int roadIndex, byte flag) => SendSignal(roadIndex, flag > 0);

        public void SendSignal(int roadIndex, bool flag)
        {
            if (roadIndex < 1 || roadIndex > ROADS_COUNT)
                throw new ArgumentOutOfRangeException(nameof(roadIndex));

            roads[roadIndex] = flag;

            switch (roadIndex)
            {
                case CLR_YELLOW_ROAD:
                    break;

                case CLR_GRAY_ROAD:
                    break;

                case CLR_GREEN_ROAD:
                    break;

                case CLR_CYAN_ROAD:
                    break;

                case CLR_BLUE_ROAD:
                    break;

                case CLR_ORANGE_ROAD:
                    break;

                case CLR_MAGENTA_ROAD:
                    break;

                case CLR_RED_ROAD:
                    break;

                case CLR_PURPURE_ROAD:
                    break;

                default:
                    // non color signal
                    break;
            }

        }

    }
}
