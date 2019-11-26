using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Sockets;
using System.Net;
using System;

namespace server
{
    class Program
    {
        static void Main(string[] args)
        {            
            while (Prompt.ReadLine() != "quit")
            {
                byte[] receivedBytes = Receiver.GetBytes();
                if (receivedBytes is null)
                    continue;
                
                ProcessData(receivedBytes, Receiver.EndPoint);
            }

            Receiver.Close();
        }

        static void ProcessData(byte[] receivedBytes, IPEndPoint endPoint)
        {
            Console.WriteLine($"Data was received from {endPoint.Address}");
            
            using MemoryStream stream = new MemoryStream(receivedBytes);
            BinaryFormatter deserializer = new BinaryFormatter();
            
            shootingame.GameState state = (shootingame.GameState)deserializer.Deserialize(stream);
            
            for (int i = 0; i < state.PlayersPos.Length; ++i) {
                Console.WriteLine($"Player {i}: X = {state.PlayersPos[i].X}, Y = {state.PlayersPos[i].Y}");
            }
        }
    }
}
