using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace TagsCloudVisualization
{
    [TestFixture]
    public class CircularCloudLayouter_Should
    {
        private CircularCloudLayouter layouter;

        [SetUp]
        public void TestSetup()
        {
            layouter= new CircularCloudLayouter(Point.Empty);
        }

        [TestCaseSource(typeof(CircularCloudLayouter_Should), nameof(CorrectSizesWithPointsCases))]
        public void ReturnRectangleWithInputtedSize_WhenPutNextRectangle(int width, int height, int x, int y)
        {
            layouter.SetCenter(new Point(x,y));
            var expectedSize = new Size(width, height);
            var actualRectangle = layouter.PutNextRectangle(expectedSize);
            actualRectangle.Size.Should().Be(expectedSize);
        }

        [TestCase(2, 2, 10, TestName = "Layout10Squares"), Timeout(2000)]
        [TestCase(10, 3, 10, TestName = "Layout10RectanglesStridedByX")]
        [TestCase(3, 10, 10, TestName = "Layout10RectanglesStridedByY")]
        [TestCase(1, 10, 10, TestName = "Layout10RectanglesWithWidthEqual1")]
        [TestCase(1, 10, 10, TestName = "Layout10RectanglesWithHeightEqual1")]
        [TestCase(1, 1, 10, TestName = "Layout10SquaresWithSideEqual1")]
        public void ReturnNotIntersectedRectangles(int width, int height, int count)
        {
            var sizes = Enumerable.Repeat(new Size(width, height), count);
            var rectangles = sizes.Select(layouter.PutNextRectangle).ToArray();
            foreach (var testingRectangle in rectangles) //very slow
            {
                Console.WriteLine(testingRectangle.Location);
                foreach (var rectangle in rectangles.Where(rect => !rect.Equals(testingRectangle)))
                {
                    testingRectangle.IntersectsWith(rectangle).Should().BeFalse();

                }
            }
        }

        [TestCaseSource(typeof(CircularCloudLayouter_Should), nameof(NegativeAndEmptySizesCases))]
        public void ThrowArgumentException_WhenSizeIsNegativeOrEmpty(int width, int height)
        {
            Action putNextRectangle = () => layouter.PutNextRectangle(new Size(width, height));
            putNextRectangle.ShouldThrow<ArgumentException>();
        }

        public static IEnumerable CorrectSizesWithPointsCases
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

        public static IEnumerable NegativeAndEmptySizesCases
        {
            get
            {
                return CartesianProduct(-1, 2)
                    .Select(digits => new Size(digits.Item1, digits.Item2))
                    .Select(size => new TestCaseData(size.Width, size.Height).SetName($"Size : {size}"));
            }
        }

        private static IEnumerable<(int, int)> CartesianProduct(int start, int count) =>
            CartesianProduct((start, count), (start, count));

        private static IEnumerable<(int, int)> CartesianProduct((int start, int count) firstRange,
            (int start, int count) secondRange) =>
            Enumerable.Range(firstRange.start, firstRange.count)
                .SelectMany(x => Enumerable.Range(firstRange.start, firstRange.count), (x, y) => (x, y));

        [TearDown]
        public void SaveLayoutOnFailure()
        {
            var testResult = TestContext.CurrentContext.Result.Outcome;
            if (!testResult.Equals(ResultState.Failure) &&
                !testResult.Equals(ResultState.Cancelled) &&
                !testResult.Equals(ResultState.Error) ) return;

            var layoutBitmap = CloudLayoutRender.RenderImage(layouter.Layout.ToArray());
            var path = $"{Environment.CurrentDirectory}/{TestContext.CurrentContext.Test.Name}.bmp";
            layoutBitmap.Save(path);
            TestContext.Out.WriteLine($"Tag cloud visualization saved to file: {path}");
        }
    }
}