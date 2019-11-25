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
            using UdpClient client = new UdpClient(4242);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 4242);
            Console.WriteLine("Waiting for data");
            byte[] receiveBytes = client.Receive(ref endPoint);
            Console.WriteLine("Data was received");
            MemoryStream stream = new MemoryStream(receiveBytes);
            BinaryFormatter deserializer = new BinaryFormatter();
            shootingame.GameState state = (shootingame.GameState)deserializer.Deserialize(stream);
            for (int i = 0; i < state.PlayersPos.Length; ++i) {
                Console.WriteLine($"Player {i}: X = {state.PlayersPos[i].X}, Y = {state.PlayersPos[i].Y}");
            }
        }
    }
}
