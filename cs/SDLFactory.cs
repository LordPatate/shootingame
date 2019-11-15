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
        
        public static SDL.SDL_FPoint MakeFPoint(int x = 0, int y = 0) {
            var point = new SDL.SDL_FPoint();
            point.x = (float)x; point.y = (float)y;
            return point;
        }

        public static SDL.SDL_Color MakeColor(byte r = 0, byte g = 0, byte b = 0, byte a = 255) {
            var color = new SDL.SDL_Color();
            color.r = r; color.g = g; color.b = b; color.a = a;
            return color;
        }
    }
}