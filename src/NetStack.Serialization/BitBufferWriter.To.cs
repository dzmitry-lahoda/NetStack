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
using System.Buffers;
#if !(ENABLE_MONO || ENABLE_IL2CPP)
using System.Diagnostics;
using System.Numerics;
#else
using UnityEngine;
#endif

// Until migration to .NET Standard 2.1
using BitOperations = System.Numerics.BitOperations;

namespace NetStack.Serialization
{
    public partial class BitBufferWriter<T>
    {
        /// <summary>
        /// Dot not use for production. GC allocated array.
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray()
        {
            var data = new byte[LengthWritten];
            ToArray(data);
            return data;
        }

        /// <summary>
        /// Rents array 
        /// </summary>
        public byte[] ToArray(ArrayPool<byte> pool = null)
        {
            pool = pool ?? ArrayPool<byte>.Shared;
            var data = pool.Rent(LengthWritten);
            ToArray(data);
            return data;
        }        

        /// <summary>
        /// Calls <see cref="Finish"/> and copies all internal data into array.
        /// </summary>
        /// <param name="data">The output buffer.</param>
        /// <returns>Count of bytes written.</returns>
        public int ToArray(byte[] data)
        {
            int length = data.Length;
            ToArray(data, 0, length);
            return LengthWritten;
        }

        public void ToArray(byte[] data, int position, int length)
        {
            // may throw here as not hot path
            if (length <= 0)
                throw new ArgumentException("Should be positive", nameof(length));
            if (position < 0)
                throw new ArgumentException("Should be non negative", nameof(position));
            var step = Unsafe.SizeOf<uint>();
            raw(1, 1);
            var bitsPassed = BitsWritten;

            Finish();

            int numChunks = (bitsPassed >> 5) + 1;

            for (int i = 0; i < numChunks; i++)
            {
                int dataIdx = i * step;
                uint chunk = chunks[i];

                if (dataIdx < length)
                    data[position + dataIdx] = (byte)(chunk);

                if (dataIdx + 1 < length)
                    data[position + dataIdx + 1] = (byte)(chunk >> 8);

                if (dataIdx + 2 < length)
                    data[position + dataIdx + 2] = (byte)(chunk >> 16);

                if (dataIdx + 3 < length)
                    data[position + dataIdx + 3] = (byte)(chunk >> 24);
            }
        }

        /// <summary>
        /// Calls <see cref="Finish"/> and copies all internal data into span.
        /// </summary>
        /// <param name="data">The output buffer.</param>
        /// <returns>Count of bytes written.</returns>
        public int ToSpan(Span<byte> data)
        {
            // may throw here as not hot path, check span length

            raw(1, 1);
            var bitsPassed = BitsWritten;
            Finish();

            int numChunks = (bitsPassed >> 5) + 1;
            int length = data.Length;
            var step = Unsafe.SizeOf<uint>();
            for (int i = 0; i < numChunks; i++)
            {
                int dataIdx = i * step;
                uint chunk = chunks[i];

                if (dataIdx < length)
                    data[dataIdx] = (byte)(chunk);

                if (dataIdx + 1 < length)
                    data[dataIdx + 1] = (byte)(chunk >> 8);

                if (dataIdx + 2 < length)
                    data[dataIdx + 2] = (byte)(chunk >> 16);

                if (dataIdx + 3 < length)
                    data[dataIdx + 3] = (byte)(chunk >> 24);
            }

            return LengthWritten;
        }
    }
}