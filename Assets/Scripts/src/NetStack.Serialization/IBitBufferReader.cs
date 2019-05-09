using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;
using System.Numerics;

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

namespace NetStack.Serialization
{
    public interface IBitBufferReader : IRawReader
    {
        BitBufferOptions Options { get; }
        u32 BitsRead { get; }

        void SetPosition(u32 index);

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        u8 u8();

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        u8 u8(u8 min, u8 max);

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        i8 i8();

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        i8 i8(u8 numberOfBits);

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        i8 i8(i8 min, i8 max);

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        i16 i16();

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        i16 i16(u8 numberOfBits);

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        i16 i16(i16 min, i16 max);

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        u16 u16();

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        u16 u16(u16 min, u16 max);

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        i32 i32();

        /// <summary>
        /// Reads signed 32 bit integer.
        /// </summary>
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        i32 i32(i32 min, i32 max);

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        u32 u32(u32 min, u32 max);

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        u32 u32();

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        i64 i64();

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        u64 u64();

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        f32 f32();

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        f32 f32(f32 min, f32 max, f32 precision);

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        f32 f32(f32 min, f32 max, u8 numberOfBits);

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        f64 f64();
        
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        u32 c(Span<char> outputValue);
    }
}