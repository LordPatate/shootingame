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
        static void Main(string[] args)
        {
            string server = "127.0.0.1";
            if (args.Length >= 1)
                server = args[0];
            Screen.Init();
            Game.Init(server);
            Menu menu = new Menu(
                "Game paused",
                "Resume", "Toggle fullscreen", "Quit"
            );
            AskQuit = new Popup(
                new string[] {
                    "Do you really want to quit?"
                }, "Yes", "No"
            );

            Shadows.Compute();
            while (Game.Running)
            {
                Screen.Update();
                Screen.Window.DispatchEvents();
                
                if (IsKeyPressed(Key.Escape)) {
                    string menuChoice = menu.Pop();
                    while (menuChoice != "Resume")
                    {
                        if (menuChoice == "Quit" && AskQuit.Pop() == "Yes") {
                            Game.Running = false;
                            break;
                        }
                        if (menuChoice == "Toogle fullscreen") {
                            Screen.ToggleFullscreen();
                        }
                        menuChoice = menu.Pop();
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
