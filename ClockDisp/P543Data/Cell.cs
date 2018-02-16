using static ClockDisp.P543Data.ColorIndexset;

namespace ClockDisp.P543Data
{
    internal sealed class Cell
    {
        // -1 = отсуствует; 0 = нет сингала; 1 = есть сигнал
        private readonly sbyte[] data;

        public Cell(ColorIndexset containsIndexset)
        {
            data = new sbyte[9] { -1, -1, -1, -1, -1, -1, -1, -1, -1 };

            data[0] = (sbyte)(containsIndexset.HaveFlag(Yellow) ? 0 : 1);
            data[1] = (sbyte)(containsIndexset.HaveFlag(Gray) ? 0 : 1);
            data[2] = (sbyte)(containsIndexset.HaveFlag(Green) ? 0 : 1);
            data[3] = (sbyte)(containsIndexset.HaveFlag(Cyan) ? 0 : 1);
            data[4] = (sbyte)(containsIndexset.HaveFlag(Blue) ? 0 : 1);
            data[5] = (sbyte)(containsIndexset.HaveFlag(Orange) ? 0 : 1);
            data[6] = (sbyte)(containsIndexset.HaveFlag(Magenta) ? 0 : 1);
            data[7] = (sbyte)(containsIndexset.HaveFlag(White) ? 0 : 1);
            data[8] = (sbyte)(containsIndexset.HaveFlag(Red) ? 0 : 1);
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
