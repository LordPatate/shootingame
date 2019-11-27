using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using SFML.System;

namespace shootingame
{
    [Serializable()]
    public class LightPlayer
    {
        [Serializable()]
        public struct LightVect2 {
            public int X, Y;
        }
        public uint ID;
        public LightVect2 Pos;
        public PlayerState State;
        public uint Frame;
        public bool Direction;
        public LightVect2 HookPoint;
        public bool Hooked;

        public LightPlayer(Player player) {
            ID = player.ID;
            Pos = new LightVect2() {
                X = player.Rect.Left,
                Y = player.Rect.Top
            };
            State = player.State;
            Direction = player.Direction;
            HookPoint = new LightVect2() {
                X = player.HookPoint.X,
                Y = player.HookPoint.Y
            };
            Hooked = player.Hooked;
        }
        public LightPlayer(uint id, Level level) {
            Vector2i spawnPoint = level.SpawnPoints[(int)id % level.SpawnPoints.Count];
            Pos.X = spawnPoint.X;
            Pos.Y = spawnPoint.Y;
        }
    }

    [Serializable()]
    public class GameState
    {
        public enum RequestType
        {
            Update,
            Connect,
            Disconnect
        }
        public LightPlayer[] Players;
        public uint PlayerID;

        public RequestType Type;

        public byte[] ToBytes(BinaryFormatter formatter)
        {
            MemoryStream stream = new MemoryStream();

            formatter.Serialize(stream, this);
            
            byte[] data = new byte[stream.Length];
            stream.Seek(0, SeekOrigin.Begin);
            int nb = stream.Read(data, 0, data.Length);
            stream.Close();

            return data;
        }

        public static GameState FromBytes(BinaryFormatter formatter, byte[] data)
        {
            using MemoryStream stream = new MemoryStream(data);
            
            return (GameState)formatter.Deserialize(stream);
        }
    }
}