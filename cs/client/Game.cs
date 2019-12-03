using System.Threading;
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
        public static RenderTexture Foreground;
        public static IntRect Bounds;

        public static void Init(string server, string name)
        {
            Running = true;
            
            GameState state = Client.ConnectToServer(server);
            Player = new Player(state.PlayerID, name);
            
            LevelID = state.LevelID;
            LevelInfos infos = Level.levelInfos[LevelID];
            Level = new Level(infos);
            LoadLevel(infos);
            
            UpdateFromGameState(state);
            Player.FromLightPlayer(state.Players[(int)state.PlayerID]);
        }

        public static void Quit()
        {
            if (Client.Connected)
                Client.SendDisconnect();
            else
                Client.Disconnect();
        }

        public static void Update()
        {
            if (!Client.Connected) {
                Running = false;
                var disconnected = new Popup(
                    new string[] {
                        "You got disconnected from server."
                    }, "Okay... :("
                );
                disconnected.Pop();
                while (disconnected.IsActive) {
                    Screen.Update();
                    Screen.Window.DispatchEvents();
                    Thread.Sleep(Const.GameStepDuration);
                }
                return;
            }
            Player.Update(Level);

            GameState state = MakeGameState();
            Client.SendUpdate(state);
            state = Client.ReceiveUpdate();
            if (state != null) {
                UpdateFromGameState(state);
            }
        }
        private static GameState MakeGameState()
        {
            GameState state = new GameState() {
                Type = GameState.RequestType.Update,
                PlayerID = Player.ID,
                Players = new LightPlayer[] { new LightPlayer(Player) },
            };
            
            state.Shots = null;
            if (Player.Shot) {
                state.Shots = new LightShot[] {
                    new LightShot() {
                        Origin = new LightVect2() { X = Player.GetCOM().X, Y = Player.GetCOM().Y },
                        Dest = new LightVect2() { X = Player.ShotPoint.X, Y = Player.ShotPoint.Y },
                        Alpha = 255
                    }
                };
            }

            return state;
        }
        private static void UpdateFromGameState(GameState state)
        {
            Players.Clear();
            Players.AddRange(state.Players);

            LightPlayer lightPlayer = Players[Player.ID];
            
            if (state.LevelID != LevelID) {
                LevelID = state.LevelID;
                var infos = Level.levelInfos[LevelID];
                Level = new Level(infos);
                LoadLevel(infos);
                lightPlayer.ReSpawn = true;
            }
            
            Player.HasRespawned = lightPlayer.HasRespawned;
            if (lightPlayer.ReSpawn) {
                Vector2i spawnPoint = Level.SpawnPoints[Player.ID % Level.SpawnPoints.Count];
                Player.Rect.Left = spawnPoint.X;
                Player.Rect.Top = spawnPoint.Y;
                Player.HasRespawned = true;
            }

            if (state.Shots == null) {
                return;
            }
            Screen.Echoes.Clear();
            for (int i = 0; i < state.Shots.Length; ++i) {
                LightShot shot = state.Shots[i];
                Color color = new Color(255, 220, 200, alpha: shot.Alpha);
                VertexArray line = new VertexArray(PrimitiveType.Lines);                
                line.Append(new Vertex(
                    Geometry.AdaptPoint(new Vector2f(shot.Origin.X, shot.Origin.Y)),
                    color
                ));
                line.Append(new Vertex(
                    Geometry.AdaptPoint(new Vector2f(shot.Dest.X, shot.Dest.Y)),
                    color
                ));
                
                Screen.Echoes.Add(line);
            }
        }

        public static void LoadLevel(LevelInfos infos)
        {
            Bounds = Geometry.ScaleRect(new IntRect(0, 0, (int)Screen.Width, (int)Screen.Height), 80, 80);

            DrawScene(infos.BackgroundImg, infos.ForegroundImg);
        }

        private static void DrawScene(string bg, string fg)
        {
            Texture foreground = GetTexture(fg, new Color(20, 17, 23));
            Texture background = GetTexture(bg, new Color(65, 60, 55));

            Background = new RenderTexture(Screen.Width, Screen.Height);

            if (bg == "") {
                var rect = new IntRect(0, 0, width: (int)Screen.Width, height: (int)Screen.Height);
                Background.Draw(Drawing.SpriteOf(foreground, rect));
                Background.Draw(Drawing.SpriteOf(background, Bounds));
            }
            Background.Display();

            Foreground = new RenderTexture(Screen.Width, Screen.Height);

            foreach (var tile in Game.Level.Tiles) {
                var rect = Geometry.AdaptRect(tile.Rect);
                Foreground.Draw(Drawing.SpriteOf(foreground, rect));
            }
            Foreground.Display();
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