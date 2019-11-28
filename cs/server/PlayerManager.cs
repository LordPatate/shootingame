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
                players[address] = state.Players[id];
                return true;
            }
            catch (KeyNotFoundException) {
                Console.Error.WriteLine($"Error: update request: unknown player from {address}");
                return false;
            }
        }

        public static void Refresh()
        {
            try {
                DateTime now = DateTime.Now;
                foreach (var keyVal in players)
                {
                    if (freePlayerIDs[keyVal.Value.ID])
                        continue;
                    
                    var address = keyVal.Key;
                    if (now - lastUpdated[address] >= timeout) {
                        Remove(address);
                    }
                }
            }
            catch (AggregateException e) {
                Console.Error.WriteLine($"Warning: unable to refresh player list: {e.InnerException.Message}");
            }
        }

        public static LightPlayer[] GetPlayers()
        {
            var array = new LightPlayer[players.Count];
            
            foreach (var player in players.Values) {
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
    }
}