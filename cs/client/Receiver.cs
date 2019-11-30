using System;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace shootingame
{
    public class Receiver
    {
        public Receiver(UdpClient client)
        {
	        this.client = client;
            receiveTask = Task.Run(Reception);
            Console.WriteLine($"Listening on {(IPEndPoint)client.Client.LocalEndPoint}");
        }
        public byte[] GetBytes(out IPEndPoint endPoint)
        {
            if (receiveTask.IsCompleted) {
                var result = receiveTask.Result;
                receiveTask = Task.Run(Reception);

                endPoint = result.EndPoint;
                return result.Data;
            }            

            endPoint = null;
            return null;
        }
        public void Close()
        {
            //client.Close();
        }
       
        private BytesAndEndPoint Reception()
        {
	        IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
            byte[] data = client.Receive(ref ep);
            return new BytesAndEndPoint() { Data = data, EndPoint = ep };
        }

        private UdpClient client;
        private Task<BytesAndEndPoint> receiveTask;
        private class BytesAndEndPoint
        {
            public byte[] Data;
            public IPEndPoint EndPoint;
        }
    }
}