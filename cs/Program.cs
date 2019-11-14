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
            SDL_ttf.TTF_Init();
            SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_PNG);
            
            SDL.SDL_CreateWindow("Shootingame", SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED,
            Const.WindowWidth, Const.WindowHeight,
            SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP);

            Thread.Sleep(1000);            

            SDL.SDL_Quit();
            SDL_ttf.TTF_Quit();
            SDL_image.IMG_Quit();
        }
    }
}
