using System;
using SFML.System;

namespace shootingame
{
    [Serializable()]
    public class GameState
    {
        [Serializable()]
        public class Point
        {
            public int X, Y;
        }
        public Point[] PlayersPos;
    }
}