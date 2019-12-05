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
        public static Popup Disconnected;
        static void Main(string[] args)
        {
            string server = "127.0.0.1";
            string name = null;
            if (args.Length >= 1)
                server = args[0];
            if (args.Length >= 2)
                name = args[1];
            
            Screen.Init();
            Sounds.Init();
            GameState state = Client.ConnectToServer(server);
            if (state == null) {
                ConnectionError(server);
                Screen.Quit();
                return;
            }
            Game.Init(state, name);

            Shadows.Compute();
            while (Game.Running)
            {
                Sounds.Update();
                Screen.Update();
                Screen.Window.DispatchEvents();

                if (!Client.Connected && !Disconnected.IsActive) {
                    if (Disconnected.GetChoice() != null)
                        Game.Running = false;
                    else
                        Disconnected.Pop();
                }
                
                PauseMenuDispatch();
                AskQuitDispatch();
                if (Popup.ActivePopups.Count == 0) {
                    if (IsKeyPressed(Key.Escape)) {
                        PauseMenu.Pop();
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
            Sounds.Quit();
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
        private static void AskQuitDispatch()
        {
            switch (AskQuit.GetChoice())
            {
                case "Yes":
                    Game.Running = false;
                    break;
                case "No":
                    PauseMenu.Pop();
                    break;
            }
        }
        private static void ConnectionError(string server)
        {
            var error = new Popup(
                new string[] {
                    $"Unable to connect to server {server}.",
                    "Make sure you got the address right and try again later."
                },
                "Okay... =/"
            );
            error.Pop();
            while (error.IsActive) {
                Screen.Window.Clear();
                error.Display();
                Screen.Window.Display();
                Screen.Window.DispatchEvents();
                Thread.Sleep(Const.GameStepDuration);
            }
        }
    }
}
