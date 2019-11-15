using SDL2;

namespace shootingame
{
    class SDLFactory
    {
        public static SDL.SDL_Rect MakeRect(int X = 0, int Y = 0, int W = 0, int H = 0)
        {
            var rect = new SDL.SDL_Rect();
            rect.X = X; rect.Y = Y; rect.W = W; rect.H = H;
            return rect;
        }
        
        public static SDL.SDL_Point MakePoint(int X = 0, int Y = 0)
        {
            var point = new SDL.SDL_Point();
            point.X = X; point.Y = Y; point.W = W; point.H = H;
            return point;
        }
    }
}