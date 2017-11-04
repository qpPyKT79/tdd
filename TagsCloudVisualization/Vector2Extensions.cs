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
    }
}