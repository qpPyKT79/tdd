using System;
using System.Drawing;
using System.Numerics;

namespace TagsCloudVisualization
{
    public static class Vector2Extensions
    {
        public static Point RoundToPoint(this Vector2 vector)
        {
            return new Point((int) Math.Round(vector.X), (int) Math.Round(vector.Y));
        }

        public static bool IsClose(this Vector2 first, Vector2 second) =>
            Math.Abs((first - second).Length()) < 2;

        public static float Cross(this Vector2 first, Vector2 second) =>
            first.X * second.Y + first.Y * second.X;
    }
}