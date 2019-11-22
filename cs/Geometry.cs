using SFML.Graphics;
using SFML.System;

namespace shootingame
{
    class Geometry
    {
        public static IntRect ScaleRect(IntRect rect, int wPercent, int hPercent)
        {
            int width = rect.Width * wPercent / 100;
            int height = rect.Height * hPercent / 100;
            return new IntRect(
                left: rect.Left + (rect.Width-width)/2,
                top: rect.Top + (rect.Height-height)/2,
                width: width, height: height
            );
        }
    }
}