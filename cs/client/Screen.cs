using System;
using System.Collections.Generic;
using SFML.Window;
using SFML.Graphics;
using SFML.System;

namespace shootingame
{
    public class Screen
    {
        public static RenderWindow Window;
        public static uint Width, Height;
        public static RenderTexture GameScene;
        public static Font Font;
        public static List<ConvexShape> Shades;

        
        public static void Init()
        {
            VideoMode videoMode = VideoMode.FullscreenModes[0];
            Window = new RenderWindow(
                videoMode,
                "Shootingame",
                Styles.Default//Fullscreen
            );
            Window.Closed += new EventHandler(OnClose);
            Width = videoMode.Width; Height = videoMode.Height;

            GameScene = new RenderTexture(Width, Height);

            Font = new Font(Const.FontFile);
        }

        private static void OnClose(object sender, EventArgs e)
        {
            if (Program.AskQuit.Pop() == "Yes")
                Game.Running = false;
        }
        public static void Quit()
        {
            Window.Close();
        }

        public static void Update()
        {
            GameScene.Draw(new Sprite(Game.Background.Texture));
            foreach (var lightPlayer in Game.Players) {
                Player player = new Player(lightPlayer);
                player.Texture = Game.Player.Texture;
                player.Draw();
            }
            CastShadows();

            GameScene.Display();
            Window.Draw(new Sprite(GameScene.Texture));

            Window.Display();
        }

        public static void CastShadows()
        {
            foreach (ConvexShape shade in Shades)
            {
                shade.FillColor = new Color(0, 0, 0);
                shade.Position = Geometry.AdaptPoint(shade.Position);
                GameScene.Draw(shade);
            }
        }
    }
}