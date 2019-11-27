using System.Threading;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Sockets;
namespace shootingame
{
    class Client
    {
        public static bool Connected = false;
        public static GameState ConnectToServer(string host)
        {
            byte[] data;
            sender = new UdpClient(host, Const.ServerPort);
            receiver.Connect(Const.ClientPort);

            state.Type = GameState.RequestType.Connect;
            data = state.ToBytes(formatter);
            sender.Send(data, data.Length);
        
            for (uint i = 0; i < 5; ++i) {
                data = receiver.GetBytes();
                
                if (data != null) {
                    state = GameState.FromBytes(formatter, data);
                    if (state.Type != GameState.RequestType.Connect)
                        continue;
                    
                    Connected = true;
                    return state;
                }

                Thread.Sleep(1000);
            }
            
            Disconnect();
            throw new Exception($"Failed to connect to server {host}");
        }

        public static void SendUpdate()
        {
            state.Type = GameState.RequestType.Update;
            state.PlayerID = Game.Player.ID;
            state.Players[state.PlayerID] = new LightPlayer(Game.Player);

            byte[] data = state.ToBytes(formatter);

            sender.Send(data, data.Length);
        }

        public static GameState ReceiveUpdate()
        {
            byte[] data = receiver.GetBytes();
            if (data is null) {
                if (turnsWaiting >= 10000) {
                    SendDisconnect();
                } else {
                    ++turnsWaiting;
                }
                return null;
            }
            turnsWaiting = 0;
            state = GameState.FromBytes(formatter, data);
            if (state.Type == GameState.RequestType.Disconnect) {
                Disconnect();
            }
            
            return state;
        }

        public static void SendDisconnect()
        {
            state.Type = GameState.RequestType.Disconnect;
            byte[] data = state.ToBytes(formatter);
            
            sender.Send(data, data.Length);
            
            Disconnect();
        }
        public static void Disconnect()
        {
            Connected = false;
            sender.Close();
            receiver.Close();
        }

        private static UdpClient sender;
        private static Receiver receiver = new Receiver();
        private static BinaryFormatter formatter = new BinaryFormatter();
        private static GameState state = new GameState();
        private static uint turnsWaiting = 0;
    }
}