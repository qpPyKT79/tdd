using System.Drawing;
using System.Numerics;

namespace TagsCloudVisualization
{
    public static class RectangleExtensions
    {
        public static Vector2 GetCenter(this Rectangle rectangle) =>
            (rectangle.Location.ToVector() + new Vector2(rectangle.Right, rectangle.Bottom)) / 2.0f;

        public static void Translate(this Rectangle rectangle, Vector2 vector) =>
            rectangle.Location = (vector + rectangle.Location.ToVector()).RoundToPoint();
    }
}