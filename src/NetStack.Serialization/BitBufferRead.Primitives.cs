using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
#if !(ENABLE_MONO || ENABLE_IL2CPP)
using System.Diagnostics;
using System.Numerics;
#else
using UnityEngine;
#endif

namespace NetStack.Serialization
{
    partial class BitBufferRead
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool PeekBool()
        {
            var tmp1 = scratchUsedBits;
            var tmp2 = chunkIndex;
            var tmp3 = scratch;
            var result = @bool();
            scratchUsedBits = tmp1;
            chunkIndex = tmp2;
            scratch = tmp3;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte u8() => (byte)raw(8);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte u8(int numberOfBits) => (byte)ReadUInt(numberOfBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte u8(byte min, byte max) => (byte)ReadUInt(min, max);

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
        public byte u8Peek(int numberOfBits) => (byte)PeekUInt(numberOfBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte u8Peek(byte min, byte max) => (byte)PeekUInt(min, max);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sbyte ReadSByte() => (sbyte)i32(8);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sbyte ReadSByte(int numberOfBits) => (sbyte)i32(numberOfBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sbyte ReadSByte(sbyte min, sbyte max) => (sbyte)i32(min, max);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sbyte PeekSByte() => (sbyte)raw(8);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sbyte PeekSByte(int numberOfBits) => (sbyte)i32Peek(numberOfBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sbyte PeekSByte(sbyte min, sbyte max) => (sbyte)PeekInt(min, max);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short ReadShort() => (short)i32();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short ReadShort(int numberOfBits) => (short)i32(numberOfBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short ReadShort(short min, short max) => (short)i32(min, max);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short PeekShort() => (short)i32Peek();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short PeekShort(int numberOfBits) => (short)i32Peek(numberOfBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short PeekShort(short min, short max) => (short)PeekInt(min, max);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort ReadUShort() => (ushort)u32();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort ReadUShort(int numberOfBits) => (ushort)ReadUInt(numberOfBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort ReadUShort(ushort min, ushort max) => (ushort)ReadUInt(min, max);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort PeekUShort() => (ushort)u32Peek();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort PeekUShort(int numberOfBits) => (ushort)PeekUInt(numberOfBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort PeekUShort(ushort min, ushort max) => (ushort)PeekUInt(min, max);

        /// <summary>
        /// Reads signed 32 bit integer.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int i32(int min, int max)
        {
            Debug.Assert(min < max, "minus is not lower than max");

            int bits = BitsRequired(min, max);
            Debug.Assert(bits < totalNumberBits, "reading too many bits for requested range");

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
        public uint ReadUInt(int numberOfBits) => raw(numberOfBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadUInt(uint min, uint max)
        {
            Debug.Assert(min < max, "minus is not lower than max");

            int bits = BitsRequired(min, max);
            Debug.Assert(bits < totalNumberBits, "reading too many bits for requested range");

            return (raw(bits) + min);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint PeekUInt(int numberOfBits) => raw(numberOfBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint PeekUInt(uint min, uint max)
        {
            Debug.Assert(min < max, "minus is not lower than max");

            int bits = BitsRequired(min, max);
            Debug.Assert(bits < totalNumberBits, "reading too many bits for requested range");

            return (raw(bits) + min);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long ReadLong()
        {
            int low = i32();
            int high = i32();
            long value = high;

            return value << 32 | (uint)low;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long PeekLong()
        {
            long value = ReadLong();
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong ReadULong()
        {
            uint low = u32();
            uint high = u32();
            return (ulong)high << 32 | low;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong PeekULong()
        {
            ulong value = ReadULong();
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float ReadFloat()
        {
            var value = raw(32);
            return Unsafe.As<uint, float>(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float ReadFloat(float min, float max, float precision)
        {
            float range = max - min;
            float invPrecision = 1.0f / precision;
            float maxVal = range * invPrecision;
            int numberOfBits = BitOperations.Log2((uint)(maxVal + 0.5f)) + 1;

            return raw(numberOfBits) * precision + min;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float ReadFloat(float min, float max, int numberOfBits)
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
        public double ReadDouble()
        {
            var value = ReadULong();
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
        public int PeekByteArrayLength() => (int)raw(config.ByteArrLengthBits);
    }
}