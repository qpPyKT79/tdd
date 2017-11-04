using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Numerics;
using FluentAssertions;
using NUnit.Framework;

namespace TagsCloudVisualization
{
    public class CloudLayoutRender
    {
        public static Bitmap RenderImage(ICollection<Rectangle> rectangles)
        {
            var bounds = GetBounds(rectangles.SelectMany(rect => rect.GetAllPoints()));

            if (bounds.Width <= 0 || bounds.Height <= 0 || bounds.IsEmpty)
                throw new ArgumentException("Layout must contains elements");

            var bitmap = new Bitmap(bounds.Height, bounds.Width);
            var graphics = Graphics.FromImage(bitmap);
            var translate = -bounds.Location.ToVector();
            graphics.TranslateTransform(translate.X, translate.Y);

            foreach (var rectangle in rectangles)
            {
                graphics.FillRectangle(Brushes.CadetBlue, rectangle);
            }
            return bitmap;
        }

        private static Rectangle GetBounds(IEnumerable<Point> points)
        {
            if (!points.Any()) return Rectangle.Empty;

            var dimensions =
                (Top: int.MaxValue,
                Botom: int.MinValue,
                Left: int.MaxValue,
                Right: int.MinValue);
            foreach (var point in points)
            {
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
            }
            var resultLoaction = new Point(dimensions.Left, dimensions.Top);
            var resultSize = new Size(dimensions.Right - dimensions.Left,
                dimensions.Botom - dimensions.Top);
            return new Rectangle(resultLoaction, resultSize);
        }

        private static Rectangle GetBounds(IEnumerable<Rectangle> rectangles) => 
            rectangles.Aggregate(Rectangle.Empty, (current, rectangle) => GetBounds(rectangle, current));

        private static Rectangle GetBounds(Rectangle first, Rectangle second)
        {
            var pointsOutOfBounds = first.GetAllPoints().Where(point => !second.Contains(point));
            return pointsOutOfBounds.Aggregate(second, (current, point) => current.ExpandByPoint(point));
        }

    }

    [TestFixture]
    public class CloudLayoutRender_Should
    {
        [Test]
        public void ThrowArgumentException_WhenLayoutEmpty()
        {
            Action rendering = () =>CloudLayoutRender.RenderImage(Enumerable.Empty<Rectangle>().ToArray());
            rendering.ShouldThrow<ArgumentException>();
        }

        [TestCase(10, 10)]
        [TestCase(1, 1)]
        [TestCase(5, 1)]
        public void ReturnLyaoutSizeEqualPuttedRectangle_AfterFirstRectanglePutted(int width, int height)
        {
            var expectedSize = new Size(width, height);
            var layouter = new CircularCloudLayouter(Point.Empty);
            var rectangles = Enumerable.Repeat(layouter.PutNextRectangle(expectedSize), 1).ToArray();
            var bitmap = CloudLayoutRender.RenderImage(rectangles);
            bitmap.Size.ShouldBeEquivalentTo(expectedSize);
        }
    }
}
