using System;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using static SFML.Window.Keyboard;

namespace shootingame
{
    class Game
    {
        public static bool Running;
        public static Player Player;
        public static RenderTexture Background;
        public static Level Level;
        public static Popup AskQuit;

        public static void Init()
        {
            Running = true;
            Player = new Player();
            Level = new Level();
            AskQuit = new Popup(
                new string[] {
                    "Do you really want to quit?"
                }, "Yes", "No"
            );

            LoadLevel(0);
        }

        public static void Quit()
        {
            Player.Destroy();
            AskQuit.Destroy();
        }

        public static void Update()
        {
            if (IsKeyPressed(Key.Escape))
                if (AskQuit.Pop() == "Yes")
                    Running = false;

            Player.Update(Level);
        }

        public static void LoadLevel(uint id)
        {
            LevelInfos infos = Level.levelInfos[id];
            Level.Init(infos);

            Player.Rect.Left = Level.PlayerStartPos.X;
            Player.Rect.Top = Level.PlayerStartPos.Y;

            Background = new RenderTexture(Screen.Width, Screen.Height);

            DrawBackground(infos.BackgroundImg, infos.ForegroundImg);
        }

        private static void DrawBackground(string bg, string fg)
        {
            Texture foreground = GetTexture(fg, new Color(0,0,0));//new Color(20, 17, 23));
            Texture background = GetTexture(bg, new Color(65, 60, 55));

            if (bg == "") {
                var rect = new IntRect(0, 0, width: (int)Screen.Width, height: (int)Screen.Height);
                Background.Draw(Drawing.SpriteOf(foreground, rect));
                Background.Draw(Drawing.SpriteOf(background, Level.Bounds));
            }

            foreach (var tile in Game.Level.Tiles) {
                Background.Draw(Drawing.SpriteOf(foreground, tile.Rect));
            }
            Background.Display();
        }
        private static Texture GetTexture(string src, Color defaultColor)
        {
            Texture texture;
            if (src == "")
            {
                RenderTexture render = new RenderTexture(Const.TileWidth, Const.TileHeight);

                render.Clear(defaultColor);
                texture = render.Texture;
            }
            else
            {
                texture = new Texture(src);
            }

            return texture;
        }
    }
}