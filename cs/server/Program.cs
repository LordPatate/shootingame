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
            level.Init(Level.levelInfos[0]);
            
            Receiver receiver = new Receiver();
            receiver.Connect(Const.ServerPort);
            UdpClient sender = new UdpClient();
            Console.WriteLine("Server ready to accept connections");
            
            while (Prompt.ReadLine() != "quit")
            {
                // Remove finished tasks
                tasks = tasks.FindAll((task) => task.Status == TaskStatus.Running);

                byte[] receivedBytes = receiver.GetBytes();
                if (receivedBytes is null)
                    continue;
                
                tasks.Add(Task.Run(() => ProcessData(receivedBytes, receiver.EndPoint, sender)));
            }

            Task.WaitAll(tasks.ToArray());
            receiver.Close();
            sender.Close();
        }

        static void ProcessData(byte[] receivedBytes, IPEndPoint endPoint, UdpClient sender)
        {
            IPAddress address = endPoint.Address;
            endPoint.Port = Const.ClientPort;
            GameState state = GameState.FromBytes(formatter, receivedBytes);
            switch (state.Type)
            {
                case GameState.RequestType.Connect:
                    int id = PlayerManager.Add(address);    
                    if (id != -1)
                        SendGameState(state.Type, (uint)id, endPoint, sender);
                    break;
                case GameState.RequestType.Disconnect:
                    PlayerManager.Remove(address);
                    break;
                case GameState.RequestType.Update:
                    try {
                        uint id = PlayerManager.players[address].ID;
                        if (id != state.PlayerID) {
                            Console.Error.WriteLine($"Error: update request: player {id} is saying to be player {state.PlayerID}");
                            return;
                        }
                        PlayerManager.players[address] = state.Players[(int)id];
                        SendGameState(state.Type, id, endPoint, sender);
                    }
                    catch (KeyNotFoundException) {
                        Console.Error.WriteLine($"Error: update request: unknown player from {address}");
                    }
                    break;
            }
        }

        static void SendGameState(GameState.RequestType type, uint playerID, IPEndPoint endPoint, UdpClient sender)
        {
            GameState state = new GameState() {
                Type = type,
                PlayerID = playerID,
                Players = PlayerManager.GetPlayers()
            };

            byte[] data = state.ToBytes(formatter);
            sender.Send(data, data.Length, endPoint);
        }
    }
}
