using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace Astroshooter
{
    public class Vec2
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Vec2()
        {

        }

        public Vec2(double x, double y)
        {
            X = x;
            Y = y;
        }

        static public PointF TransformToPointF(Vec2 vector)
        {
            return new PointF((float)vector.X, (float)vector.Y);
        }

        static public double GetDistanceBetween(Vec2 left, Vec2 right)
        {
            var dx = left.X - right.X;
            var dy = left.Y - right.Y;
            return Math.Sqrt(dx*dx + dy*dy);
        }

        public static Vec2 operator + (Vec2 left, Vec2 right) => new Vec2(left.X + right.X, left.Y + right.Y);

        public static Vec2 operator *(Vec2 left, double multiplier) => new Vec2(left.X * multiplier, left.Y * multiplier);
    }
}
