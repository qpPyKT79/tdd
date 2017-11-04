using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace TagsCloudVisualization
{
    [TestFixture]
    public class CircularCloudLayouter_Should
    {
        private static IEnumerable<(int, int)> CartesianProduct(int start, int count) =>
            CartesianProduct((start, count), (start, count));

        private static IEnumerable<(int, int)> CartesianProduct((int start, int count) firstRange,
            (int start, int count) secondRange) =>
            Enumerable.Range(firstRange.start, firstRange.count)
                .SelectMany(x => Enumerable.Range(firstRange.start, firstRange.count), (x, y) => (x, y));

        public static IEnumerable CorrectSizesWithPoints
        {
            get
            {
                var sizes = CartesianProduct(1, 2).Select(digits => new Size(digits.Item1, digits.Item2))
                    .Where(size => !size.IsEmpty);
                var points = CartesianProduct(-1, 3).Select(digits => new Point(digits.Item1, digits.Item2));
                return points.SelectMany(point => sizes,
                    (point, size) =>
                        new TestCaseData(size.Width, size.Height, point.X, point.Y).SetName(
                            $"Point : {point} | Size : {size}"));
            }
        }

        public static IEnumerable NegativeAndEmptySizes
        {
            get
            {
                return CartesianProduct(-1, 2)
                    .Select(digits => new Size(digits.Item1, digits.Item2))
                    .Select(size => new TestCaseData(size.Width, size.Height).SetName($"Size : {size}"));
            }
        }

        [TestCaseSource(typeof(CircularCloudLayouter_Should), nameof(CorrectSizesWithPoints))]
        public void ReturnRectangleWithInputtedSize_WhenPutNextRectangle(int width, int height, int x, int y)
        {
            var layouter = new CircularCloudLayouter(new Point(x, y));
            var expectedSize = new Size(width, height);
            var actualRectangle = layouter.PutNextRectangle(expectedSize);
            actualRectangle.Size.Should().Be(expectedSize);
        }

        [TestCase(2, 2, 10)]
        [TestCase(10, 3, 10)]
        [TestCase(3, 10, 10)]
        public void ReturnNotIntersectedRectangles(int width, int height, int count)
        {
            var layouter = new CircularCloudLayouter(Point.Empty);
            var sizes = Enumerable.Repeat(new Size(width, height), count);
            var rectangles = sizes.Select(layouter.PutNextRectangle).ToArray();
            foreach (var testingRectangle in rectangles) //very slow
            {
                foreach (var rectangle in rectangles.Where(rect => !rect.Equals(testingRectangle)))
                {
                    testingRectangle.IntersectsWith(rectangle).Should().BeFalse();
                }
            }
        }

        [TestCaseSource(typeof(CircularCloudLayouter_Should), nameof(NegativeAndEmptySizes))]
        public void ThrowArgumentException_WhenSizeIsNegativeOrEmpty(int width, int height)
        {
            var layouter = new CircularCloudLayouter(Point.Empty);
            Action putNextRectangle = () => layouter.PutNextRectangle(new Size(width, height));
            putNextRectangle.ShouldThrow<ArgumentException>();
        }
    }
}