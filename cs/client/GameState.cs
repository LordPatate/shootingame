using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using SFML.System;

namespace shootingame
{
    [Serializable()]
    public struct LightVect2 {
        public int X, Y;
    }
    [Serializable()]
    public struct LightShot {
        public LightVect2 Origin, Dest;
        public byte Alpha;
    }
    [Serializable()]
    public class LightPlayer
    {
        public int ID;
        public string Name;
        public int Score;
        public LightVect2 Pos;
        public PlayerState State;
        public uint Frame;
        public bool Direction;
        public LightVect2 HookPoint;
        public bool Hooked;
        public bool ReSpawn;
        public bool HasRespawned;

        public LightPlayer(Player player) {
            ID = player.ID;
            Name = player.Name;
            Score = player.Score;
            Pos = new LightVect2() {
                X = player.Rect.Left,
                Y = player.Rect.Top
            };
            State = player.State;
            Frame = player.Frame;
            Direction = player.Direction;
            HookPoint = new LightVect2() {
                X = player.HookPoint.X,
                Y = player.HookPoint.Y
            };
            Hooked = player.Hooked;
            HasRespawned = player.HasRespawned;
        }
        public LightPlayer(int id, Level level) {
            ID = id;
            Vector2i spawnPoint = level.SpawnPoints[id % level.SpawnPoints.Count];
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
        public LightShot[] Shots;
        public int PlayerID;
        public int LevelID;

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