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

            DrawScores();

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

        public static void DrawScores() {
            int lineHeight = 20;
            int i = 0;
            foreach (var player in Game.Players)
            {
                var rect = new IntRect(
                    0, top: lineHeight*i,
                    width: (int)Screen.Width, height: lineHeight
                );
                Color fg = (player.ID == Game.Player.ID) ?
                    new Color(255, 255, 0):
                    new Color(255, 255, 255);
                Color bg = new Color(0, 0, 0, 100);
                
                Drawing.DrawText(
                    Window, $"Player {player.ID}: {player.Score}", rect,
                    fg, bg, Const.FontSize, TextAlignment.Left
                );

                ++i;
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