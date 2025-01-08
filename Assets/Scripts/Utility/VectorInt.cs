using System;

namespace Utility
{
    public struct VectorInt : IEquatable<VectorInt>
    {
        public int x { get; set; }
        public int y { get; set; }

        public VectorInt(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static VectorInt operator +(VectorInt v1, VectorInt v2)
        {
            return new VectorInt(v1.x + v2.x, v1.y + v2.y);
        }

        public static VectorInt operator -(VectorInt v1, VectorInt v2)
        {
            return new VectorInt(v1.x - v2.x, v1.y - v2.y);
        }
        
        public static VectorInt operator -(VectorInt v1)
        {
            return new VectorInt(-v1.x, -v1.y);
        }

        public static VectorInt operator *(VectorInt v, int scalar)
        {
            return new VectorInt(v.x * scalar, v.y * scalar);
        }

        public static VectorInt operator *(int scalar, VectorInt v)
        {
            return new VectorInt(v.x * scalar, v.y * scalar);
        }

        public float Magnitude()
        {
            return (float)Math.Sqrt(x * x + y * y);
        }

        public float SqrMagnitude()
        {
            return x * x + y * y;
        }

        public float Dot(VectorInt v)
        {
            return x * v.x + y * v.y;
        }

        public float Cross(VectorInt v)
        {
            return x * v.y - y * v.x;
        }

        public float AngleTo(VectorInt v)
        {
            return (float)Math.Atan2(y - v.y, x - v.x);
        }

        public static VectorInt operator /(VectorInt v, int scalar)
        {
            if (scalar == 0)
            {
                throw new DivideByZeroException("Division by zero is not allowed.");
            }
            return new VectorInt(v.x / scalar, v.y / scalar);
        }

        public override string ToString()
        {
            return $"({x}, {y})";
        }

        public bool Equals(VectorInt other)
        {
            return x == other.x && y == other.y;
        }

        public override bool Equals(object obj)
        {
            return obj is VectorInt other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, y);
        }
    }
}