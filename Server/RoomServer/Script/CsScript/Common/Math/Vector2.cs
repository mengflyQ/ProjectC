using System;

namespace MathLib
{
    public struct Vector2
    {
        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static Vector2 operator -(Vector2 v)
        {
            return new Vector2(-v.x, -v.y);
        }

        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x - b.x, a.y - b.y);
        }

        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x + b.x, a.y + b.y);
        }

        public static Vector2 operator *(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x * b.x, a.y * b.y);
        }

        public static Vector2 operator *(Vector2 a, float f)
        {
            return new Vector2(a.x * f, a.y * f);
        }

        public static Vector2 operator *(float f, Vector2 a)
        {
            return new Vector2(a.x * f, a.y * f);
        }

        public static Vector2 operator /(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x / b.x, a.y / b.y);
        }

        public static Vector2 operator /(Vector2 a, float f)
        {
            return new Vector2(a.x / f, a.y / f);
        }

        public static bool operator ==(Vector2 a, Vector2 b)
        {
            return a.x == b.x && a.y == b.y;
        }

        public static bool operator !=(Vector2 a, Vector2 b)
        {
            return a.x != b.x || a.y != b.y;
        }

        public override bool Equals(object obj)
        {
            //if (obj == null)
            //    throw new NullReferenceException("Vector2");
            //Vector2 v = obj as Vector2;
            //return x == v.x && y == v.y;
            return false;
        }

        public override int GetHashCode()
        {
            return this.x.GetHashCode() ^ this.y.GetHashCode();
        }

        public float Length()
        {
            return (float)System.Math.Sqrt((double)(x * x + y * y));
        }

        public float LengthSquared()
        {
            return x * x + y * y;
        }

        public void Normalize()
        {
            float len = Length();
            if (len == 0.0f)
                return;
            x /= len;
            y /= len;
        }

        public void Set(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public float Dot(Vector2 v)
        {
            return x * v.x + y * v.y;
        }

        public static float Dot(Vector2 a, Vector2 b)
        {
            return a.x * b.x + a.y * b.y;
        }

        public Vector2 normalize
        {
            get
            {
                Vector2 v = new Vector2(x, y);
                v.Normalize();
                return v;
            }
        }

        public static Vector2 zero = new Vector2();
        public static Vector2 one = new Vector2(1.0f, 1.0f);
        public float x;
        public float y;
    }
}
