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
        /// Calls <see cref="Finish"/> and copies all internal data into span.
        /// </summary>
        /// <param name="data">The output buffer.</param>
        /// <returns>Count of bytes written.</returns>
        public i32 ToSpan(Span<u8> data)
        {
            // may throw here as not hot path, check span length

            raw(1, 1);
            var bitsPassed = BitsWritten;
            Finish();

            i32 numChunks = (bitsPassed >> 5) + 1;
            i32 length = data.Length;
            var step = Unsafe.SizeOf<u32>();
            for (var i = 0; i < numChunks; i++)
            {
                i32 dataIdx = i * step;
                u32 chunk = chunks[i];
                // TODO: optimize by copying 4 byte in single call via Unsafe
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