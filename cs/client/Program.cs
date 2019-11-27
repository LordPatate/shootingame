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
        static void Main(string[] args)
        {
            string server = "127.0.0.1";
            if (args.Length >= 1)
                server = args[0];
            Screen.Init();
            Game.Init(server);
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
                
                if (IsKeyPressed(Key.Escape))
                    if (AskQuit.Pop() == "Yes")
                        Game.Running = false;
                
                Task update = Task.Run(() => {
                    Game.Update();
                    Shadows.Compute();
                });

                Thread.Sleep(Const.GameStepDuration);
                update.Wait();
            }

            AskQuit.Destroy();
            Game.Quit();
            Screen.Quit();
        }
        
    }
}
