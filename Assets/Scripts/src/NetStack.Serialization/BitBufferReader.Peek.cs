﻿using System;
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
        /// <summary>
        /// Reads int, but does not move cursor.
        /// </summary>
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public i32 i32Peek()
        {
            T encoder = default;
            return encoder.i32(this);
        }

        /// <summary>
        /// Reads uint, but does not move cursor.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public u32 u32Peek()
        {
            u32 value = u32();
            return value;
        }

        /// <summary>
        /// Reads i32 value without progressing bits position.
        /// </summary>
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public i32 i32Peek(i32 numberOfBits)
        {
            T encoder = default;
            u32 value = raw(numberOfBits);
            return encoder.decode(value);
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public u8 u8Peek()
        {
            var index = state.SIndex;
            var value = (u8)raw(8);
            state.SIndex = index;
            return value;
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public u8 u8Peek(i32 numberOfBits)
        {
            var index = state.SIndex;
            var value = (u8)u32Peek(numberOfBits);
            state.SIndex = index;
            return value;
        }
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public u8 u8Peek(u8 min, u8 max)
        {
            var index = state.SIndex;
            var value = (u8)u32Peek(min, max);
            state.SIndex = index;
            return value;
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public i8 i8Peek()
        {
            var index = state.SIndex;
            var value = i8();
            state.SIndex = index;
            return value;
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public i8 i8Peek(i32 numberOfBits)
        {
            var index = state.SIndex;
            var value = i8(numberOfBits);
            state.SIndex = index;
            return value;
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public i8 i8Peek(i8 min, i8 max)
        {
            var index = state.SIndex;
            var value = i8(min, max);
            state.SIndex = index;
            return value;
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public i16 i16Peek()
        {
            var index = state.SIndex;
            var value = i16();
            state.SIndex = index;
            return value;
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public i16 i16Peek(i32 numberOfBits)
        {
            var index = state.SIndex;
            var value = i16(numberOfBits);
            state.SIndex = index;
            return value;
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public i16 i16Peek(i16 min, i16 max)
        {
            var index = state.SIndex;
            var value = i16(min, max);
            state.SIndex = index;
            return value;
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public u16 u16Peek()
        {
            var index = state.SIndex;
            var value = u16();
            state.SIndex = index;
            return value;
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public u16 u16Peek(i32 numberOfBits)
        {
            var index = state.SIndex;
            var value = u16(numberOfBits);
            state.SIndex = index;
            return value;
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public u16 u16Peek(u16 min, u16 max)
        {
            var index = state.SIndex;
            var value = u16(min, max);
            state.SIndex = index;
            return value;
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public bool bPeek()
        {
            var index = state.SIndex;
            var result = b();
            state.SIndex = index;
            return result;
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public i32 i32Peek(i32 min, i32 max)
        {
            var index = state.SIndex;
            var value = i32(min, max);
            state.SIndex = index;
            return value;
        }


        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public u32 u32Peek(i32 numberOfBits)
        {
            var index = state.SIndex;
            var value = u32(numberOfBits);
            state.SIndex = index;
            return value;
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public u32 u32Peek(u32 min, u32 max)
        {
            var index = state.SIndex;
            var value = u32(min, max);
            state.SIndex = index;
            return value;
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public i64 i64Peek()
        {
            var index = state.SIndex;
            var value = i64();
            state.SIndex = index;
            return value;
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public u64 u64Peek()
        {
            var index = state.SIndex;
            var value = u64();
            state.SIndex = index;
            return value;
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public f32 f32Peek(f32 min, f32 max, f32 precision)
        {
            var index = state.SIndex;
            var value = f32(min, max, precision);
            state.SIndex = index;
            return value;
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public f32 f32Peek(f32 min, f32 max, i32 numberOfBits)
        {
            var index = state.SIndex;
            var value = f32(min, max, numberOfBits);
            state.SIndex = index;
            return value;
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public f32 f32Peek()
        {
            var index = state.SIndex;
            var value = f32();
            state.SIndex = index;
            return value;
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public f64 f64Peek()
        {
            var index = state.SIndex;
            var value = f64();
            state.SIndex = index;
            return value;
        }
    }
}