using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace TagsCloudVisualization
{
    public class CircularCloudLayouter
    {
        private const float InvoluteRadius = 1;
        private const float InvoluteAngleStep = (float) (Math.PI / 20.0f);
        private readonly List<Rectangle> layout;
        private Vector2 center;

        public CircularCloudLayouter(Point center)
        {
            layout = new List<Rectangle>();
            this.center = center.ToVector();
        }

        public IEnumerable<Rectangle> Layout => layout;

        public void SetCenter(Point center)
        {
            this.center = center.ToVector();
        }

        public Rectangle PutNextRectangle(Size rectangleSize)
        {
            if (rectangleSize.Width <= 0 || rectangleSize.Height <= 0 || rectangleSize.IsEmpty)
                throw new ArgumentException("Size must be positive and not empty");

            var rectangle = new Rectangle(Point.Empty, rectangleSize);
            PlaceRectangleOnInvolute(ref rectangle);
            if (layout.Count > 0)
                PressToCenter(ref rectangle);
            layout.Add(rectangle);
            return rectangle;
        }

        private void PlaceRectangleOnInvolute(ref Rectangle rectangle)
        {
            var angle = 0.0f;
            while (IsIntersectLayout(rectangle))
            {
                rectangle.Location = (PointOnInvolute(angle, InvoluteRadius) - center).RoundToPoint();
                angle += InvoluteAngleStep;
            }
        }

        private void PressToCenter(ref Rectangle rectangle)
        {
            MoveToPointWhile(ref rectangle, center, 2, rect => !IsIntersectLayout(rect));
            MoveToPointWhile(ref rectangle, center * Vector2.UnitX + rectangle.GetCenter() * Vector2.UnitY, 1,
                rect => !IsIntersectLayout(rect));
            MoveToPointWhile(ref rectangle, center * Vector2.UnitY + rectangle.GetCenter() * Vector2.UnitX, 1,
                rect => !IsIntersectLayout(rect));
        }

        private static void MoveToPointWhile(ref Rectangle rectangle, Vector2 point, int step,
            Func<Rectangle, bool> predicate)
        {
            var translationVector = Vector2.Normalize(point - rectangle.GetCenter()) * step;
            var lastFreeLocation = rectangle.Location;
            while (predicate(rectangle) && Math.Abs((rectangle.GetCenter() - point).Length()) > step)
            {
                lastFreeLocation = rectangle.Location;
                rectangle.Location = (translationVector + rectangle.Location.ToVector()).RoundToPoint();
            }
            rectangle.Location = lastFreeLocation;
        }

        private bool IsIntersectLayout(Rectangle rectangle)
        {
            return layout.Any(rect => rect.IntersectsWith(rectangle));
        }

        private static Vector2 PointOnInvolute(float angle, float radius)
        {
            return new Vector2(
                (float) (radius * (Math.Cos(angle) + angle * Math.Sin(angle))),
                (float) (radius * (Math.Sin(angle) - angle * Math.Cos(angle))));
        }
    }
}