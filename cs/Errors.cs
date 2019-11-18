using System;
using SDL2;

namespace shootingame
{
    class Errors
    {
        public static void Check(int val, string msg = "")
        {
            if (val == 0)
                return;
            
            if (msg == "")
                msg = "Error";
            
            throw new Exception("%s: %s", msg, SDL.SDL_GetError());
        }
    }
}