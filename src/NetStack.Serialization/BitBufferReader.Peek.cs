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
    partial class BitBufferReader<T>
    {
        /// <summary>
        /// Reads int, but does not move cursor.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public i32 i32Peek()
        {
            T encoder = default;
            return encoder.i32(this);
        }

        /// <summary>
        /// Reads uint, but does not move cursor.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public u32 u32Peek()
        {
            u32 value = u32();
            return value;
        }

        /// <summary>
        /// Reads i32 value without progressing bits position.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public i32 i32Peek(i32 numberOfBits)
        {
            T encoder = default;
            u32 value = raw(numberOfBits);
            return encoder.decode(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public u8 u8Peek()
        {
            var index = SIndex;
            var value = (u8)raw(8);
            SIndex = index;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public u8 u8Peek(i32 numberOfBits)
        {
            var index = SIndex;
            var value = (u8)u32Peek(numberOfBits);
            SIndex = index;
            return value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public u8 u8Peek(u8 min, u8 max)
        {
            var index = SIndex;
            var value = (u8)u32Peek(min, max);
            SIndex = index;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public i8 i8Peek() => (i8)raw(8);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public i8 i8Peek(i32 numberOfBits) => (i8)i32Peek(numberOfBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public i8 i8Peek(i8 min, i8 max) => (i8)i32Peek(min, max);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public i16 i16Peek() => (i16)i32Peek();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public i16 i16Peek(i32 numberOfBits) => (i16)i32Peek(numberOfBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public i16 i16Peek(i16 min, i16 max) => (i16)i32Peek(min, max);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public u16 u16Peek() => (u16)u32Peek();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public u16 u16Peek(i32 numberOfBits) => (u16)u32Peek(numberOfBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public u16 u16Peek(u16 min, u16 max) => (u16)u32Peek(min, max);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool bPeek()
        {
            var index = SIndex;
            var result = b();
            SIndex = index;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public i32 i32Peek(i32 min, i32 max)
        {
#if DEBUG || NETSTACK_VALIDATE
            if (min > max) throw Argument("min should not be not lower than max");
#endif

            i32 bits = BitsRequired(min, max);
#if DEBUG || NETSTACK_VALIDATE
            if (BitsRead + bits > totalNumberBits) throw ArgumentOutOfRange("Reading too many bits for requested range");
#endif

            return (i32)(raw(bits) + min);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public u32 u32Peek(i32 numberOfBits) => raw(numberOfBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public u32 u32Peek(u32 min, u32 max)
        {
#if DEBUG || NETSTACK_VALIDATE
            if (min > max) throw Argument("min should not be not lower than max");
#endif

            i32 bits = BitsRequired(min, max);
#if DEBUG || NETSTACK_VALIDATE
            if (BitsRead + bits > totalNumberBits) throw ArgumentOutOfRange("Reading too many bits for requested range");
#endif     

            return (raw(bits) + min);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public i64 i64Peek()
        {
            i64 value = i64();
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public u64 u64Peek()
        {
            u64 value = u64();
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public f32 f32Peek(f32 min, f32 max, f32 precision)
        {
            f32 range = max - min;
            f32 invPrecision = 1.0f / precision;
            f32 maxVal = range * invPrecision;
            i32 numberOfBits = BitOperations.Log2((u32)(maxVal + 0.5f)) + 1;

            return raw(numberOfBits) * precision + min;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public f32 f32Peek(f32 min, f32 max, i32 numberOfBits)
        {
            var maxvalue = (1 << numberOfBits) - 1;
            f32 range = max - min;
            var precision = range / maxvalue;

            return raw(numberOfBits) * precision + min;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public f32 f32Peek()
        {
            var value = raw(32);
            return Unsafe.As<u32, f32>(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public f64 f64Peek()
        {
            var value = u64Peek();
            return Unsafe.As<u64, f64>(ref value);
        }
    }
}