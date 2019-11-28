using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using shootingame;

namespace server
{
    class PlayerManager
    {
        public static Level level = new Level();
        public static Dictionary<IPAddress, LightPlayer> players = new Dictionary<IPAddress, LightPlayer>();

        public static int Add(IPAddress address)
        {
            int id = -1;
            try {
                id = (int)players[address].ID;
                
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
                players.Add(address, new LightPlayer((uint)id, level));
                freePlayerIDs.Add(false);
                lastUpdated.Add(address, DateTime.Now);
                Console.WriteLine($"Player {id} has joined");
            }
            
            return id;
        }

        public static void Remove(IPAddress address)
        {
            int id = (int)players[address].ID;
            freePlayerIDs[id] = true;
            Console.WriteLine($"Player {id} has left");
        }

        public static bool Update(IPAddress address, GameState state)
        {
            try {
                uint id = players[address].ID;
                if (id != state.PlayerID) {
                    Console.Error.WriteLine($"Warning: update request: player {id} is saying to be player {state.PlayerID}");
                    return false;
                }
                lastUpdated[address] = DateTime.Now;
                players[address] = state.Players[(int)id];
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
                    if (freePlayerIDs[(int)keyVal.Value.ID])
                        continue;
                    
                    var address = keyVal.Key;
                    if (now - lastUpdated[address] >= timeout) {
                        Remove(address);
                    }
                }
            }
            catch (InvalidOperationException e) {
                Console.Error.WriteLine($"Warning: unable to refresh player list: {e.Message}");
            }
        }

        public static LightPlayer[] GetPlayers()
        {
            var array = new LightPlayer[players.Count];
            
            foreach (var player in players.Values) {
                int id = (int)player.ID;
                
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