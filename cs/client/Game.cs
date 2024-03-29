using System.Threading.Tasks.Sources;
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
	public static LevelUpdateRequest levelInfos;
        public static Level Level;
        public static RenderTexture Background;
        public static RenderTexture Foreground;
        public static IntRect Bounds;
	public static bool TextureUpdated = false;

        public static void Init(GameState state, string name)
        {
            Running = true;
            Player = new Player(state.PlayerID, name);
	    levelInfos = (LevelUpdateRequest)state;
            
            LoadLevel();

	    state = MakeGameState();
	    Client.SendUpdate(state);

	    state = Client.ReceiveUpdate();
	    for (int i = 0; i < 10 && state == null; ++i) {
		Thread.Sleep(300);
		state = Client.ReceiveUpdate();
	    }
	    if (state == null) {
		Client.SendDisconnect();
		return;
	    }
	    if (state.Type != GameState.RequestType.Update)
		return;

	    UpdateFromGameState((UpdateRequest)state);
	    Respawn();
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
            if (!Client.Connected)
                return;
            
            Player.Update(Level);

            GameState state = MakeGameState();
            Client.SendUpdate(state);
            state = Client.ReceiveUpdate();
            if (state != null) {
		switch (state.Type)
		{
		    case GameState.RequestType.Update:
			UpdateFromGameState((UpdateRequest)state);
			break;
		    case GameState.RequestType.LevelUpdate:
			levelInfos = (LevelUpdateRequest)state;
			LoadLevel();
			TextureUpdated = true;
                        Respawn();
			break;
		}
            }
        }
        private static GameState MakeGameState()
        {
            UpdateRequest state = new UpdateRequest(Player.ID) {
                Players = new LightPlayer[] { new LightPlayer(Player) },
            };
            
            if (Player.Shot) {
                state.Shots = new LightShot[] {
                    new LightShot() {
                        Origin = new LightVect2(Player.GetCOM()),
                        Dest = new LightVect2(Player.ShotPoint),
                        ID = (Player.ID << sizeof(int)/2*8) | Player.ShotCount
                    }
                };
            }

            return state;
        }
        private static void UpdateFromGameState(UpdateRequest state)
        {
            Players.Clear();
            Players.AddRange(state.Players);

            LightPlayer lightPlayer = Players[Player.ID];
            if (lightPlayer.Deaths != Player.Deaths) {
                Player.Deaths = lightPlayer.Deaths;
                Sounds.PlayShort("pop");
            }
            if (lightPlayer.Score != Player.Score) {
                Player.Score = lightPlayer.Score;
                Sounds.PlayShort("tic");
            }
            
            Player.HasRespawned = lightPlayer.HasRespawned;
            if (lightPlayer.ReSpawn) {
		Respawn();
		Player.HasRespawned = true;
            }

            if (state.Shots == null) {
                return;
            }
            for (int i = 0; i < state.Shots.Length; ++i) {
                LightShot shot = state.Shots[i];
                if (ShotLog.Contains(shot.ID))
                    continue;
                ShotLog.Add(shot.ID);
		        Sounds.PlayShort("shot");
                
                Color color = new Color(255, 220, 200, 255);
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

	public static void Respawn()
	{
	    Vector2i spawnPoint = Level.SpawnPoints[(Player.ID + Player.Deaths) % Level.SpawnPoints.Count];
	    Player.Rect.Left = spawnPoint.X;
	    Player.Rect.Top = spawnPoint.Y;
	}

        public static void LoadLevel()
        {
            Bounds = Geometry.ScaleRect(new IntRect(0, 0, (int)Screen.Width, (int)Screen.Height),
                80, 80);
	        Level = new Level(levelInfos);
            int wFactor = 100*Level.Bounds.Height/Level.Bounds.Width;

            Bounds = Geometry.ScaleRect(Bounds, wFactor, 100);

            DrawScene("", "");
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
                texture = new Texture(Program.ResourceDir + src);
            }

            return texture;
        }

        private static HashSet<int> ShotLog = new HashSet<int>();
    }
}
