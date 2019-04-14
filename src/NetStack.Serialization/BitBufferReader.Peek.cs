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
        public i8 i8Peek()
        {
            var index = SIndex;
            var value = i8();
            SIndex = index;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public i8 i8Peek(i32 numberOfBits) 
        {
            var index = SIndex;
            var value = i8(numberOfBits);
            SIndex = index;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public i8 i8Peek(i8 min, i8 max)
        {
            var index = SIndex;
            var value = i8(min, max);
            SIndex = index;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public i16 i16Peek() 
        {
            var index = SIndex;
            var value = i16();
            SIndex = index;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public i16 i16Peek(i32 numberOfBits) 
        {
            var index = SIndex;
            var value = i16(numberOfBits);
            SIndex = index;
            return value;
        } 

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public i16 i16Peek(i16 min, i16 max) 
        {
            var index = SIndex;
            var value = i16(min, max);
            SIndex = index;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public u16 u16Peek() 
        {
            var index = SIndex;
            var value = u16();
            SIndex = index;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public u16 u16Peek(i32 numberOfBits)
        {
            var index = SIndex;
            var value = u16(numberOfBits);
            SIndex = index;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public u16 u16Peek(u16 min, u16 max)
        {
            var index = SIndex;
            var value = u16(min, max);
            SIndex = index;
            return value;
        }

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
            var index = SIndex;
            var value = i32(min, max);
            SIndex = index;
            return value;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public u32 u32Peek(i32 numberOfBits) 
        {
            var index = SIndex;
            var value = u32(numberOfBits);
            SIndex = index;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public u32 u32Peek(u32 min, u32 max)
        {
            var index = SIndex;
            var value = u32(min, max);
            SIndex = index;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public i64 i64Peek()
        {
            var index = SIndex;
            var value = i64();
            SIndex = index;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public u64 u64Peek()
        {
            var index = SIndex;
            var value = u64();
            SIndex = index;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public f32 f32Peek(f32 min, f32 max, f32 precision)
        {
            var index = SIndex;
            var value = f32(min, max, precision);
            SIndex = index;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public f32 f32Peek(f32 min, f32 max, i32 numberOfBits)
        {
            var index = SIndex;
            var value = f32(min, max, numberOfBits);
            SIndex = index;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public f32 f32Peek()
        {
            var index = SIndex;
            var value = f32();
            SIndex = index;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public f64 f64Peek()
        {
            var index = SIndex;
            var value = f64();
            SIndex = index;
            return value;
        }
    }
}