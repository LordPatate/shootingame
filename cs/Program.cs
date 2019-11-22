using System;
using System.Threading;
using System.Threading.Tasks;
using SFML.Window;
using SFML.Graphics;

namespace shootingame
{
    class Program
    {        
        static void Main(string[] args)
        {
            
            Screen.Init();
            Game.Init();

            Screen.ComputeShadows();
            while (Game.Running)
            {
                Screen.Update();
                Task update = Task.Run(() => {
                    Game.Update();
                    Screen.ComputeShadows();
                });

                Thread.Sleep(Const.GameStepDuration);
                update.Wait();
            }

            Game.Quit();
            Screen.Quit();
        }
        
    }
}
