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
                
                if (playerIDs[id]) {
                    playerIDs[id] = false;
                    lastUpdated[address] = DateTime.Now;
                    Console.WriteLine($"Player {id} has joined");
                }
                else {
                    Console.Error.WriteLine($"Error: connect request: address {address} is already used by player {id}");
                }
            }
            catch (KeyNotFoundException) {
                id = playerIDs.Count;
                players.Add(address, new LightPlayer((uint)id, level));
                playerIDs.Add(false);
                lastUpdated.Add(address, DateTime.Now);
                Console.WriteLine($"Player {id} has joined");
            }
            
            return id;
        }

        public static void Remove(IPAddress address)
        {
            int id = (int)players[address].ID;
            playerIDs[id] = true;
            Console.WriteLine($"Player {id} has left");
        }

        public static bool Update(IPAddress address, GameState state)
        {
            try {
                uint id = players[address].ID;
                if (id != state.PlayerID) {
                    Console.Error.WriteLine($"Error: update request: player {id} is saying to be player {state.PlayerID}");
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
            DateTime now = DateTime.Now;
            foreach (IPAddress key in players.Keys)
            {
                if (now - lastUpdated[key] >= timeout) {
                    Remove(key);
                }
            }
        }

        public static LightPlayer[] GetPlayers()
        {
            var array = players.Values.ToArray();
            for (int i = 0; i < playerIDs.Count; ++i) {
                if (playerIDs[i])
                    array[i] = null;
            }
            return array;
        }

        private static readonly TimeSpan timeout = new TimeSpan(0, 0, 3);
        private static List<bool> playerIDs = new List<bool>();
        private static Dictionary<IPAddress, DateTime> lastUpdated = new Dictionary<IPAddress, DateTime>();
    }
}