using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Sockets;
using System.Net;
using System;
using shootingame;

namespace server
{
    class Program
    {
        static BinaryFormatter formatter = new BinaryFormatter();
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
            
            int turn = 0;
            while (Prompt.ReadLine() != "quit")
            {
                ++turn;
                turn %= 500000;
                if (turn == 0) {
                    Task.WaitAll(tasks.ToArray());
                    tasks.Clear();
                    PlayerManager.Refresh();
                }

                byte[] receivedBytes = receiver.GetBytes();
                if (receivedBytes is null)
                    continue;
                
                tasks.Add(Task.Run(() => ProcessData(receivedBytes, receiver.EndPoint, client)));
            }

            Task.WaitAll(tasks.ToArray());
            receiver.Close();
            client.Close();
        }

        static void ProcessData(byte[] receivedBytes, IPEndPoint endPoint, UdpClient client)
        {
            GameState state = GameState.FromBytes(formatter, receivedBytes);
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

            byte[] data = state.ToBytes(formatter);
            client.Send(data, data.Length,
	        endPoint.Address.ToString(), endPoint.Port);
        }
    }
}
