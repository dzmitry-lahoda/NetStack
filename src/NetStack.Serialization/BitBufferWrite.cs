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
    partial class BitBuffer
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddInt(int value, int min, int max)
        {
            Debug.Assert(min < max, "minus is not lower than max");
            Debug.Assert(value >= min, "value is lower than minimal");
            Debug.Assert(value <= max, "value is higher than maximal");
            int bits = BitsRequired(min, max);
            AddRaw((uint)(value - min), bits);            
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddByte(byte value) => AddRaw(value, 8);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddByte(byte value, int numberOfBits) => AddUInt(value, numberOfBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddByte(byte value, byte min, byte max) => AddUInt(value, min, max);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddSByte(sbyte value) => AddInt(value, 8);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddSByte(sbyte value, int numberOfBits) => AddInt(value, numberOfBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddSByte(sbyte value, sbyte min, sbyte max) => AddInt(value, min, max);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddShort(short value) => AddInt(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddShort(short value, int numberOfBits) => AddInt(value, numberOfBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddShort(short value, short min, short max) => AddInt(value, min, max);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddUShort(ushort value) => AddUInt(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddUShort(ushort value, int numberOfBits) => AddUInt(value, numberOfBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddUShort(ushort value, ushort min, ushort max) => AddUInt(value, min, max);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddUInt(uint value, int numberOfBits) => AddRaw(value, numberOfBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddUInt(uint value, uint min, uint max)
        {
            Debug.Assert(min < max, "minus is not lower than max");
            Debug.Assert(value >= min, "value is lower than minimal");
            Debug.Assert(value <= max, "value is higher than maximal");
            int bits = BitsRequired(min, max);
            AddRaw(value - min, bits);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddLong(long value)
        {
            AddInt((int)(value & uint.MaxValue));
            AddInt((int)(value >> 32));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddULong(ulong value)
        {
            AddUInt((uint)(value & uint.MaxValue));
            AddUInt((uint)(value >> 32));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddFloat(in float value)
        {
            uint reinterpreted = Unsafe.As<float, uint>(ref Unsafe.AsRef<float>(in value));
            AddRaw(reinterpreted, 32);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddFloat(float value, float min, float max, float precision)
        {
            float range = max - min;
            float invPrecision = 1.0f / precision;
            float maxVal = range * invPrecision;
            int numberOfBits = BitOperations.Log2((uint)(maxVal + 0.5f)) + 1;
            float adjusted = (value - min) * invPrecision;
            AddRaw((uint)(adjusted + 0.5f), numberOfBits);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddFloat(float value, float min, float max, int numberOfBits)
        {
            var maxvalue = (1 << numberOfBits) - 1;

            float range = max - min;
            var precision = range / maxvalue;
            var invPrecision = 1.0f / precision;

            float adjusted = (value - min) * invPrecision;

            AddRaw((uint)(adjusted + 0.5f), numberOfBits);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddDouble(in double value)
        {
            ulong reinterpreted = Unsafe.As<double, ulong>(ref Unsafe.AsRef<double>(in value));
            AddULong(reinterpreted);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddByteArray(byte[] value)
        {
            AddByteArray(value, 0, value.Length);            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddByteArray(byte[] value, int length) => AddByteArray(value, 0, length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddByteArray(byte[] value, int offset, int length)
        {
            Debug.Assert(value != null, "Supplied bytearray is null");
            Debug.Assert(length <= config.ByteArrLengthMax, $"Byte array too big, raise the {nameof(config.ByteArrLengthBits)} value or split the array.");

            if (length > config.ByteArrLengthMax)
                length = config.ByteArrLengthMax;

            Debug.Assert(length + 9 <= (totalNumberBits - BitsWritten), "Byte array too big for buffer.");

            AddRaw((uint)length, config.ByteArrLengthBits);

            for (var index = offset; index < length; index++)
            {
                AddByte(value[index]);
            }
        }        
    }
}