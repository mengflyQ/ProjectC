using System;

namespace MathLib
{
    public struct Vector4
    {
        public Vector4(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public Vector4(Vector3 v, float w)
        {
            this.x = v.x;
            this.y = v.y;
            this.z = v.z;
            this.w = w;
        }

        public static Vector4 operator -(Vector4 v)
        {
            return new Vector4(-v.x, -v.y, -v.z, v.w);
        }

        public static Vector4 operator -(Vector4 a, Vector4 b)
        {
            return new Vector4(a.x - b.x, a.y - b.y, a.z - b.z, a.w);
        }

        public static Vector4 operator +(Vector4 a, Vector4 b)
        {
            return new Vector4(a.x + b.x, a.y + b.y, a.z + b.z, a.w);
        }

        public static Vector4 operator *(Vector4 a, Vector4 b)
        {
            return new Vector4(a.x * b.x, a.y * b.y, a.z * b.z, a.w);
        }

        public static Vector4 operator *(Vector4 a, float f)
        {
            return new Vector4(a.x * f, a.y * f, a.z * f, a.w);
        }

        public static Vector4 operator *(float f, Vector4 a)
        {
            return new Vector4(a.x * f, a.y * f, a.z * f, a.w);
        }

        public static Vector4 operator /(Vector4 a, Vector4 b)
        {
            return new Vector4(a.x / b.x, a.y / b.y, a.z / b.z, a.w);
        }

        public static Vector4 operator /(Vector4 a, float f)
        {
            return new Vector4(a.x / f, a.y / f, a.z / f, a.w);
        }

        public static bool operator ==(Vector4 a, Vector4 b)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z && a.w == b.w;
        }

        public static bool operator !=(Vector4 a, Vector4 b)
        {
            return a.x != b.x || a.y != b.y || a.z != b.z || a.w != b.w;
        }

        public override bool Equals(object obj)
        {
            //if (obj == null)
            //    throw new NullReferenceException("Vector4");
            //Vector4 v = obj as Vector4;
            //return x == v.x && y == v.y && z == v.z && w == v.w;
            return false;
        }

        public override int GetHashCode()
        {
            return this.x.GetHashCode() ^ this.y.GetHashCode() ^ this.z.GetHashCode() ^ this.w.GetHashCode();
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
                w = 0.0f;
                return;
            }
            x /= len;
            y /= len;
            z /= len;
            w = 0.0f;
        }

        public void Set(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public float Dot(Vector4 v)
        {
            return x * v.x + y * v.y + z * v.z;
        }

        public static float Dot(Vector4 a, Vector4 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public Vector4 Cross(Vector4 v)
        {
            return new Vector4(y * v.z - z * v.y,
                z * v.x - x * v.z,
                x * v.y - y * v.x,
                0.0f);
        }

        public static Vector4 Cross(Vector4 a, Vector4 b)
        {
            return new Vector4(a.y * b.z - a.z * b.y,
                a.z * b.x - a.x * b.z,
                a.x * b.y - a.y * b.x,
                0.0f);
        }

        public Vector4 normalize
        {
            get
            {
                Vector4 v = new Vector4(x, y, z, w);
                v.Normalize();
                return v;
            }
        }

        public float x;
        public float y;
        public float z;
        public float w;
    }
}
