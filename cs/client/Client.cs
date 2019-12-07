using System.Threading;
using System;
using System.Net.Sockets;
using System.Net;
namespace shootingame
{
    class Client
    {
        public static bool Connected = false;
        public static GameState ConnectToServer(string host)
        {
            byte[] data;
            client = new UdpClient(0);
            try {
		client.Connect(host, Const.ServerPort);
	    } catch (Exception) {
		return null;
	    }
	    receiver = new Receiver(client);

	    GameState state = new GameState();
	    state.Type = GameState.RequestType.Connect;
	    data = state.ToBytes();
            try {
		client.Send(data, data.Length);
            
                for (uint i = 0; i < 5; ++i) {
                    data = receiver.GetBytes(out IPEndPoint endPoint);
                    
                    if (data != null) {
                        state = GameState.FromBytes(data);
                        if (state.Type != GameState.RequestType.LevelUpdate)
                            continue;
                        
                        Connected = true;
                        return state;
                    }

                    Thread.Sleep(1000);
                }
            } catch (SocketException) {}
            
            Disconnect();
            return null;
        }

        public static void SendUpdate(GameState state)
        {
            byte[] data = state.ToBytes();

            try {
                client.Send(data, data.Length);
            }
            catch (SocketException) {
                Disconnect();
            }
        }

        public static GameState ReceiveUpdate()
        {
            try {
                byte[] data = receiver.GetBytes(out IPEndPoint endPoint);
                if (data is null) {
                    if (turnsWaiting >= 100) {
                        SendDisconnect();
                    } else {
                        ++turnsWaiting;
                    }
                    return null;
                }
                turnsWaiting = 0;
                GameState state = GameState.FromBytes(data);
                if (state.Type == GameState.RequestType.Disconnect) {
                    Disconnect();
                }
                
                return state;
            }
            catch (SocketException) {
                Disconnect();
                return null;
            }
        }

        public static void SendDisconnect()
        {
            GameState state = new GameState();
            state.Type = GameState.RequestType.Disconnect;
            byte[] data = state.ToBytes();
            
            try {
                client.Send(data, data.Length);
            }
            catch (SocketException) {}
            
            Disconnect();
        }
        public static void Disconnect()
        {
            Connected = false;
            client.Close();
            receiver.Close();
        }

        private static UdpClient client;
        private static Receiver receiver;
        private static uint turnsWaiting = 0;
    }
}
