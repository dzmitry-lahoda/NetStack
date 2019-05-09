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
using System.Diagnostics;
using System.Numerics;

namespace NetStack.Serialization
{
    public interface IRawReader
    {
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        u32 u32(i32 numberOfBits);

        // reads raw number of bits, not more than 16
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        u16 u16(u8 numberOfBits);

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        bool b();

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        u8 u8Raw();
    }
}