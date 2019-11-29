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
        public static List<VertexArray> Echoes = new List<VertexArray>();

        public static void Init()
        {
            Font = new Font(Const.FontFile);
            NewWindow();
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
            foreach (var line in Echoes) {
                GameScene.Draw(line);
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
            if (Window != null) {
                Window.Close();
            }
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
            
            Program.PauseMenu = new Menu(
                "Game paused",
                "Resume", "Toggle fullscreen", "Quit"
            );
            Program.AskQuit = new Popup(
                new string[] {
                    "Do you really want to quit?"
                }, "Yes", "No"
            );

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