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
        public static Dictionary<IPAddress, LightPlayer> players = new Dictionary<IPAddress, LightPlayer>();

        public static int Add(IPAddress address)
        {
            int id = -1;
            try {
                id = players[address].ID;
                
                if (freePlayerIDs[id]) {
                    freePlayerIDs[id] = false;
                    lastUpdated[address] = DateTime.Now;
                    Console.WriteLine($"Player {id} has joined");
                }
                else {
                    Console.Error.WriteLine($"Error: connect request: address {address} is already used by player {id}");
                }
            }
            catch (KeyNotFoundException) {
                id = freePlayerIDs.Count;
                players.Add(address, new LightPlayer(id, level));
                freePlayerIDs.Add(false);
                lastUpdated.Add(address, DateTime.Now);
                Console.WriteLine($"Player {id} has joined");
            }
            
            return id;
        }

        public static void Remove(IPAddress address)
        {
            int id = players[address].ID;
            freePlayerIDs[id] = true;
            Console.WriteLine($"Player {id} has left");
        }

        public static bool Update(IPAddress address, GameState state)
        {
            try {
                lastUpdated[address] = DateTime.Now;
                
                int id = players[address].ID;
                if (id != state.PlayerID) {
                    return false;
                }
                
                players[address] = state.Players[0];
                if (state.Shots != null) {
                    shots.Add(state.Shots[0]);
                }
                return true;
            }
            catch (KeyNotFoundException) {
                Console.Error.WriteLine($"Error: update request: unknown player from {address}");
                return false;
            }
        }

        public static void Refresh()
        {
            DateTime now = DateTime.Now;
            List<IPAddress> toRemove = new List<IPAddress>();
            foreach (var keyVal in players)
            {
                if (freePlayerIDs[keyVal.Value.ID])
                    continue;
                
                var address = keyVal.Key;
                if (now - lastUpdated[address] >= timeout) {
                    toRemove.Add(address);
                }
            }
            foreach (var address in toRemove) {
                Remove(address);
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
            var cpy = new Dictionary<IPAddress, LightPlayer>(players);
            var array = new LightPlayer[cpy.Count];
            
            foreach (var player in cpy.Values) {
                int id = player.ID;
                
                array[id] = (freePlayerIDs[id]) ?
                    null:
                    player;
            }

            return array;
        }

        private static readonly TimeSpan timeout = new TimeSpan(0, 0, 30);
        private static List<bool> freePlayerIDs = new List<bool>();
        private static Dictionary<IPAddress, DateTime> lastUpdated = new Dictionary<IPAddress, DateTime>();
        public static List<LightShot> shots = new List<LightShot>();        
    }
}