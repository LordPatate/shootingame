using System.Net;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Sockets;
using System;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace shootingame
{
    class Game
    {
        public static bool Running;
        public static Player Player;
        public static RenderTexture Background;
        public static Level Level;

        public static void Init()
        {
            Running = true;
            Player = new Player();
            Level = new Level();

            LoadLevel(0);
        }

        public static void Quit()
        {
            Player.Destroy();
        }

        public static void Update()
        {
            Player.Update(Level);

            using UdpClient client = new UdpClient("localhost", 4242);
            GameState state = new GameState();
            Array.Resize(ref state.PlayersPos, 1);
            state.PlayersPos[0] = new GameState.Point() {X = Player.Rect.Left, Y = Player.Rect.Top};
            MemoryStream stream = new MemoryStream();
            BinaryFormatter serializer = new BinaryFormatter();
            serializer.Serialize(stream, state);
            stream.Seek(0, SeekOrigin.Begin);
            byte[] data = new byte[stream.Length];
            int nb = stream.Read(data, 0, data.Length);
            stream.Close();
            client.Send(data, nb);
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