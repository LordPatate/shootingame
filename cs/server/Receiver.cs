using System;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace server
{
    class Receiver
    {
        public static IPEndPoint EndPoint;
        public static byte[] GetBytes()
        {
            if (receiveTask is null) {
                receiveTask = Task.Run(Reception);
                return null;
            }
            if (receiveTask.IsCompleted) {
                byte[] bytes = receiveTask.Result;
                receiveTask = Task.Run(Reception);
                return bytes;
            }            

            return null;
        }
        public static void Close()
        {
            client.Close();
        }
       
        private static byte[] Reception()
        {
            EndPoint = new IPEndPoint(IPAddress.Any, 4242);
            Console.WriteLine("Waiting for data");
            return client.Receive(ref EndPoint);
        }

        private static UdpClient client = new UdpClient(4242);
        private static Task<byte[]> receiveTask;
    }
}