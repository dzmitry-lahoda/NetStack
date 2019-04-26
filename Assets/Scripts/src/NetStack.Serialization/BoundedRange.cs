using System;
using System.Runtime.CompilerServices;
using NetStack.Serialization;

#if !(ENABLE_MONO || ENABLE_IL2CPP)
using System.Numerics;
#else
using UnityEngine;
#endif

// Until migration to .NET Standard 2.1
using BitOperations = System.Numerics.BitOperations;

namespace NetStack.Serialization
{
    public class BoundedRange
    {
        private readonly float minValue;
        private readonly float maxValue;
        private readonly float precision;
        private readonly int requiredBits;
        private readonly uint mask;

        public BoundedRange(float minValue, float maxValue, float precision)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.precision = precision;

            requiredBits = BitOperations.Log2((uint)((maxValue - minValue) * (1.0f / precision) + 0.5f)) + 1;
            mask = (uint)((1L << requiredBits) - 1);
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public uint Compress(float value)
        {
            if (value < minValue)
                value = minValue;
            else if (value > maxValue)
                value = maxValue;

            return (uint)((float)((value - minValue) * (1f / precision)) + 0.5f) & mask;
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public float Decompress(uint data)
        {
            float adjusted = ((float)data * precision) + minValue;

            if (adjusted < minValue)
                adjusted = minValue;
            else if (adjusted > maxValue)
                adjusted = maxValue;

            return adjusted;
        }

        public static CompressedVector2 Compress(in Vector2 vector2, BoundedRange[] boundedRange)
        {
            CompressedVector2 data = default;

#if ENABLE_MONO || ENABLE_IL2CPP
            data.x = boundedRange[0].Compress(vector2.x);
            data.y = boundedRange[1].Compress(vector2.y);
#else
				data.x = boundedRange[0].Compress(vector2.X);
				data.y = boundedRange[1].Compress(vector2.Y);
#endif

            return data;
        }

        public static CompressedVector3 Compress(in Vector3 vector3, BoundedRange[] boundedRange)
        {
            CompressedVector3 data = default;

#if ENABLE_MONO || ENABLE_IL2CPP
            data.x = boundedRange[0].Compress(vector3.x);
            data.y = boundedRange[1].Compress(vector3.y);
            data.z = boundedRange[2].Compress(vector3.z);
#else
				data.x = boundedRange[0].Compress(vector3.X);
				data.y = boundedRange[1].Compress(vector3.Y);
				data.z = boundedRange[2].Compress(vector3.Z);
#endif

            return data;
        }

        public static CompressedVector4 Compress(in Vector4 vector4, BoundedRange[] boundedRange)
        {
            CompressedVector4 data = default;

#if ENABLE_MONO || ENABLE_IL2CPP
            data.x = boundedRange[0].Compress(vector4.x);
            data.y = boundedRange[1].Compress(vector4.y);
            data.z = boundedRange[2].Compress(vector4.z);
            data.w = boundedRange[3].Compress(vector4.w);
#else
				data.x = boundedRange[0].Compress(vector4.X);
				data.y = boundedRange[1].Compress(vector4.Y);
				data.z = boundedRange[2].Compress(vector4.Z);
				data.w = boundedRange[3].Compress(vector4.W);
#endif

            return data;
        }

        public static Vector2 Decompress(in CompressedVector2 data, BoundedRange[] boundedRange) => 
            new Vector2(boundedRange[0].Decompress(data.x), boundedRange[1].Decompress(data.y));

        public static Vector3 Decompress(in CompressedVector3 data, BoundedRange[] boundedRange) => 
            new Vector3(boundedRange[0].Decompress(data.x), boundedRange[1].Decompress(data.y), boundedRange[2].Decompress(data.z));

        public static Vector4 Decompress(in CompressedVector4 data, BoundedRange[] boundedRange) => 
            new Vector4(boundedRange[0].Decompress(data.x), boundedRange[1].Decompress(data.y), boundedRange[2].Decompress(data.z), boundedRange[3].Decompress(data.w));
    }
}