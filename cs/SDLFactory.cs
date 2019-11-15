using SDL2;

namespace shootingame
{
    class SDLFactory
    {
        public static SDL.SDL_Rect MakeRect(int x = 0, int y = 0, int w = 0, int h = 0)
        {
            var rect = new SDL.SDL_Rect();
            rect.x = x; rect.y = y; rect.w = w; rect.h = h;
            return rect;
        }
        
        public static SDL.SDL_Point MakePoint(int x = 0, int y = 0)
        {
            var point = new SDL.SDL_Point();
            point.x = x; point.y = y;
            return point;
        }
    }
}