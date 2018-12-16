using System;

namespace MathLib
{
    public class Matrix4x4
    {
        public Matrix4x4()
        {
            for (int i = 0; i < 4; ++i)
            {
                for (int j = 0; j < 4; ++j)
                {
                    if (i == j)
                    {
                        this.m[i, j] = 1.0f;
                        continue;
                    }
                    this.m[i, j] = 0.0f;
                }
            }
        }

        public Matrix4x4(float[,] m)
        {
            for (int i = 0; i < 4; ++i)
            {
                for (int j = 0; j < 4; ++j)
                {
                    this.m[i, j] = m[i, j];
                }
            }
        }

        public static Matrix4x4 operator -(Matrix4x4 v)
        {
            for (int i = 0; i < 4; ++i)
            {
                for (int j = 0; j < 4; ++j)
                {
                    v.m[i, j] = -v.m[i, j];
                }
            }
            return new Matrix4x4(v.m);
        }

        public static Matrix4x4 operator -(Matrix4x4 a, Matrix4x4 b)
        {
            float[,] data = new float[4, 4];
            for (int i = 0; i < 4; ++i)
            {
                for (int j = 0; j < 4; ++j)
                {
                    data[i, j] = a.m[i, j] - b.m[i, j];
                }
            }
            return new Matrix4x4(data);
        }



        float[,] m = new float[4, 4];
    }
}
