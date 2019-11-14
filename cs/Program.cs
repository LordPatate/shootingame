using System;
using System.Threading;
using SDL2;

namespace shootingame
{
    class Program
    {
        static void Main(string[] args)
        {
            SDL.SDL_Init(SDL.SDL_INIT_EVERYTHING);
            SDL.SDL_CreateWindow("Shootingame", SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, 1120, 630, SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP);

            Thread.Sleep(1000);            

            SDL.SDL_Quit();
        }
    }
}
