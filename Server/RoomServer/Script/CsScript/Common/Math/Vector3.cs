using System;

namespace MathLib
{
    public struct Vector3
    {
        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static Vector3 operator -(Vector3 v)
        {
            return new Vector3(-v.x, -v.y, -v.z);
        }

        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vector3 operator -(Vector3 a, float f)
        {
            return new Vector3(a.x - f, a.y - f, a.z - f);
        }

        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector3 operator +(Vector3 a, float f)
        {
            return new Vector3(a.x + f, a.y + f, a.z + f);
        }

        public static Vector3 operator *(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public static Vector3 operator *(Vector3 a, float f)
        {
            return new Vector3(a.x * f, a.y * f, a.z * f);
        }

        public static Vector3 operator *(float f, Vector3 a)
        {
            return new Vector3(a.x * f, a.y * f, a.z * f);
        }

        public static Vector3 operator /(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
        }

        public static Vector3 operator /(Vector3 a, float f)
        {
            return new Vector3(a.x / f, a.y / f, a.z / f);
        }

        public static bool operator ==(Vector3 a, Vector3 b)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }

        public static bool operator !=(Vector3 a, Vector3 b)
        {
            return a.x != b.x || a.y != b.y || a.z != b.z;
        }

        public static float Distance(Vector3 a, Vector3 b)
        {
            return (b - a).Length();
        }

        public override bool Equals(object obj)
        {
            //if (obj == null)
            //    throw new NullReferenceException("Vector3");
            //Vector3 v = obj as Vector3;
            //return x == v.x && y == v.y && z == v.z;
            return false;
        }

        public override int GetHashCode()
        {
            return this.x.GetHashCode() ^ this.y.GetHashCode() ^ this.z.GetHashCode();
        }

        public float Length()
        {
            return (float)System.Math.Sqrt(x * x + y * y + z * z);
        }

        public float LengthSquared()
        {
            return x * x + y * y + z * z;
        }

        public void Normalize()
        {
            float len = Length();
            if (len == 0.0f)
            {
                x = 0.0f;
                y = 0.0f;
                z = 0.0f;
                return;
            }
            x /= len;
            y /= len;
            z /= len;
        }

        public void Set(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public float Dot(Vector3 v)
        {
            return x * v.x + y * v.y + z * v.z;
        }

        public static float Dot(Vector3 a, Vector3 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public Vector3 Cross(Vector3 v)
        {
            return new Vector3(y * v.z - z * v.y,
                z * v.x - x * v.z,
                x * v.y - y * v.x);
        }

        public static Vector3 Cross(Vector3 a, Vector3 b)
        {
            return new Vector3(a.y * b.z - a.z * b.y,
                a.z * b.x - a.x * b.z,
                a.x * b.y - a.y * b.x);
        }

        public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
        {
            return new Vector3(
                a.x + (b.x - a.x) * t,
                a.y + (b.y - a.y) * t,
                a.z + (b.z - a.z) * t);
        }

        public Vector3 normalize
        {
            get
            {
                Vector3 v = new Vector3(x, y, z);
                v.Normalize();
                return v;
            }
        }

        public static Vector3 zero = new Vector3();
        public static Vector3 one = new Vector3(1.0f, 1.0f, 1.0f);
        public static Vector3 forward = new Vector3(0.0f, 0.0f, 1.0f);
        public static Vector3 up = new Vector3(0.0f, 1.0f, 0.0f);
        public static Vector3 right = new Vector3(1.0f, 0.0f, 0.0f);
        public float x;
        public float y;
        public float z;
    }
}
