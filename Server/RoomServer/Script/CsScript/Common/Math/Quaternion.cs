using System;

namespace MathLib
{
    public struct Quaternion
    {
        public Quaternion(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public Quaternion(Vector3 axis, float radian)
        {
            float fHalfAngle = 0.5f * radian;
	        float fSin = Mathf.Sin(fHalfAngle);
            w = Mathf.Cos(fHalfAngle);
	        x = fSin * axis.x;
	        y = fSin * axis.y;
	        z = fSin * axis.z;
        }

        public void ToAngleAxis(out Vector3 axis, out float radian)
        {
            axis = new Vector3();
            float fSqrLength = x * x + y * y + z * z;
            if (fSqrLength > 0.0)
            {
                radian = 2.0f * Mathf.Acos(w);
                float fInvLength = 1.0f / Mathf.Sqrt(fSqrLength);
                axis.x = x * fInvLength;
                axis.y = y * fInvLength;
                axis.z = z * fInvLength;
            }
            else
            {
                // angle is 0 (mod 2*pi), so any axis will do
                radian = 0.0f;
                axis.x = 1.0f;
                axis.y = 0.0f;
                axis.z = 0.0f;
            }
        }

        public static  Quaternion AngleAxis(float angle, Vector3 axis)
        {
            float radian = angle * Mathf.Deg2Rad;
            return new Quaternion(axis, radian);
        }

        public static Quaternion operator -(Quaternion q)
        {
            return new Quaternion(-q.x, -q.y, -q.z, -q.w);
        }

        public static Quaternion operator -(Quaternion a, Quaternion b)
        {
            return new Quaternion(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
        }

        public static Quaternion operator +(Quaternion a, Quaternion b)
        {
            return new Quaternion(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
        }

        public static Quaternion operator *(Quaternion a, Quaternion b)
        {
            return new Quaternion
                (
                a.w * b.x + a.x * b.w - a.y * b.z + a.z * b.y,
                a.w * b.y + a.x * b.z + a.y * b.w - a.z * b.x,
                a.w * b.z - a.x * b.y + a.y * b.x + a.z * b.w,
                a.w * b.w - a.x * b.x - a.y * b.y - a.z * b.z
                );
        }

        public static Quaternion operator *(Quaternion a, float scale)
        {
            return new Quaternion(a.x * scale, a.y * scale, a.z * scale, a.w * scale);
        }

        public static Quaternion operator *(float scale, Quaternion a)
        {
            return new Quaternion(a.x * scale, a.y * scale, a.z * scale, a.w * scale);
        }

        public static Vector3 operator *(Quaternion a, Vector3 v)
        {
            // nVidia SDK implementation
	        Vector3 uv, uuv; 
	        Vector3 qvec = new Vector3(a.x, a.y, a.z);
	        //! original: uv = Cross(qvec, v);
	        uv = Vector3.Cross(qvec,v);
	        uuv = Vector3.Cross(qvec,uv);
	        uv *= (2.0f * a.w);
	        uuv *= 2.0f; 

	        return v + uv + uuv;
        }

        public static bool operator ==(Quaternion a, Quaternion b)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }

        public static bool operator !=(Quaternion a, Quaternion b)
        {
            return a.x != b.x || a.y != b.y || a.z != b.z;
        }

        public override bool Equals(object obj)
        {
            //if (obj == null)
            //    throw new NullReferenceException("Quaternion");
            //Quaternion v = obj as Quaternion;
            //return x == v.x && y == v.y && z == v.z && w == v.w;
            return false;
        }

        public override int GetHashCode()
        {
            return this.x.GetHashCode() ^ this.y.GetHashCode() ^ this.z.GetHashCode() ^ this.w.GetHashCode();
        }

        public static Quaternion identity = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);
        float x;
        float y;
        float z;
        float w;
    }
}
