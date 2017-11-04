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
        private readonly Vector2 center;

        public CircularCloudLayouter(Point center)
        {
            layout = new List<Rectangle>();
            this.center = center.ToVector();
        }

        public Rectangle PutNextRectangle(Size rectangleSize)
        {
            if (rectangleSize.Width <= 0 || rectangleSize.Height <= 0 || rectangleSize.IsEmpty)
                throw new ArgumentException("Size must be positive and not empty");

            var rectangle = new Rectangle(Point.Empty, rectangleSize);
            PlaceRectangleOnInvolute(rectangle);
            if (layout.Count > 0)
            {
                PressToCenter(rectangle);
            }
            layout.Add(rectangle);
            return rectangle;
        }

        private void PlaceRectangleOnInvolute(Rectangle rectangle)
        {
            var angle = 0.0f;
            while (IsIntersectLayout(rectangle))
            {
                rectangle.Location = (PointOnInvolute(angle, InvoluteRadius) - center).RoundToPoint();
                angle += InvoluteAngleStep;
            }
        }

        private void PressToCenter(Rectangle rectangle)
        {
            MoveToPointWhile(rectangle, center, () => !IsIntersectLayout(rectangle));
            MoveToPointWhile(rectangle, center * Vector2.UnitX, () => !IsIntersectLayout(rectangle));
            MoveToPointWhile(rectangle, center * Vector2.UnitY, () => !IsIntersectLayout(rectangle));
        }

        private void MoveToPointWhile(Rectangle rectangle, Vector2 point, Func<bool> predicate)
        {
            var translationVector = Vector2.Normalize(point - rectangle.GetCenter());
            var lastFreeLocation = rectangle.Location;
            while (predicate() && !rectangle.GetCenter().IsClose(center))
            {
                lastFreeLocation = rectangle.Location;
                rectangle.Translate(translationVector);
            }
            rectangle.Location = lastFreeLocation;
        }

        private bool IsIntersectLayout(Rectangle rectangle) =>
            layout.Any(rect => rect.IntersectsWith(rectangle));

        private static Vector2 PointOnInvolute(float angle, float radius) => new Vector2(
            (float) (radius * (Math.Cos(angle) + angle * Math.Sin(angle))),
            (float) (radius * (Math.Sin(angle) - angle * Math.Cos(angle))));
    }
}