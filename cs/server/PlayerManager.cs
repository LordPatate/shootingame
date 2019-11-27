using System.Linq;
using System.Collections.Generic;
using System.Net;
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
                    Console.WriteLine($"Player {id} has joined");
                }
                else {
                    Console.Error.WriteLine($"Error: connect request: address {address} is already used by player {id}");
                }
            }
            catch (KeyNotFoundException) {
                id = playerIDs.Count;
                players.Add(address, new LightPlayer(id, level));
                playerIDs.Add(false);
                Console.WriteLine($"Player {id} has joined");
            }
            finally {
                return id;
            }
        }

        public static void Remove(IPAddress address)
        {
            int id = (int)players[address].ID;
            playerIDs[id] = true;
            Console.WriteLine($"Player {id} has left");
        }

        public static LightPlayer[] GetPlayers()
        {
            var array = players.ToArray();
            for (int i = 0; i < playerIDs.Count; ++i) {
                if (playerIDs[i])
                    array[i] = null;
            }
            return array;
        }

        private static List<bool> playerIDs = new List<bool>();
    }
}