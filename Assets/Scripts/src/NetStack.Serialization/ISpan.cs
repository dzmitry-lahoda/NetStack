using System;
using System.Runtime.InteropServices;
using System.Text;
using static System.Except;
using i8 = System.SByte;
using i16 = System.Int16;
using i32 = System.Int32;
using i64 = System.Int64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;
using f32 = System.Single;
using f64 = System.Double;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Numerics;

namespace NetStack.Serialization
{
    public interface ISpan<T>
    {
        T this[i32 index]
        {
            [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
            get;
            [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
            set;
        }

        i32 Length
        {
            [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
            get;
        }
    }

    public struct ArraySpan : ISpan<u32>
    {
        public u32[] chunks;

        public u32 this[i32 index]
        {
            [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
            get => chunks[index];
            [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
            set => chunks[index] = value;
        }

        public i32 Length
        {
            [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
            get => chunks.Length;
        }
    }

    public struct MemorySpan : ISpan<u32>
    {
        public Memory<u32> chunks;

        public u32 this[i32 index]
        {
            [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
            get => chunks.Span[index];
            [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
            set => chunks.Span[index] = value;
        }

        public i32 Length
        {
            [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
            get => chunks.Length;
        }
    }
}