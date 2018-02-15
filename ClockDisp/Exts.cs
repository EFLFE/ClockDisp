using System;
using System.Windows;
using ClockDisp.P543Data;

namespace ClockDisp
{
    internal static class Exts
    {
        public static bool HaveFlag(this ColorIndexset a, ColorIndexset b)
        {
            return (a & b) == b;
        }

        public static void ToggleOpacity(this UIElement uIElement)
        {
            uIElement.Opacity = uIElement.Opacity >= 1.0 ? 0.0 : 1.0;
        }

        public static void Hide(this UIElement uIElement)
        {
            // скрыт, но мы всё ещё можем по нему кликать
            uIElement.Opacity = 0.0;
        }

        public static void Show(this UIElement uIElement)
        {
            uIElement.Opacity = 1.0;
        }
    }
}
