using System;
using System.Threading;
using System.Threading.Tasks;
using SFML.Window;
using SFML.Graphics;

namespace shootingame
{
    class Program
    {
        /*
        static void Main(string[] args)
        {

            // Load a sprite to display
            Texture texture = new Texture(Const.PlayerSpriteSheet);
            Sprite sprite = new Sprite(texture);

            // Create a graphical string to display
            Font font = new Font(Const.FontFile);
            Text text = new Text("Hello SFML.Net", font);

            // Start the game loop
            while (window.IsOpen)
            {
                // Process events
                window.DispatchEvents();

                // Clear screen
                window.Clear();

                // Draw the sprite
                window.Draw(sprite);

                // Draw the string
                window.Draw(text);

                // Update the window
                window.Display();
            }
        }
        */
        
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

            Screen.Quit();
            Game.Quit();
        }
        
    }
}
