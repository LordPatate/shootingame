using System;
using System.Collections.Generic;
using SFML.Window;
using SFML.Graphics;
using SFML.System;

namespace shootingame
{
    public class Screen
    {
        private static bool fullscreen = false;
        public static RenderWindow Window;
        public static uint Width, Height;
        public static RenderTexture GameScene;
        public static Font Font;
        public static List<ConvexShape> Shades;

        public static void Init()
        {
            NewWindow();

            GameScene = new RenderTexture(Width, Height);

            Font = new Font(Const.FontFile);
        }
        public static void ToggleFullscreen()
        {
            fullscreen = !fullscreen;
            NewWindow();            
        }

        public static void Quit()
        {
            Window.Close();
        }

        public static void Update()
        {
            GameScene.Draw(new Sprite(Game.Background.Texture));
            foreach (var lightPlayer in Game.Players) {
                if (lightPlayer != null) {
                    Player player = new Player(lightPlayer);
                    player.Texture = Game.Player.Texture;
                    player.Draw();
                }
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

        private static void NewWindow() {
            VideoMode videoMode;
            if (fullscreen) {
                videoMode = VideoMode.FullscreenModes[0];
                Window = new RenderWindow(
                    videoMode,
                    "Shootingame",
                    Styles.Fullscreen
                );
            } else {
                videoMode = new VideoMode(Const.WindowWidth, Const.WindowHeight);
                Window = new RenderWindow(
                    videoMode,
                    "Shootingame",
                    Styles.Default
                );
            }
            Window.Closed += new EventHandler(OnClose);            
            Width = videoMode.Width; Height = videoMode.Height;            
            GameScene = new RenderTexture(Width, Height);

            if (Game.Level != null) {
                LevelInfos infos = Level.levelInfos[Game.LevelID];
                Game.LoadLevel(infos);
            }
        }
        private static void OnClose(object sender, EventArgs e)
        {
            if (Program.AskQuit.Pop() == "Yes")
                Game.Running = false;
        }
    }
}