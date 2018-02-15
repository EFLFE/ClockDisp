﻿using static ClockDisp.P543Data.ColorIndexset;

namespace ClockDisp.P543Data
{
    internal sealed class Cell
    {
        private readonly sbyte[] data; // -1 = отсуствует

        public Cell(ColorIndexset containsIndexset)
        {
            data = new sbyte[9] { -1, -1, -1, -1, -1, -1, -1, -1, -1 };

            data[0] = (sbyte)(containsIndexset.HaveFlag(Yellow) ? 0 : 2);
            data[1] = (sbyte)(containsIndexset.HaveFlag(Gray) ? 0 : 2);
            data[2] = (sbyte)(containsIndexset.HaveFlag(Green) ? 0 : 2);
            data[3] = (sbyte)(containsIndexset.HaveFlag(Cyan) ? 0 : 2);
            data[4] = (sbyte)(containsIndexset.HaveFlag(Blue) ? 0 : 2);
            data[5] = (sbyte)(containsIndexset.HaveFlag(Orange) ? 0 : 2);
            data[6] = (sbyte)(containsIndexset.HaveFlag(Magenta) ? 0 : 2);
            data[7] = (sbyte)(containsIndexset.HaveFlag(White) ? 0 : 2);
            data[8] = (sbyte)(containsIndexset.HaveFlag(Red) ? 0 : 2);
        }

        public bool SendSignal(int indexColor, bool flag)
        {
            if (data[indexColor] == -1)
            {
                return false;
            }
            data[indexColor] = (sbyte)(flag ? 1 : 0);
            return true;
        }

    }
}