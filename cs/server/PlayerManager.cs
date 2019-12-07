using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using shootingame;

namespace server
{
    class PlayerManager
    {
	public static readonly string[] levels = LevelInfos.Init();
        public static Level level;
        public static int levelID;
        public static Dictionary<IPEndPoint, LightPlayer> players = new Dictionary<IPEndPoint, LightPlayer>();

	public static void Init(string[] args)
	{
            levelID = 0;
            if (args.Length >= 1) {
                if (Int32.TryParse(args[0], out int i))
                    levelID = i % levels.Length;
            }
            Console.WriteLine($"Starting server with level {levelID}");
            level = new Level(levels[levelID]);
	}

        public static int Add(IPEndPoint endPoint)
        {
            int id = freePlayerIDs.FindIndex((x) => x);
            if (id == -1) {
                id = freePlayerIDs.Count;
                freePlayerIDs.Add(false);
            }
            else {
                freePlayerIDs[id] = false;
            }
            lastUpdated.Add(endPoint, DateTime.Now);
            players.Add(endPoint, new LightPlayer(id, level));
            Console.WriteLine($"Player {id} has joined");
            return id;
        }
        public static void Remove(IPEndPoint endPoint)
        {
            int id = players[endPoint].ID;
            string name = players[endPoint].Name;
            freePlayerIDs[id] = true;
            players.Remove(endPoint);
            lastUpdated.Remove(endPoint);
            
            string msg = $"Player {id} has left";
            if (name != null) {
                msg += $" (was {name})";
            }
            Console.WriteLine(msg);
        }

        public static bool Update(IPEndPoint endPoint, UpdateRequest state)
        {
            try {
                lastUpdated[endPoint] = DateTime.Now;
                
                int id = players[endPoint].ID;
                if (id != state.PlayerID) {
                    return false;
                }
                
                UpdatePlayer(endPoint, state.Players[0]);
                
                if (state.Shots != null) {
                    LightShot shot = state.Shots[0];
                    if (shots.Any((x) => x.ID == shot.ID))
                        return true;

                    shots.Add(shot);
                    
                    var target = FindTarget(shot, id);
                    if (target != null) {
                        target.ReSpawn = true;
			            target.Deaths += 1;
                        players[endPoint].Score += 1;
                    }
                }
                return true;
            }
            catch (KeyNotFoundException) {
                Console.Error.WriteLine($"Error: update request: unknown player from {endPoint}");
                return false;
            }
        }

        public static void Refresh()
        {
            DateTime now = DateTime.Now;
            List<IPEndPoint> toRemove = new List<IPEndPoint>();
            
            var cpy = new Dictionary<IPEndPoint, LightPlayer>(players);
            foreach (var keyVal in cpy)
            {
                if (freePlayerIDs[keyVal.Value.ID])
                    continue;
                
                var endPoint = keyVal.Key;
                if (now - lastUpdated[endPoint] >= timeout) {
                    toRemove.Add(endPoint);
                }
            }
            foreach (var endPoint in toRemove) {
                Remove(endPoint);
            }
        }

        public static LightPlayer[] GetPlayers()
        {
            var cpy = new List<LightPlayer>(players.Values);
            var array = new LightPlayer[freePlayerIDs.Count];
            
            foreach (var player in cpy) {
                array[player.ID] = player;
            }

            return array;
        }

	public static LightVect2[] GetTiles()
	{
	    var array = new LightVect2[level.Tiles.Count];

	    for (int i = 0; i < level.Tiles.Count; ++i) {
		var rect = level.Tiles[i].Rect;
		array[i] = new LightVect2() { X = rect.Left, Y = rect.Top };
	    }

	    return array;
	}

	public static LightVect2[] GetSpawnPoints()
	{
	    var array = new LightVect2[level.SpawnPoints.Count];

	    for (int i = 0; i < level.SpawnPoints.Count; ++i) {
		var pos = level.SpawnPoints[i];
		array[i] = new LightVect2() { X = pos.X, Y = pos.Y };
	    }

	    return array;
	}

        private static void UpdatePlayer(IPEndPoint endPoint, LightPlayer clientPlayer)
        {
            LightPlayer player = players[endPoint];
            player.Name = clientPlayer.Name;
            player.Pos = clientPlayer.Pos;
            player.State = clientPlayer.State;
            player.Frame = clientPlayer.Frame;
            player.Direction = clientPlayer.Direction;
            player.HookPoint = clientPlayer.HookPoint;
            player.Hooked = clientPlayer.Hooked;

            if (clientPlayer.HasRespawned) {
                player.ReSpawn = false;
                player.HasRespawned = false;
            }
        }

        private static LightPlayer FindTarget(LightShot shot, int id)
        {
            foreach (var lightPlayer in players.Values) {
                if (freePlayerIDs[lightPlayer.ID] || lightPlayer.ID == id)
                    continue;
                
                Player player = new Player(lightPlayer);
                player.MakeRect();
                player.Rect.Left = lightPlayer.Pos.X;
                player.Rect.Top = lightPlayer.Pos.Y;
                if (Geometry.ScaleRect(player.Rect, 120, 120).Contains(shot.Dest.X, shot.Dest.Y)) {
                    return lightPlayer;
                }
            }

            return null;
        }

        private static readonly TimeSpan timeout = new TimeSpan(0, 0, 30);
        private static List<bool> freePlayerIDs = new List<bool>();
        private static Dictionary<IPEndPoint, DateTime> lastUpdated = new Dictionary<IPEndPoint, DateTime>();
        public static List<LightShot> shots = new List<LightShot>();
    }
}
