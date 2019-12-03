﻿using System;
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
    }
}
