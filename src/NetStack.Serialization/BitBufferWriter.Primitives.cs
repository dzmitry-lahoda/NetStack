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
    partial class BitBufferWriter
    {
        /// <summary>
        /// Adds int value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void i32(i32 value, i32 min, i32 max)
        {
#if DEBUG || NETSTACK_VALIDATE
            if (min >= max) throw Argument("min should be lower than max");
            if (value < min || value > max) throw ArgumentOutOfRange(nameof(value), $"Value should be withing provided {min} and {max} range");
#endif

            int bits = BitsRequired(min, max);
            raw((uint)(value - min), bits);            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void u8(u8 value) => raw(value, 8);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void u8(u8 value, int numberOfBits) => u32(value, numberOfBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void u8(u8 value, u8 min, u8 max) => u32(value, min, max);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void i8(i8 value) => i32(value, 8);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void i8(i8 value, int numberOfBits) => i32(value, numberOfBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void i8(i8 value, i8 min, i8 max) => i32(value, min, max);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void i16(i16 value) => i32(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void i16(i16 value, int numberOfBits) => i32(value, numberOfBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void i16(i16 value, i16 min, i16 max) => i32(value, min, max);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void u16(u16 value) => u32(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void u16(u16 value, int numberOfBits) => u32(value, numberOfBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void u16(u16 value, u16 min, u16 max) => u32(value, min, max);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void u32(u32 value, int numberOfBits) => raw(value, numberOfBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void u32(u32 value, u32 min, u32 max)
        {
#if DEBUG || NETSTACK_VALIDATE
            if (min >= max) throw Argument("min should be lower than max");
            if (value < min || value > max) throw ArgumentOutOfRange(nameof(value), $"Value should be withing provided {min} and {max} range");
#endif
            int bits = BitsRequired(min, max);
            raw(value - min, bits);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void i64(i64 value)
        {
            i32((i32)(value & uint.MaxValue));
            i32((i32)(value >> 32));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void u64(u64 value)
        {
            u32((u32)(value & uint.MaxValue));
            u32((u32)(value >> 32));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void f32(f32 value)
        {
            u32 reinterpreted = Unsafe.As<f32, u32>(ref Unsafe.AsRef<f32>(in value));
            raw(reinterpreted, 32);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void f32(f32 value, f32 min, f32 max, f32 precision)
        {
#if DEBUG || NETSTACK_VALIDATE
            if (min >= max) throw Argument("min should be lower than max");
            if (value < min || value > max) throw ArgumentOutOfRange(nameof(value), $"Value should be withing provided {min} and {max} range");
#endif            
            float range = max - min;
            float invPrecision = 1.0f / precision;
            float maxVal = range * invPrecision;
            int numberOfBits = BitOperations.Log2((u32)(maxVal + 0.5f)) + 1;
            float adjusted = (value - min) * invPrecision;
            raw((u32)(adjusted + 0.5f), numberOfBits);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void f32(f32 value, f32 min, f32 max, i32 numberOfBits)
        {
#if DEBUG || NETSTACK_VALIDATE
            if (min >= max) throw Argument("min should be lower than max");
            if (value < min || value > max) throw ArgumentOutOfRange(nameof(value), $"Value should be withing provided {min} and {max} range");
#endif                    
            var maxvalue = (1 << numberOfBits) - 1;
            float range = max - min;
            var precision = range / maxvalue;
            var invPrecision = 1.0f / precision;
            f32 adjusted = (value - min) * invPrecision;
            raw((u32)(adjusted + 0.5f), numberOfBits);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void f64(f64 value)
        {
            u64 reinterpreted = Unsafe.As<f64, u64>(ref Unsafe.AsRef<f64>(in value));
            u64(reinterpreted);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddByteArray(u8[] value) => AddByteArray(value, 0, value.Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddByteArray(u8[] value, int length) => AddByteArray(value, 0, length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddByteArray(u8[] value, int offset, int length)
        {
            Debug.Assert(value != null, "Supplied bytearray is null");
            Debug.Assert(length <= config.ByteArrLengthMax, $"Byte array too big, raise the {nameof(config.ByteArrLengthBits)} value or split the array.");
            if (length > config.ByteArrLengthMax)
                length = config.ByteArrLengthMax;
            Debug.Assert(length + 9 <= (totalNumberBits - BitsWritten), "Byte array too big for buffer.");
            raw((uint)length, config.ByteArrLengthBits);
            for (var index = offset; index < length; index++)
                u8(value[index]);
        }        
    }
}