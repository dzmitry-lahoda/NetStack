using System;
using System.Runtime.CompilerServices;
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
#if !(ENABLE_MONO || ENABLE_IL2CPP)
using System.Diagnostics;
using System.Numerics;
#else
using UnityEngine;
#endif

namespace NetStack.Serialization
{
    public interface ICompression<T> where T : IRawWriter
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void u32(T b, u32 value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void i32(T b, i32 value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        u32 encode(i32 value);
    }

    public interface IDecompression<T> where T : IRawReader
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        u32 u32(T b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]


        i32 i32(T b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        i32 decode(u32 value);
    }
}