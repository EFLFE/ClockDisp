using System;
using System.Windows;
using ClockDisp.P543Data;

namespace ClockDisp
{
    internal static class Exts
    {
        private const double FADE_DOWN = 1.0 / P543.TOTAL_SEGMENT_COUNT;

        public static bool IsBitSet(byte value, int bitindex)
        {
            return (value & (1 << bitindex)) != 0;
        }

        public static void ToggleOpacity(this UIElement uIElement)
        {
            uIElement.Opacity = uIElement.Opacity >= 1.0 ? 0.0 : 1.0;
        }

        public static void Hide(this UIElement uIElement)
        {
            // скрыт, но мы всё ещё можем по нему кликать
            if (uIElement.Opacity > 0.0)
                uIElement.Opacity = 0.0;
        }

        public static void HideSmooth(this UIElement uIElement)
        {
            // скрыт, но мы всё ещё можем по нему кликать
            if (uIElement.Opacity > 0.0)
                uIElement.Opacity -= FADE_DOWN;
        }

        public static void Show(this UIElement uIElement)
        {
            if (uIElement.Opacity < 1.0)
                uIElement.Opacity = 1.0;
        }

        // TEMP
        // справо налево
        private static byte[] dbits = new byte[]
        {
            0b0_1110111, // #0
            0b0_0010010, // #1
            0b0_1011101, // #2
            0b0_1011011, // #3
            0b0_0111010, // #4
            0b0_1101011, // #5
            0b0_1101111, // #6
            0b0_1010010, // #7
            0b0_1111111, // #8
            0b0_1111011, // #9
        };

        public static byte DigitToBit(int digit)
        {
            if (digit < 0 || digit > 9)
                return 0;
            return dbits[digit];
        }

    }
}
