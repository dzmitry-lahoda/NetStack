using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BenchmarkDotNet;
using BenchmarkDotNet.Attributes;
using NetStack.Serialization;

namespace NetStack.Serialization
{
#if !(ENABLE_MONO || ENABLE_IL2CPP)
    [CoreJob]
#endif
    public class MathFAbs
    {
        [Params(100_000)]
        public int N;

        [Benchmark]
        public void MathAbs()
        {
            float x = 0;
            for (int i = 0; i < N; i++)
            {
                var f = (i % 2) - 1.0f;
                x = Math.Abs(f);
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct FastAbs
        {
            [FieldOffset(0)] public uint uint32;
            [FieldOffset(0)] public float single;
        }

        [Benchmark]
        public void CastViaStuct()
        {
            FastAbs a = default;
            float x = 0;
            for (int i = 0; i < N; i++)
            {
                var f = (i % 2) - 1.0f;
                a.single = f;
                a.uint32 &= 0x7FFFFFFF;
                x = a.single;
            }
        }

        [Benchmark]
        public void CastViaUnsafe()
        {
            float x = 0;
            for (int i = 0; i < N; i++)
            {
                var f = (i % 2) - 1.0f;
                var b = Unsafe.As<float, uint>(ref f);
                b &= 0x7FFFFFFF;
                x = Unsafe.As<uint, float>(ref b);
            }
        }   

       [Benchmark]
        public void CastViaRefUnsafe()
        {
            float x = 0;
            for (int i = 0; i < N; i++)
            {
                var f = (i % 2) - 1.0f;
                ref var b = ref Unsafe.As<float, uint>(ref f);
                b &= 0x7FFFFFFF;
                x = Unsafe.As<uint, float>(ref b);
            }
        }   

        [Benchmark]
        public void CastViaUnsafeWithReuse()
        {
            uint b = 0;
            float x = 0;
            for (int i = 0; i < N; i++)
            {
                var f = (i % 2) - 1.0f;
                b = Unsafe.As<float, uint>(ref f);
                b &= 0x7FFFFFFF;
                x = Unsafe.As<uint, float>(ref b);
            }
        }                 
    }
}



