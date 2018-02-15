using System;
using static ClockDisp.P543Data.ColorIndexset;

namespace ClockDisp.P543Data
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
            CLR_WHITE_ROAD = 22,
            CLR_RED_ROAD = 24;

        public event Action<int, bool> OnRoadSignal;
        public event Action<int, bool> OnCellSignal;

        private readonly bool[] roads;
        private readonly Cell cell_1, cell_2, cell_3, cell_4, cell_5, cell_6;

        public P543()
        {
            // слева на право
            roads = new bool[ROADS_COUNT + 1];
            cell_1 = new Cell(Yellow | Gray | Green | Cyan | Red | Blue | Magenta | White);
            cell_2 = new Cell(Yellow | Green | Cyan | Blue | White | Red | Magenta);
            cell_3 = new Cell(Yellow | Green | Cyan | Blue | White | Red | Magenta | Orange);
            cell_4 = new Cell(Orange);
            cell_5 = new Cell(Yellow | Green | Cyan | Blue | White | Red | Magenta | Orange);
            cell_6 = new Cell(Yellow | Green | Cyan | Blue | White | Red | Magenta | Orange);
        }

        public void SendSignal(int roadIndex, byte flag) => SendSignal(roadIndex, flag > 0);

        public void SendSignal(int roadIndex, bool flag)
        {
            if (roadIndex > 3 && roadIndex < 25)
                return;

            roads[roadIndex] = flag;
            OnRoadSignal.Invoke(roadIndex, flag);

            // установка цветоного сегмента?
            switch (roadIndex)
            {
                case CLR_YELLOW_ROAD:
                    SetSegment(0, flag);
                    break;

                case CLR_GRAY_ROAD:
                    SetSegment(1, flag);
                    break;

                case CLR_GREEN_ROAD:
                    SetSegment(2, flag);
                    break;

                case CLR_CYAN_ROAD:
                    SetSegment(3, flag);
                    break;

                case CLR_BLUE_ROAD:
                    SetSegment(4, flag);
                    break;

                case CLR_ORANGE_ROAD:
                    SetSegment(5, flag);
                    break;

                case CLR_MAGENTA_ROAD:
                    SetSegment(6, flag);
                    break;

                case CLR_WHITE_ROAD:
                    SetSegment(7, flag);
                    break;

                case CLR_RED_ROAD:
                    SetSegment(8, flag);
                    break;
            }
        }

        private void SetSegment(int clrIndex, bool flag)
        {
            // поиск подключенных сегметов (парные дорожки) и установка свечения в нём цветоного сегмента
            if (roads[4] && roads[7])
            {
                if (cell_1.SendSignal(clrIndex, flag))
                {
                    OnCellSignal(clrIndex, flag);
                }
            }
            if (roads[9] && roads[11])
            {
                if (cell_2.SendSignal(clrIndex, flag))
                {
                    OnCellSignal(clrIndex, flag);
                }
            }
            if (roads[13] && roads[14])
            {
                if (cell_3.SendSignal(clrIndex, flag))
                {
                    OnCellSignal(clrIndex, flag);
                }
            }
            if (roads[15] && roads[16])
            {
                if (cell_4.SendSignal(clrIndex, flag))
                {
                    OnCellSignal(clrIndex, flag);
                }
            }
            if (roads[17] && roads[18])
            {
                if (cell_5.SendSignal(clrIndex, flag))
                {
                    OnCellSignal(clrIndex, flag);
                }
            }
            if (roads[20] && roads[23])
            {
                if (cell_6.SendSignal(clrIndex, flag))
                {
                    OnCellSignal(clrIndex, flag);
                }
            }
        }

    }
}
