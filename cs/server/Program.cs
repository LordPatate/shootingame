
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System;
using shootingame;

namespace server
{
    class Program
    {
        static void Main(string[] args)
        {
	    PlayerManager.Init(args);
            UdpClient client = new UdpClient(Const.ServerPort);
            Receiver receiver = new Receiver(client);

            Console.WriteLine(String.Join("\n", new string[] {
                        "Server ready to accept connections.",
                        "Type 'quit' to shut the server down, 'next' to load next level."
                    }));
            
            TimeSpan refreshPeriod = new TimeSpan(0, 0, 0, 0, Const.GameStepDuration);
            List<Task> tasks = new List<Task>();

            DateTime lastRefresh = DateTime.Now;
            string cmd = "";
            while (cmd != "quit")
            {
                cmd = Prompt.ReadLine();

                if (PlayerManager.players.Count == 0) {
                    Thread.Sleep(1000);
                }

                if (DateTime.Now - lastRefresh >= refreshPeriod) {
                    lastRefresh = DateTime.Now;
                    tasks.RemoveAll((task) => task.Status != TaskStatus.Running);
                    tasks.Add(Task.Run(PlayerManager.Refresh));
                }

		if (cmd == "next") {
		    PlayerManager.NextLevel();
		    foreach (var keyVal in PlayerManager.players)
			SendGameState(GameState.RequestType.LevelUpdate, keyVal.Value.ID, keyVal.Key, client);
		}
                
                byte[] receivedBytes = receiver.GetBytes(out IPEndPoint endPoint);                
                if (receivedBytes != null)
                    tasks.Add(Task.Run(() => ProcessData(receivedBytes, endPoint, client)));
            }

            Task.WaitAll(tasks.ToArray());
            client.Close();
            receiver.Close();
        }

        static void ProcessData(byte[] receivedBytes, IPEndPoint endPoint, UdpClient client)
        {
            GameState state = GameState.FromBytes(receivedBytes);
            switch (state.Type)
            {
                case GameState.RequestType.Connect:
                    {
                        int id = PlayerManager.Add(endPoint);
                        if (id != -1)
                            SendGameState(GameState.RequestType.LevelUpdate, id, endPoint, client);
                    }
                    break;
                case GameState.RequestType.Disconnect:
                    PlayerManager.Remove(endPoint);
                    break;
                case GameState.RequestType.Update:
                    if (PlayerManager.Update(endPoint, (UpdateRequest)state))
                        SendGameState(state.Type, state.PlayerID, endPoint, client);
                    break;
            }
        }

        static void SendGameState(GameState.RequestType type, int playerID, IPEndPoint endPoint, UdpClient client)
        {
	    GameState state;
	    switch (type)
	    {
		case GameState.RequestType.Update:
		    state = new UpdateRequest(playerID) {
			Players = PlayerManager.GetPlayers(),
			Shots = PlayerManager.shots.ToArray()
		    };
		    break;
		case GameState.RequestType.LevelUpdate:
		    state = new LevelUpdateRequest(playerID) {
			Dimensions = new LightVect2() {
			    X = PlayerManager.level.Bounds.Width,
			    Y = PlayerManager.level.Bounds.Height
			},
			TilePos = PlayerManager.GetTiles(),
			SpawnPoints = PlayerManager.GetSpawnPoints()
		    };
		    break;
		default:
		    state = new GameState(type, playerID);
		    break;
	    }

            byte[] data = state.ToBytes();
            client.Send(data, data.Length,
	        endPoint.Address.ToString(), endPoint.Port);
        }
    }
}
