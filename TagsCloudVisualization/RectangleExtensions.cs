using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using NUnit.Framework;

namespace TagsCloudVisualization
{
    public static class RectangleExtensions
    {
        public static Vector2 GetCenter(this Rectangle rectangle) =>
            (rectangle.Location.ToVector() + new Vector2(rectangle.Right, rectangle.Bottom)) / 2.0f;

        public static IEnumerable<Point> GetAllPoints(this Rectangle rectangle)
        {
            yield return rectangle.Location;
            yield return new Point(rectangle.Top, rectangle.Right);
            yield return new Point(rectangle.Bottom, rectangle.Left);
            yield return new Point(rectangle.Bottom, rectangle.Right);
        }

        public static Rectangle ExpandByPoint(this Rectangle rectangle, Point point)
        {
            var dimensions =
                (Top: rectangle.Top,
                Botom: rectangle.Bottom,
                Left: rectangle.Left,
                Right: rectangle.Right);

            if (point.X > dimensions.Right)
            {
                dimensions.Right = point.X;
            }
            if (point.X < dimensions.Left)
            {
                dimensions.Left = point.X;
            }
            if (point.Y < dimensions.Top)
            {
                dimensions.Top = point.Y;
            }
            if (point.Y > dimensions.Botom)
            {
                dimensions.Botom = point.Y;
            }

            var resultLoaction = new Point(dimensions.Left, dimensions.Top);
            var resultSize = new Size(dimensions.Right - dimensions.Left,
                dimensions.Botom - dimensions.Top);
            return new Rectangle(resultLoaction, resultSize);
        }
    }
}