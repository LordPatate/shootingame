using SDL2;

namespace shootingame
{
    class Geometry
    {
        public static bool PointInRectangle(int x, int y, SDL.SDL_Rect r) {
            return r.x < x && r.x+r.w > x &&
                r.y < y && r.y+r.h > y;
        }

        public static SDL.SDL_Rect ScaleRect(SDL.SDL_Rect rect, int wPercent, int hPercent)
        {
            int width = rect.w * wPercent / 100;
            int height = rect.h * hPercent / 100;
            return SDLFactory.MakeRect(
                x: rect.x + (rect.w-width)/2,
                y: rect.y + (rect.h-height)/2,
                w: width, h: height
            );
        }
    }
}