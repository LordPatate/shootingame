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
            
            Screen.Init();

            Thread.Sleep(1000);            

            Screen.Quit();
            SDL.SDL_Quit();
            SDL_ttf.TTF_Quit();
            SDL_image.IMG_Quit();
        }
    }
}
