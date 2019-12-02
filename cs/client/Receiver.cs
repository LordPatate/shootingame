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
                if (result == null) {
                    throw new SocketException((int)SocketError.ConnectionRefused);
                }
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
            try {
                IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref ep);
                return new BytesAndEndPoint() { Data = data, EndPoint = ep };
            }
            catch (SocketException e) {
                if (e.SocketErrorCode != SocketError.ConnectionRefused)
                    throw e;
                
                return null;
            }
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