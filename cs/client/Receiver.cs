using System;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace shootingame
{
    public class Receiver
    {
        public IPEndPoint EndPoint;
        public Receiver(UdpClient client)
        {
	        this.client = client;
            receiveTask = Task.Run(Reception);
            Console.WriteLine($"Listening on {(IPEndPoint)client.Client.LocalEndPoint}");
        }
        public byte[] GetBytes()
        {
            if (receiveTask.IsCompleted) {
                byte[] bytes = receiveTask.Result;
                receiveTask = Task.Run(Reception);
                return bytes;
            }            

            return null;
        }
        public void Close()
        {
            //client.Close();
        }
       
        private byte[] Reception()
        {
	        IPEndPoint e = new IPEndPoint(IPAddress.Any, 0);
            byte[] data = client.Receive(ref e);
            EndPoint = e;
            return data;
        }

        private UdpClient client;
        private Task<byte[]> receiveTask;
    }
}