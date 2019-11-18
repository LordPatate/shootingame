using System;
using System.Threading;
using System.Threading.Tasks;
using SDL2;

namespace shootingame
{
    class Program
    {
        static void Main(string[] args)
        {
            int err; Errors.msg = "Main";
            err = SDL.SDL_Init(SDL.SDL_INIT_EVERYTHING);
            Errors.Check(err);
            err = SDL_ttf.TTF_Init();
            Errors.Check(err);
            SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_PNG);
            
            Screen.Init();
            Game.Init();

            while (Game.Running)
            {
                Screen.Update();
                SDL.SDL_PumpEvents();
                Task update = Task.Run(() => {
                    Game.Update();
                    Screen.ComputeShadows();
                });

                Thread.Sleep(Const.GameStepDuration);
                update.Wait();
            }

            Screen.Quit();
            Game.Quit();
            SDL.SDL_Quit();
            SDL_ttf.TTF_Quit();
            SDL_image.IMG_Quit();
        }
    }
}
