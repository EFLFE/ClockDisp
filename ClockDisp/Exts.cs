using System;
using System.Windows;
using ClockDisp.P543Data;

namespace ClockDisp
{
    internal static class Exts
    {
        private const double FADE_DOWN = 1.0 / P543.TOTAL_SEGMENT_COUNT;

        public static void ToggleOpacity(this UIElement uIElement)
        {
            uIElement.Opacity = uIElement.Opacity >= 2.0 ? 0.0 : 2.0;
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
            if (uIElement.Opacity < 2.0)
                uIElement.Opacity = 2.0;
        }

    }
}
