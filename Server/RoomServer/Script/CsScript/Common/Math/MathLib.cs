using System;

namespace MathLib
{
    public class Math
    {
        public static float Max(float a, float b)
        {
            return a > b ? a : b;
        }

        public static float Min(float a, float b)
        {
            return a < b ? a : b;
        }

        public static float Sqrt(float f)
        {
            return (float)System.Math.Sqrt((double)f);
        }

        public static float Floor(float f)
        {
            return (float)System.Math.Floor((double)f);
        }

        public static float Ceil(float f)
        {
            return (float)System.Math.Ceiling((double)f);
        }

        public static int FloorToInt(float f)
        {
            return (int)System.Math.Floor((double)f);
        }

        public static int CeilToInt(float f)
        {
            return (int)System.Math.Ceiling((double)f);
        }

        public const float Rad2Deg = 57.29578f;
        public const float Deg2Rad = 0.01745329f;
        public const float PI = 3.141593f;
    }
}