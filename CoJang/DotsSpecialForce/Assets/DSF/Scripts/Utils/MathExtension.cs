using Unity.Mathematics;
using UnityEngine;

namespace MathExtension
{
    public static class VectorExtension
    {
        #region Float Extensions

        public static Vector3 Convert(this float3 b)
        {
            return new Vector3(b.x, b.y, b.z);
        }

        public static Vector3 Add(this float3 a, Vector3 b)
        {
            return Convert(a) + b;
        }

        public static float3 Normalize(this float3 a)
        {
            return Convert(Convert(a).normalized);
        }

        public static Vector3 Normalize_ToVector3(this float3 a)
        {
            return Convert(a).normalized;
        }

        public static float2 float2_XZ(this float3 a)
        {
            return new float2(a.x, a.z);
        }

        public static float2 float2_XY(this float3 a)
        {
            return new float2(a.x, a.y);
        }

        public static float3 float3_XNZ(this float3 a, float N = 0)
        {
            return new float3(a.x, N, a.z);
        }

        public static float3 float3_XYN(this float3 a, float N = 0)
        {
            return new float3(a.x, a.y, N);
        }

        #endregion


        #region Vector Extensions

        public static Vector2 Vector2_XZ(this Vector3 a)
        {
            return new Vector2(a.x, a.z);
        }

        public static Vector2 Vector2_XY(this Vector3 a)
        {
            return new Vector2(a.x, a.y);
        }

        public static float3 Convert(this Vector3 b)
        {
            return new float3(b.x, b.y, b.z);
        }

        public static float3 Add(this Vector3 a, float3 b)
        {
            return Convert(a) + b;
        }

        public static float2 float2_XZ(this Vector3 a)
        {
            return new float2(a.x, a.z);
        }

        public static float2 float2_XY(this Vector3 a)
        {
            return new float2(a.x, a.y);
        }

        public static float3 float3_XNZ(this Vector3 a, float N = 0)
        {
            return new float3(a.x, N, a.z);
        }

        public static float3 float3_XYN(this Vector3 a, float N = 0)
        {
            return new float3(a.x, a.y, N);
        }

        public static Vector3 Vector3_XNZ(this Vector3 a, float N = 0)
        {
            return new Vector3(a.x, N, a.z);
        }

        public static Vector3 Vector3_XYN(this Vector3 a, float N = 0)
        {
            return new Vector3(a.x, a.y, N);
        }

        #endregion


    }
}
