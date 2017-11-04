using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using FluentAssertions;
using NUnit.Framework;

namespace TagsCloudVisualization
{
    public class CloudLayoutRender
    {
        public static Bitmap RenderImage(ICollection<Rectangle> rectangles)
        {
            var bounds = GetBounds(rectangles);

            if (bounds.Width <= 0 || bounds.Height <= 0 || bounds.IsEmpty)
                throw new ArgumentException("Layout must contains elements");

            var bitmap = new Bitmap(bounds.Width, bounds.Height);
            var graphics = Graphics.FromImage(bitmap);
            foreach (var rectangle in rectangles)
            {
                graphics.FillRectangle(Brushes.CadetBlue, rectangle);
                graphics.DrawRectangle(Pens.Black, rectangle);
            }
            return bitmap;
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
