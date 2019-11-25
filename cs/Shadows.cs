using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;

namespace shootingame
{
    class Shadows
    {
        public static void Compute()
        {
            var shades = new List<ConvexShape>();

            bounds = Game.Level.Bounds;
            topLeft = new Vector2i(bounds.Top, bounds.Left);
            topRight = new Vector2i(bounds.Top, bounds.Left + bounds.Width);
            botLeft = new Vector2i(bounds.Top + bounds.Height, bounds.Left);
            botRight = new Vector2i(bounds.Top + bounds.Height, bounds.Left + bounds.Width);
            playerRect = Game.Player.Rect;
            playerEye = new Vector2i(
                x: playerRect.Left + playerRect.Width/2,
                y: playerRect.Top + 10
            );

            foreach (var tile in Game.Level.Tiles)
            {
                var rect = tile.Rect;
                int x = rect.Left, y = rect.Top, w = rect.Width, h = rect.Height;

                // TopLeft - BotRight diagonal
                var s = new ConvexShape(4);
                point1 = Geometry.PointShade(bounds, playerEye, x, y);
                point2 = Geometry.PointShade(bounds, playerEye, x+w, y+h);
                s.SetPoint(0, (Vector2f)point1);
                s.SetPoint(1, new Vector2f(x, y));
                s.SetPoint(2, new Vector2f(x+w, y+h));
                s.SetPoint(3, (Vector2f)point2);

                bool above = playerEye.Y < playerEye.X + (y-x);
                CheckCorner(ref s, botRight, above, above);
                if (above) {
                    CheckCorner(ref s, botLeft, above, above);
                } else {
                    CheckCorner(ref s, topRight, above, above);
                }
                CheckCorner(ref s, topLeft, above, above);

                shades.Add(s);

                // BotLeft - TopRight diagonal
                s = new ConvexShape(4);
                point1 = Geometry.PointShade(bounds, playerEye, x, y+h);
                point2 = Geometry.PointShade(bounds, playerEye, x+w, y);
                s.SetPoint(0, (Vector2f)point1);
                s.SetPoint(1, new Vector2f(x, y+h));
                s.SetPoint(2, new Vector2f(x+w, y));
                s.SetPoint(3, (Vector2f)point2);

                above = playerEye.Y < -playerEye.X + (y+h+x);
                CheckCorner(ref s, topRight, above, !above);
                if (above) {
                    CheckCorner(ref s, botRight, above, !above);
                } else {
                    CheckCorner(ref s, topLeft, above, !above);
                } 
                CheckCorner(ref s, botLeft, above, !above);

                shades.Add(s);
            }

            Screen.Shades = shades;
        }

        private static void CheckCorner(ref ConvexShape s, Vector2i corner, bool above, bool right)
        {
            Func<Vector2i, double> computeAngle = (v) =>
            {
                int x = v.X, y = v.Y;

                var val = Math.Acos(Geometry.Cos(playerEye, x, y));
                if (playerEye.Y < y) {
                    val = -val;
                }
                if (val < 0 && right) {
                    val += Math.PI * 2;
                }
                return val;
            };

            var upper = computeAngle(point1);
            var lower = computeAngle(point2);
            var angle = computeAngle(corner);
            if (above) {
                if (upper < angle && angle < lower) {
                    Append(ref s, corner);
                }
                return;
            }
            if (upper > angle && angle > lower) {
                Append(ref s, corner);
            }
        }

        private static void Append(ref ConvexShape s, Vector2i point)
        {
            var count = s.GetPointCount();
            s.SetPointCount(count + 1);
            s.SetPoint(count, (Vector2f)point);
        }

        private static IntRect bounds, playerRect;
        private static Vector2i topLeft, topRight,
            botLeft, botRight,
            playerEye,
            point1, point2;
    }
}