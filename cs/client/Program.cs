using System;
using System.Threading;
using System.Threading.Tasks;
using SFML.Window;
using SFML.Graphics;
using static SFML.Window.Keyboard;

namespace shootingame
{
    class Program
    {
        public static Popup AskQuit;
        public static Menu PauseMenu;   
        static void Main(string[] args)
        {
            string server = "127.0.0.1";
            if (args.Length >= 1)
                server = args[0];
            Screen.Init();
            Game.Init(server);

            Shadows.Compute();
            while (Game.Running)
            {
                Screen.Update();
                Screen.Window.DispatchEvents();
                
                if (IsKeyPressed(Key.Escape)) {
                    string menuChoice = PauseMenu.Pop();
                    while (menuChoice != "Resume" && menuChoice != "right click")
                    {
                        if (menuChoice == "Quit" && AskQuit.Pop() == "Yes") {
                            Game.Running = false;
                            break;
                        }
                        if (menuChoice == "Toggle fullscreen") {
                            Screen.ToggleFullscreen();
                        }
                        menuChoice = PauseMenu.Pop();
                    }
                }
                
                Task update = Task.Run(() => {
                    Game.Update();
                    Shadows.Compute();
                });

                Thread.Sleep(Const.GameStepDuration);
                update.Wait();
            }

            Game.Quit();
            Screen.Quit();
        }
        
    }
}
