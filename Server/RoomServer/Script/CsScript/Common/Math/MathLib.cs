using System;

namespace MathLib
{
    public class Mathf
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

        public static float Acos(float cos)
        {
            return (float)System.Math.Acos((double)cos);
        }

        public static float Sin(float rad)
        {
            return (float)System.Math.Sin((double)rad);
        }

        public static float Cos(float rad)
        {
            return (float)System.Math.Cos((double)rad);
        }

        public static float RandRange(float min, float max)
        {
            int iMin = (int)(min * 1000.0f);
            int iMax = (int)(max * 1000.0f);
            int r = random.Next(iMin, iMax);
            return (float)r * 0.001f;
        }

        public static int RandRange(int min, int max)
        {
            return random.Next(min, max);
        }

        public const float Rad2Deg = 57.29578f;
        public const float Deg2Rad = 0.01745329f;
        public const float PI = 3.141593f;
        private static Random random = new Random();
    }
}