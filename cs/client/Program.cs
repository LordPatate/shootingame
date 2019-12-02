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
            string name = null;
            if (args.Length >= 1)
                server = args[0];
            if (args.Length >= 2)
                name = args[1];
            Screen.Init();
            Game.Init(server, name);

            Shadows.Compute();
            while (Game.Running)
            {
                Screen.Update();
                Screen.Window.DispatchEvents();
                
                if (Popup.ActivePopups == 0) {
                    if (IsKeyPressed(Key.Escape)) {
                        PauseMenu.Pop();
                    }
                    PauseMenuDispatch();
                    if (AskQuit.GetChoice() == "Yes") {
                        Game.Running = false;
                    }
                    Controls.Update();
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

        private static void PauseMenuDispatch()
        {
            switch (PauseMenu.GetChoice())
            {
                case "Quit":
                    AskQuit.Pop();
                    break;
                case "Toggle fullscreen":
                    Screen.ToggleFullscreen();
                    break;
            }
        }        
    }
}
