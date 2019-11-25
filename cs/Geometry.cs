using System;
using SFML.Graphics;
using SFML.System;

namespace shootingame
{
    class Geometry
    {
        public static IntRect ScaleRect(IntRect rect, int wPercent, int hPercent)
        {
            int width = rect.Width * wPercent / 100;
            int height = rect.Height * hPercent / 100;
            return new IntRect(
                left: rect.Left + (rect.Width-width)/2,
                top: rect.Top + (rect.Height-height)/2,
                width: width, height: height
            );
        }

        public static double Dist(Vector2i a, Vector2i b)
        {
            Func<double, double> square = (x) => x*x;
            return Math.Sqrt(square((double)b.X - a.X) + square((double)b.Y - a.Y));
        }
        public static double Cos(Vector2i origin, int x, int y)
        {
            return (double)(x - origin.X) / Dist(origin, new Vector2i(x, y));
        }
        public static double Sin(Vector2i origin, int x, int y)
        {
            return (double)(y - origin.Y) / Dist(origin, new Vector2i(x, y));
        }

        public static int HorizontalIntersection(Vector2f a1, Vector2f a2, float y)
        {
            double coefA = (a1.Y - a2.Y) / (a1.X - a2.X);
            double ordA = a1.Y - a1.X * coefA;

            return (int)Math.Round((y - ordA)/coefA);
        }
        public static int VerticalIntersection(Vector2f a1, Vector2f a2, float x)
        {
            double coefA = (a1.Y - a2.Y) / (a1.X - a2.X);
            double ordA = a1.Y - a1.X * coefA;

            return (int)Math.Round(coefA*x + ordA);
        }
        public static Vector2i PointShade(IntRect bounds, Vector2i playerEye, int x, int y)
        {
            Vector2f a1 = new Vector2f(x, y), a2 = (Vector2f)playerEye;
            Vector2i s = new Vector2i();

            if (playerEye.X > x)
            { // shadow on left bound
                s.X = bounds.Left;
            }
            else
            { // shadow on right bound
                s.X = bounds.Left + bounds.Width;
            }
            s.Y = VerticalIntersection(a1, a2, s.X);
            if (s.Y >= bounds.Top && s.Y <= bounds.Top + bounds.Height)
                return s;
            
            if (playerEye.Y > y)
            { // shadow on top edge
                s.Y = bounds.Top;
            }
            else
            { // shadow on bottom edge
                s.Y = bounds.Top + bounds.Height;
            }
            s.X = HorizontalIntersection(a1, a2, s.Y);
            if (s.X >= bounds.Left && s.X <= bounds.Left + bounds.Width)
		        return s;

            s.X = x;
            return s;
        }

        public static bool IntersectLine(IntRect rect, Vector2i point1, Vector2i point2)
        {
            int rectX1 = rect.Left, rectX2 = rect.Left + rect.Width,
                rectY1 = rect.Top, rectY2 = rect.Top + rect.Height;
            Vector2f a1 = (Vector2f)point1, a2 = (Vector2f)point2;

            // easy cases
            if ((point1.X < rectX1 && point2.X < rectX1) || (point1.X > rectX2 && point2.X > rectX2)
            || (point1.Y < rectY1 && point2.Y < rectY1) || (point1.Y > rectY2 && point2.Y > rectY2))
                return false;
            
            if (point1.Y == point2.Y) { // horizontal line
                return point1.Y >= rectY1 && point1.Y < rectY2;
            }
            if (point1.X == point2.X) { // horizontal line
                return point1.X >= rectX1 && point1.X < rectX2;
            }

            // top edge
            int topX = HorizontalIntersection(a1, a2, rectY1);
            if (topX > rectX1 && topX < rectX2) {
                return true;
            }

            // bottom edge
            int bottomX = HorizontalIntersection(a1, a2, rectY2);
            if (bottomX > rectX1 && bottomX < rectX2) {
                return true;
            }

            // left edge
            int leftY = HorizontalIntersection(a1, a2, rectX1);
            return leftY > rectY1 && leftY < rectY2;
        }

        public static Vector2i HitPoint(IntRect rect, Vector2i point1, Vector2i point2)
        {
            Vector2i hitPoint = new Vector2i();

            int rectX1 = rect.Left, rectX2 = rect.Left + rect.Width,
                rectY1 = rect.Top, rectY2 = rect.Top + rect.Height;
            Vector2f a1 = (Vector2f)point1, a2 = (Vector2f)point2;
            
            double minDist = Double.PositiveInfinity;
            Action<Vector2i> minimize = (point) => {
                double dist = Dist(point1, point);
                if (dist < minDist) {
                    minDist = dist;
                    hitPoint = point;
                }
            };

            // top edge
            int topX = HorizontalIntersection(a1, a2, rectY1);
            if (topX > rectX1 && topX < rectX2) {
                minimize(new Vector2i(topX, rectY1));
            }

            // bottom edge
            int bottomX = HorizontalIntersection(a1, a2, rectY2);
            if (bottomX > rectX1 && bottomX < rectX2) {
                minimize(new Vector2i(bottomX, rectY2));
            }

            // left edge
            int leftY = HorizontalIntersection(a1, a2, rectX1);
            if (leftY > rectY1 && leftY < rectY2) {
                minimize(new Vector2i(leftY, rectX1));
            }

            // right edge
            int rihgtY = HorizontalIntersection(a1, a2, rectX2);
            if (rihgtY > rectY1 && rihgtY < rectY2) {
                minimize(new Vector2i(rihgtY, rectX2));
            }

            return hitPoint;
        }
    }
}