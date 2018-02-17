using System;

namespace ClockDisp.P543Data
{
    internal sealed class P543
    {
        public const int TOTAL_SEGMENT_COUNT = 6;

        private readonly int[] roadPairs;

        public P543()
        {
            // слева на право
            roadPairs = new int[] { 4, 7, 9, 11, 13, 14, 15, 16, 17, 18, 20, 23 };
        }
    }
}
