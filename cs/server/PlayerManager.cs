using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using shootingame;

namespace server
{
    class PlayerManager
    {
        public static Level level;
        public static int levelID;
        public static Dictionary<IPEndPoint, LightPlayer> players = new Dictionary<IPEndPoint, LightPlayer>();

        public static int Add(IPEndPoint endPoint)
        {
            int id = -1;
            try {
                id = players[endPoint].ID;
                
                if (freePlayerIDs[id]) {
                    freePlayerIDs[id] = false;
                    lastUpdated[endPoint] = DateTime.Now;
                    Console.WriteLine($"Player {id} has joined");
                    Console.WriteLine($"    on {endPoint}");
                }
                else {
                    Console.Error.WriteLine($"Error: connect request: endPoint {endPoint} is already used by player {id}");
                }
            }
            catch (KeyNotFoundException) {
                id = freePlayerIDs.Count;
                players.Add(endPoint, new LightPlayer(id, level));
                freePlayerIDs.Add(false);
                lastUpdated.Add(endPoint, DateTime.Now);
                Console.WriteLine($"Player {id} has joined");
                Console.WriteLine($"    on {endPoint}");
            }
            
            return id;
        }

        public static void Remove(IPEndPoint endPoint)
        {
            int id = players[endPoint].ID;
            freePlayerIDs[id] = true;
            players[endPoint] = new LightPlayer(id, level);
            Console.WriteLine($"Player {id} has left");
        }

        public static bool Update(IPEndPoint endPoint, GameState state)
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
                    shots.Add(shot);
                    
                    var target = FindTarget(shot);
                    if (target != null) {
                        target.ReSpawn = true;
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

            for (int i = 0; i < shots.Count; ++i) {
                byte alpha = (byte)(shots[i].Alpha - 16);
                shots[i] = new LightShot() {
                    Origin = shots[i].Origin, Dest = shots[i].Dest,
                    Alpha = alpha
                };
            }
            shots.RemoveAll((shot) => shot.Alpha < 16);
        }

        public static LightPlayer[] GetPlayers()
        {
            var cpy = new Dictionary<IPEndPoint, LightPlayer>(players);
            var array = new LightPlayer[cpy.Count];
            
            foreach (var player in cpy.Values) {
                int id = player.ID;
                
                array[id] = (freePlayerIDs[id]) ?
                    null:
                    player;
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

        private static LightPlayer FindTarget(LightShot shot)
        {
            foreach (var lightPlayer in players.Values) {
                if (freePlayerIDs[lightPlayer.ID])
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