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
        public static uint FontSize;
        public static RenderTexture GameScene;
        public static Font Font;
        public static List<ConvexShape> Shades;
        public static List<VertexArray> Echoes = new List<VertexArray>();

        public static void Init()
        {
            Font = new Font(Program.ResourceDir + Const.FontFile);
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
            DrawShots();
            CastShadows();
            GameScene.Draw(new Sprite(Game.Foreground.Texture));

            GameScene.Display();
            Window.Draw(new Sprite(GameScene.Texture));

            DrawScores();
            
            foreach (var popup in Popup.ActivePopups) {
                popup.Display();
                break;
            }

            Window.Display();
        }

        public static void CastShadows()
        {
            foreach (ConvexShape shade in Shades)
            {
                shade.FillColor = new Color(0, 0, 0);
                GameScene.Draw(shade);
            }
        }

        public static void DrawShots()
        {
            foreach (var line in Echoes) {
                GameScene.Draw(line);
                Vertex v = line[0];
                Color color = v.Color;
                color.A -= (byte)16;
                v.Color = color;
                line[0] = v;
                line[1] = v;
            }
            
            Echoes.RemoveAll((line) => line[0].Color.A < (byte)16);
        }

        public static void DrawScores() {
            Color bg = new Color(0, 0, 0, 100);
            Color fg = new Color(255, 255, 255);
            var rect = new IntRect(
                0, 0,
                width: (int)Screen.Width, height: (int)FontSize + 2
            );
            Drawing.DrawText(
                Window, "# Scoreboard", rect,
                fg, bg, FontSize, TextAlignment.Left
            );

            foreach (var player in Game.Players)
            {
                if (player == null)
                    continue;
                
                rect.Top += rect.Height;
                fg.B = (player.ID == Game.Player.ID) ? (byte)0 : (byte)255;
                string playerName = (player.Name == null) ? $"Player {player.ID}" : player.Name;
                
                Drawing.DrawText(
                    Window, $"{player.Score} | {playerName}", rect,
                    fg, bg, FontSize, TextAlignment.Left
                );
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
            FontSize = Const.FontSize*Height/600;        
            GameScene = new RenderTexture(Width, Height);
            
            Program.PauseMenu = new Menu(
                "Menu",
                "Resume", "Toggle fullscreen", "Quit"
            );
            Program.AskQuit = new Popup(
                new string[] {
                    "Do you really want to quit?"
                }, "Yes", "No"
            );
            Program.Disconnected = new Popup(
                new string[] {
                    "You got disconnected from server."
                }, "Okay... :("
            );

            if (Game.Level != null) {
                Game.LoadLevel();
            }
        }
        private static void OnClose(object sender, EventArgs e)
        {
            Program.AskQuit.Pop();
        }
    }
}
