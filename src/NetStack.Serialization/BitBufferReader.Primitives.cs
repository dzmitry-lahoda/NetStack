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
        public byte u8() => (byte)raw(8);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte u8(int numberOfBits) => (byte)u32(numberOfBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public u8 u8(u8 min, u8 max) => (u8)u32(min, max);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte u8Peek()
        {
            var tmp = scratch;
            var tmp2 = scratchUsedBits;
            var tmp3 = chunkIndex;
            var result = (byte)raw(8);
            tmp = scratch;
            tmp2 = scratchUsedBits;
            tmp3 = chunkIndex;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte u8Peek(i32 numberOfBits) => (byte)u32Peek(numberOfBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte u8Peek(byte min, byte max) => (byte)u32Peek(min, max);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sbyte i8() => (sbyte)i32(8);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sbyte i8(int numberOfBits) => (sbyte)i32(numberOfBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sbyte i8(sbyte min, sbyte max) => (sbyte)i32(min, max);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sbyte i8Peek() => (sbyte)raw(8);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sbyte i8Peek(int numberOfBits) => (sbyte)i32Peek(numberOfBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sbyte i8Peek(sbyte min, sbyte max) => (sbyte)PeekInt(min, max);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public i16 i16() => (i16)i32();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public i16 i16(i32 numberOfBits) => (i16)i32(numberOfBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public i16 i16(i16 min, i16 max) => (i16)i32(min, max);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short i16Peek() => (i16)i32Peek();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short i16Peek(i32 numberOfBits) => (i16)i32Peek(numberOfBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short i16Peek(i16 min, i16 max) => (i16)PeekInt(min, max);

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
            Debug.Assert(min < max, "minus is not lower than max");

            int bits = BitsRequired(min, max);
            Debug.Assert(bits < totalNumberBits, "reading too many bits for requested range");

            return (int)(raw(bits) + min);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint u32(int numberOfBits) => raw(numberOfBits);

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
            Debug.Assert(min < max, "minus is not lower than max");

            int bits = BitsRequired(min, max);
            Debug.Assert(bits < totalNumberBits, "reading too many bits for requested range");

            return (raw(bits) + min);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long i64()
        {
            int low = i32();
            int high = i32();
            long value = high;
            return value << 32 | (uint)low;
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
            uint low = u32();
            uint high = u32();
            return (ulong)high << 32 | low;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong PeekULong()
        {
            ulong value = u64();
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float f32()
        {
            var value = raw(32);
            return Unsafe.As<uint, float>(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float f32(float min, float max, float precision)
        {
            float range = max - min;
            float invPrecision = 1.0f / precision;
            float maxVal = range * invPrecision;
            int numberOfBits = BitOperations.Log2((uint)(maxVal + 0.5f)) + 1;

            return raw(numberOfBits) * precision + min;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float f32(float min, float max, int numberOfBits)
        {
            var maxvalue = (1 << numberOfBits) - 1;
            float range = max - min;
            var precision = range / maxvalue;

            return raw(numberOfBits) * precision + min;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float PeekFloat(float min, float max, float precision)
        {
            float range = max - min;
            float invPrecision = 1.0f / precision;
            float maxVal = range * invPrecision;
            int numberOfBits = BitOperations.Log2((uint)(maxVal + 0.5f)) + 1;

            return raw(numberOfBits) * precision + min;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float PeekFloat(float min, float max, int numberOfBits)
        {
            var maxvalue = (1 << numberOfBits) - 1;
            float range = max - min;
            var precision = range / maxvalue;

            return raw(numberOfBits) * precision + min;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float PeekFloat()
        {
            var value = raw(32);
            return Unsafe.As<uint, float>(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double f64()
        {
            var value = u64();
            return Unsafe.As<ulong, double>(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double PeekDouble()
        {
            var value = PeekULong();
            return Unsafe.As<ulong, double>(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadByteArray(byte[] outValue) => ReadByteArray(outValue, out var length, 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadByteArray(byte[] outValue, out int length) => ReadByteArray(outValue, out length, 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadByteArray(byte[] outValue, out int length, int offset)
        {
            // may throw here consider array to be non one or couple of elements, but larger - not hot path
            Debug.Assert(outValue != null, "Supplied bytearray is null");

            length = (int)raw(config.ByteArrLengthBits);

            //Debug.Assert(BitsPassed2 - bitsRead <= length * 8, "The length for this read is bigger than bitbuffer");
            Debug.Assert(length <= outValue.Length + offset, "The supplied byte array is too small for requested read");

            for (int index = offset; index < length; index++)
            {
                outValue[index] = u8();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int PeekByteArrayLength() => (i32)raw(config.ByteArrLengthBits);
    }
}