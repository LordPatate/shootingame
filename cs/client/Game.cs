using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace shootingame
{
    public class Game
    {
        public static bool Running;
        public static Player Player;
        public static List<LightPlayer> Players = new List<LightPlayer>();
        public static int LevelID;
        public static Level Level;
        public static RenderTexture Background;
        public static IntRect Bounds;

        public static void Init(string server)
        {
            Running = true;
            
            GameState state = Client.ConnectToServer(server);
            Player = new Player(state.PlayerID);
            
            LevelID = 0;
            LevelInfos infos = Level.levelInfos[LevelID];
            Level = new Level(infos);
            LoadLevel(infos);
            
            UpdateFromGameState(state);
            Player.FromLightPlayer(state.Players[(int)state.PlayerID]);
        }

        public static void Quit()
        {
            Player.Destroy();
            if (Client.Connected)
                Client.SendDisconnect();
            else
                Client.Disconnect();
        }

        public static void Update()
        {
            if (!Client.Connected) {
                Running = false;
                return;
            }
            Player.Update(Level);

            Client.SendUpdate();
            GameState state = Client.ReceiveUpdate();
            if (state != null) {
                UpdateFromGameState(state);
            }
        }
        private static void UpdateFromGameState(GameState state)
        {
            Players.Clear();
            Players.AddRange(state.Players);
        }

        public static void LoadLevel(LevelInfos infos)
        {
            Bounds = new IntRect(
                left: (int)Screen.Width/2 - Level.Bounds.Width/2,
                top: (int)Screen.Height/2 - Level.Bounds.Height/2,
                width: Level.Bounds.Width,
                height: Level.Bounds.Height
            );

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
                Background.Draw(Drawing.SpriteOf(background, Bounds));
            }

            foreach (var tile in Game.Level.Tiles) {
                var rect = Geometry.AdaptRect(tile.Rect);
                Background.Draw(Drawing.SpriteOf(foreground, rect));
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