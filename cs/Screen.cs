using System;
using System.Collections.Generic;
using SFML.Window;
using SFML.Graphics;

namespace shootingame
{
    class Screen
    {
        public struct ShadePolygon {int[] vx, vy;}
        public static RenderWindow Window;
        public static uint Width, Height;
        public static RectangleShape GameScene;
        public static Font Font;
        public static List<ShadePolygon> Shades;

        
        public static void Init()
        {
            VideoMode videoMode = VideoMode.FullscreenModes[0];
            Window = new RenderWindow(
                videoMode,
                "Shootingame",
                Styles.Fullscreen
            );
            Window.Closed += new EventHandler(OnClose);
            Width = videoMode.Width; Height = videoMode.Height;

            GameScene = new RectangleShape(new Vector2f((float)Width, (float)Height));

            Font = new Font(Const.FontFile);
        }

        private static void OnClose(object sender, EventArgs e)
        {
            if (Game.AskQuit.Pop() == "Yes")
                Game.Running = false;
        }
        public static void Quit()
        {
            Window.Close();
        }

        public static void Update()
        {
            Window.Draw(GameScene);
            Game.Player.Draw();
            CastShadows();

            Window.Display();
        }

        public static void ComputeShadows()
        {
            Shades = new List<ShadePolygon>();
        }

        public static void CastShadows()
        {
            foreach (ShadePolygon shade in Shades)
            {
                //SDL_gfx.SDL_FilledPolygonRGBA
            }
        }
    }
}