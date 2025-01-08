using System;

namespace Utility
{
    public struct VectorFloat : IEquatable<VectorFloat>
    {
        public float x { get; set; }
        public float y { get; set; }

        public VectorFloat(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static VectorFloat operator +(VectorFloat v1, VectorFloat v2)
        {
            return new VectorFloat(v1.x + v2.x, v1.y + v2.y);
        }

        public static VectorFloat operator -(VectorFloat v1, VectorFloat v2)
        {
            return new VectorFloat(v1.x - v2.x, v1.y - v2.y);
        }
        
        public static VectorFloat operator -(VectorFloat v1)
        {
            return new VectorFloat(-v1.x, -v1.y);
        }

        public static VectorFloat operator *(VectorFloat v, int scalar)
        {
            return new VectorFloat(v.x * scalar, v.y * scalar);
        }

        public static VectorFloat operator *(int scalar, VectorFloat v)
        {
            return new VectorFloat(v.x * scalar, v.y * scalar);
        }

        public static VectorFloat operator *(VectorFloat v, float scalar)
        {
            return new VectorFloat((int)(v.x * scalar), (int)(v.y * scalar));
        }

        public static VectorFloat operator *(float scalar, VectorFloat v)
        {
            return new VectorFloat((int)(v.x * scalar), (int)(v.y * scalar));
        }
        
        public float Magnitude()
        {
            return (float)Math.Sqrt(x * x + y * y);
        }

        public float SqrMagnitude()
        {
            return x * x + y * y;
        }

        public float Dot(VectorFloat v)
        {
            return x * v.x + y * v.y;
        }

        public float Cross(VectorFloat v)
        {
            return x * v.y - y * v.x;
        }

        public float AngleTo(VectorFloat v)
        {
            return (float)Math.Atan2(y - v.y, x - v.x);
        }

        public VectorFloat Normalize()
        {
            float magnitude = Magnitude();
            return new VectorFloat(x / magnitude, y / magnitude);
        }
        
        public static VectorFloat operator /(VectorFloat v, int scalar)
        {
            if (scalar == 0)
            {
                throw new DivideByZeroException("Division by zero is not allowed.");
            }
            return new VectorFloat(v.x / scalar, v.y / scalar);
        }
        
        public static VectorFloat operator /(VectorFloat v, float scalar)
        {
            if (scalar == 0)
            {
                throw new DivideByZeroException("Division by zero is not allowed.");
            }
            return new VectorFloat((int)(v.x / scalar), (int)(v.y / scalar));
        }

        public override string ToString()
        {
            return $"({x}, {y})";
        }

        public bool Equals(VectorFloat other)
        {
            return x == other.x && y == other.y;
        }

        public override bool Equals(object obj)
        {
            return obj is VectorFloat other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, y);
        }
    }
}