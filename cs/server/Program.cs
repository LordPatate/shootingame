using System.Threading.Tasks;
using System.Linq;
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
        static Level level = new Level();
        static Dictionary<IPAddress, LightPlayer> players = new Dictionary<IPAddress, LightPlayer>();
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
                    try {
                        uint id = (uint)players.Count;
                        players.Add(address, new LightPlayer(id, level));
                        
                        tasks.Add(SendGameState(state.Type, id, endPoint, sender));
                    
                        Console.WriteLine($"Player {id} has joined");
                    }
                    catch (ArgumentException) {
                        Console.Error.WriteLine($"Error: connect request: address {address} is already used by player {players[address].ID}");
                    }
                    break;
                case GameState.RequestType.Disconnect:
                    try {
                        Console.WriteLine($"Player {players[address].ID} has left");
                        players.Remove(address);
                    }
                    catch (KeyNotFoundException) {
                        Console.Error.WriteLine($"Error: disconnect request: unknown player from {address}");
                    }
                    break;
                case GameState.RequestType.Update:
                    try {
                        uint id = players[address].ID;
                        if (id != state.PlayerID) {
                            Console.Error.WriteLine($"Error: update request: player {id} is saying to be player {state.PlayerID}");
                            return;
                        }
                        players[address] = state.Players[(int)id];
                        tasks.Add(SendGameState(state.Type, id, endPoint, sender));
                    }
                    catch (KeyNotFoundException) {
                        Console.Error.WriteLine($"Error: update request: unknown player from {address}");
                    }
                    break;
            }
        }

        static async Task SendGameState(GameState.RequestType type, uint playerID, IPEndPoint endPoint, UdpClient sender)
        {
            GameState state = new GameState() {
                Type = type,
                PlayerID = playerID,
                Players = players.Values.ToArray()
            };

            byte[] data = new byte[1];
            await Task.Run(() => data = state.ToBytes(formatter));
            sender.Send(data, data.Length, endPoint);
        }
    }
}
