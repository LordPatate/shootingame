using System;
using Newtonsoft.Json;
using SFML.System;

namespace shootingame
{
    public struct LightVect2 {
        public int X {get; set;}
	public int Y {get; set;}
    }
    public struct LightShot {
        public LightVect2 Origin {get; set;}
	public LightVect2 Dest {get; set;}
        public byte Alpha {get; set;}
    }
    public class LightPlayer
    {
        public int ID {get; set;}
        public string Name {get; set;}
        public int Score {get; set;}
        public int Deaths {get; set;}
        public LightVect2 Pos {get; set;}
        public PlayerState State {get; set;}
        public uint Frame {get; set;}
        public bool Direction {get; set;}
        public LightVect2 HookPoint {get; set;}
        public bool Hooked {get; set;}
        public bool ReSpawn {get; set;}
        public bool HasRespawned {get; set;}

	public LightPlayer() {} // Used by Json deserialization

        public LightPlayer(Player player) {
            ID = player.ID;
            Name = player.Name;
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
            Pos = new LightVect2() {
		X = spawnPoint.X,
		Y = spawnPoint.Y
	    };
        }
    }

    public class GameState
    {
        public enum RequestType
        {
            Update,
            Connect,
            Disconnect
        }
        public LightPlayer[] Players {get; set;}
        public LightShot[] Shots {get; set;}
        public int PlayerID {get; set;}
        public int LevelID {get; set;}

        public RequestType Type {get; set;}

        public byte[] ToBytes()
        {
            string json = JsonConvert.SerializeObject(this);

            byte[] data = new byte[json.Length];
	    for (int i = 0; i < json.Length; ++i)
		data[i] = (byte)json[i];

	    return data;
        }

        public static GameState FromBytes(byte[] data)
	{
	    char[] chars = new char[data.Length];
	    for (int i = 0; i < data.Length; ++i)
		chars[i] = (char)data[i];

	    string json = new string(chars);

            return JsonConvert.DeserializeObject<GameState>(json);
        }
    }
}
