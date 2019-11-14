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
            Game.Init();

            while (Game.Running)
            {
                Screen.Update();
                Game.Update();

                Thread.Sleep(Const.GameStepDuration);
            }

            Screen.Quit();
            Game.Quit();
            SDL.SDL_Quit();
            SDL_ttf.TTF_Quit();
            SDL_image.IMG_Quit();
        }
    }
}
