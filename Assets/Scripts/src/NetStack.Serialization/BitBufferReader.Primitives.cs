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
    partial class BitBufferReader<T>
    {
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public u8 u8() => (u8)raw(8);

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public u8 u8(i32 numberOfBits) => (u8)u32(numberOfBits);

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public u8 u8(u8 min, u8 max) => (u8)u32(min, max);

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public i8 i8() => (i8)i32(8);

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public i8 i8(i32 numberOfBits) => (i8)i32(numberOfBits);

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public i8 i8(i8 min, i8 max) => (i8)i32(min, max);

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public i16 i16() => (i16)i32();

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public i16 i16(i32 numberOfBits) => (i16)i32(numberOfBits);

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public i16 i16(i16 min, i16 max) => (i16)i32(min, max);

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public u16 u16() => (u16)u32();

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public u16 u16(i32 numberOfBits) => (u16)u32(numberOfBits);

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public u16 u16(u16 min, u16 max) => (u16)u32(min, max);

        /// <summary>
        /// Reads signed 32 bit integer.
        /// </summary>
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public i32 i32(i32 min, i32 max)
        {
#if !NO_EXCEPTIONS
            if (min > max) Throw.Argument("min should not be not lower than max");
#endif
            i32 bits = BitsRequired(min, max);

#if !NO_EXCEPTIONS
            if (BitsRead + bits > state.totalNumberBits) Throw.ArgumentOutOfRange("Reading too many bits for requested range");
#endif      
            return (int)(raw(bits) + min);
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public u32 u32(i32 numberOfBits) => raw(numberOfBits);

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public u32 u32(u32 min, u32 max)
        {
#if !NO_EXCEPTIONS
            if (min > max) Throw.Argument("min should not be not lower than max");
#endif
            i32 bits = BitsRequired(min, max);
#if !NO_EXCEPTIONS
            if (BitsRead + bits > state.totalNumberBits) Throw.ArgumentOutOfRange("Reading too many bits for requested range");
#endif      
            return (raw(bits) + min);
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public i64 i64()
        {
            i32 low = i32();
            i32 high = i32();
            i64 value = high;
            return value << 32 | (u32)low;
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public u64 u64()
        {
            u32 low = u32();
            u32 high = u32();
            return (u64)high << 32 | low;
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public f32 f32()
        {
            var value = raw(32);
            return Unsafe.As<u32, f32>(ref value);
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public f32 f32(f32 min, f32 max, f32 precision)
        {
            f32 range = max - min;
            f32 invPrecision = 1.0f / precision;
            f32 maxVal = range * invPrecision;
            i32 numberOfBits = BitOperations.Log2((u32)(maxVal + 0.5f)) + 1;

            return raw(numberOfBits) * precision + min;
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public f32 f32(f32 min, f32 max, i32 numberOfBits)
        {
            var maxvalue = (1 << numberOfBits) - 1;
            f32 range = max - min;
            var precision = range / maxvalue;

            return raw(numberOfBits) * precision + min;
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public f64 f64()
        {
            var value = u64();
            return Unsafe.As<u64, f64>(ref value);
        }
    }
}