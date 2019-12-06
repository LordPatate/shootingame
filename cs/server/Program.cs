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
        static List<Task> tasks = new List<Task>();
        static void Main(string[] args)
        {
            PlayerManager.levelID = 0;
            if (args.Length >= 1) {
                if (Int32.TryParse(args[0], out int i))
                    PlayerManager.levelID = i;
            }
            Console.WriteLine($"Starting server with level {PlayerManager.levelID}");
            PlayerManager.level = new Level(Level.levelInfos[PlayerManager.levelID]);
            
            UdpClient client = new UdpClient(Const.ServerPort);
            Receiver receiver = new Receiver(client);
            Console.WriteLine("Server ready to accept connections");
            
            DateTime lastRefresh = DateTime.Now;
            TimeSpan refreshPeriod = new TimeSpan(0, 0, 0, 0, Const.GameStepDuration);
            string cmd = "";
            while (cmd != "quit")
            {
                cmd = Prompt.ReadLine();
                
                if (DateTime.Now - lastRefresh >= refreshPeriod) {
                    lastRefresh = DateTime.Now;
                    tasks.RemoveAll((task) => task.Status != TaskStatus.Running);
                    tasks.Add(Task.Run(PlayerManager.Refresh));
                }
                
                byte[] receivedBytes = receiver.GetBytes(out IPEndPoint endPoint);
                while (cmd != "quit" && PlayerManager.players.Count == 0 && receivedBytes == null) {
                    Thread.Sleep(1000);
                    cmd = Prompt.ReadLine();
                    receivedBytes = receiver.GetBytes(out endPoint);
                }
                
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
                            SendGameState(state.Type, id, endPoint, client);
                    }
                    break;
                case GameState.RequestType.Disconnect:
                    PlayerManager.Remove(endPoint);
                    break;
                case GameState.RequestType.Update:
                    if (PlayerManager.Update(endPoint, state))
                        SendGameState(state.Type, state.PlayerID, endPoint, client);
                    break;
            }
        }

        static void SendGameState(GameState.RequestType type, int playerID, IPEndPoint endPoint, UdpClient client)
        {
            GameState state = new GameState() {
                Type = type,
                LevelID = PlayerManager.levelID,
                PlayerID = playerID,
                Players = PlayerManager.GetPlayers(),
                Shots = PlayerManager.shots.ToArray()
            };

            byte[] data = state.ToBytes();
            client.Send(data, data.Length,
	        endPoint.Address.ToString(), endPoint.Port);
        }
    }
}
