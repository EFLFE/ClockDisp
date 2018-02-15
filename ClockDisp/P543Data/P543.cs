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

        // index, flag
        public event Action<int, bool> OnRoadSignal;

        // cellIndex, clrIndex, flag
        public event Action<int, int, bool> OnCellSignal;

        private readonly int[] roadPairs;
        private readonly bool[] roads;
        private readonly Cell[] cells;

        public P543()
        {
            // слева на право
            roadPairs = new int[] { 4, 7, 9, 11, 13, 14, 15, 16, 17, 18, 20, 23 };
            roads = new bool[ROADS_COUNT + 1];
            cells = new Cell[]
            {
                new Cell(Yellow | Gray | Green | Cyan | Red | Blue | Magenta | White),
                new Cell(Yellow | Green | Cyan | Blue | White | Red | Magenta),
                new Cell(Yellow | Green | Cyan | Blue | White | Red | Magenta | Orange),
                new Cell(Orange),
                new Cell(Yellow | Green | Cyan | Blue | White | Red | Magenta | Orange),
                new Cell(Yellow | Green | Cyan | Blue | White | Red | Magenta | Orange),
            };
        }

        public void SendSignal(int roadIndex, byte flag) => SendSignal(roadIndex, flag > 0);

        public void SendSignal(int roadIndex, bool flag)
        {
            if (roadIndex > 3 && roadIndex < 25)
                return;

            roads[roadIndex] = flag;
            OnRoadSignal(roadIndex, flag);

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
            for (int i = 0, j = 0; i < roadPairs.Length; i += 2, j++)
            {
                if (roads[roadPairs[i]] && roads[roadPairs[i + 1]])
                {
                    if (cells[j].SendSignal(clrIndex, flag))
                    {
                        OnCellSignal(j, clrIndex, flag);
                    }
                }
            }
        }

    }
}
