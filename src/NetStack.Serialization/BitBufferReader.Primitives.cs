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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool PeekBool()
        {
            var tmp1 = scratchUsedBits;
            var tmp2 = chunkIndex;
            var tmp3 = scratch;
            var result = b();
            scratchUsedBits = tmp1;
            chunkIndex = tmp2;
            scratch = tmp3;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public u8 u8() => (u8)raw(8);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public u8 u8(int numberOfBits) => (u8)u32(numberOfBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public u8 u8(u8 min, u8 max) => (u8)u32(min, max);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public u8 u8Peek()
        {
            var tmp = scratch;
            var tmp2 = scratchUsedBits;
            var tmp3 = chunkIndex;
            var result = (u8)raw(8);
            tmp = scratch;
            tmp2 = scratchUsedBits;
            tmp3 = chunkIndex;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public u8 u8Peek(i32 numberOfBits) => (u8)u32Peek(numberOfBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public u8 u8Peek(u8 min, u8 max) => (u8)u32Peek(min, max);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public i8 i8() => (i8)i32(8);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public i8 i8(int numberOfBits) => (i8)i32(numberOfBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public i8 i8(i8 min, i8 max) => (i8)i32(min, max);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public i8 i8Peek() => (i8)raw(8);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public i8 i8Peek(int numberOfBits) => (i8)i32Peek(numberOfBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public i8 i8Peek(i8 min, i8 max) => (i8)PeekInt(min, max);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public i16 i16() => (i16)i32();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public i16 i16(i32 numberOfBits) => (i16)i32(numberOfBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public i16 i16(i16 min, i16 max) => (i16)i32(min, max);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public i16 i16Peek() => (i16)i32Peek();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public i16 i16Peek(i32 numberOfBits) => (i16)i32Peek(numberOfBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public i16 i16Peek(i16 min, i16 max) => (i16)PeekInt(min, max);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public u16 u16() => (u16)u32();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public u16 u16(int numberOfBits) => (u16)u32(numberOfBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public u16 u16(u16 min, u16 max) => (u16)u32(min, max);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public u16 u16Peek() => (u16)u32Peek();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public u16 u16Peek(i32 numberOfBits) => (u16)u32Peek(numberOfBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public u16 u16Peek(u16 min, u16 max) => (u16)u32Peek(min, max);

        /// <summary>
        /// Reads signed 32 bit integer.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int i32(int min, int max)
        {
#if DEBUG || NETSTACK_VALIDATE
            if (min > max) throw Argument("min should not be not lower than max");
#endif
            int bits = BitsRequired(min, max);

#if DEBUG || NETSTACK_VALIDATE
            if (BitsRead + bits > totalNumberBits) throw ArgumentOutOfRange("Reading too many bits for requested range");
#endif      
            return (int)(raw(bits) + min);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int PeekInt(int min, int max)
        {
#if DEBUG || NETSTACK_VALIDATE
            if (min > max) throw Argument("min should not be not lower than max");
#endif

            int bits = BitsRequired(min, max);
 #if DEBUG || NETSTACK_VALIDATE
            if (BitsRead + bits > totalNumberBits) throw ArgumentOutOfRange("Reading too many bits for requested range");
#endif     

            return (int)(raw(bits) + min);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public u32 u32(int numberOfBits) => raw(numberOfBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public u32 u32(u32 min, u32 max)
        {
#if DEBUG || NETSTACK_VALIDATE
            if (min > max) throw Argument("min should not be not lower than max");
#endif
            int bits = BitsRequired(min, max);
#if DEBUG || NETSTACK_VALIDATE
            if (BitsRead + bits > totalNumberBits) throw ArgumentOutOfRange("Reading too many bits for requested range");
#endif      
            return (raw(bits) + min);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public u32 u32Peek(int numberOfBits) => raw(numberOfBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public u32 u32Peek(u32 min, u32 max)
        {
#if DEBUG || NETSTACK_VALIDATE
            if (min > max) throw Argument("min should not be not lower than max");
#endif

            int bits = BitsRequired(min, max);
#if DEBUG || NETSTACK_VALIDATE
            if (BitsRead + bits > totalNumberBits) throw ArgumentOutOfRange("Reading too many bits for requested range");
#endif     

            return (raw(bits) + min);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long i64()
        {
            int low = i32();
            int high = i32();
            long value = high;
            return value << 32 | (u32)low;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long PeekLong()
        {
            long value = i64();
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong u64()
        {
            u32 low = u32();
            u32 high = u32();
            return (ulong)high << 32 | low;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong PeekULong()
        {
            ulong value = u64();
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public f32 f32()
        {
            var value = raw(32);
            return Unsafe.As<u32, f32>(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public f32 f32(f32 min, f32 max, f32 precision)
        {
            f32 range = max - min;
            f32 invPrecision = 1.0f / precision;
            f32 maxVal = range * invPrecision;
            int numberOfBits = BitOperations.Log2((u32)(maxVal + 0.5f)) + 1;

            return raw(numberOfBits) * precision + min;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public f32 f32(f32 min, f32 max, int numberOfBits)
        {
            var maxvalue = (1 << numberOfBits) - 1;
            f32 range = max - min;
            var precision = range / maxvalue;

            return raw(numberOfBits) * precision + min;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public f32 f32Peek(f32 min, f32 max, f32 precision)
        {
            f32 range = max - min;
            f32 invPrecision = 1.0f / precision;
            f32 maxVal = range * invPrecision;
            int numberOfBits = BitOperations.Log2((u32)(maxVal + 0.5f)) + 1;

            return raw(numberOfBits) * precision + min;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public f32 f32Peek(f32 min, f32 max, int numberOfBits)
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
        public f64 f64()
        {
            var value = u64();
            return Unsafe.As<ulong, f64>(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public f64 f64Peek()
        {
            var value = PeekULong();
            return Unsafe.As<ulong, f64>(ref value);
        }


    }
}