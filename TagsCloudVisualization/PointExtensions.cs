using System.Drawing;
using System.Numerics;

namespace TagsCloudVisualization
{
    public static class PointExtensions
    {
        public static Vector2 ToVector(this Point point) => new Vector2(point.X, point.Y);
    }
}