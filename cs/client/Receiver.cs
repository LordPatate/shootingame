using System;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace shootingame
{
    public class Receiver
    {
        public IPEndPoint EndPoint;
        public void Connect(int port)
        {
            client = new UdpClient(port);
            EndPoint = new IPEndPoint(IPAddress.Any, port);
        }
        public byte[] GetBytes()
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
        public void Close()
        {
            client.Close();
        }
       
        private byte[] Reception()
        {
            return client.Receive(ref EndPoint);
        }

        private UdpClient client;
        private Task<byte[]> receiveTask;
    }
}