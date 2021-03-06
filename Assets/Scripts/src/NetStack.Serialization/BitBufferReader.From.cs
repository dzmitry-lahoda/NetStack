﻿﻿using System;
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
    public partial class BitBufferReader<T>
    {
        /// <summary>
        /// Copies data from array.
        /// </summary>
        public void CopyFrom(ReadOnlySpan<u8> data)
        {
            // may Throw.here as not hot path
            if (data.Length <= 0)
                Throw.Argument("Should be positive", nameof(data.Length));
            if (data.Length > System.UInt16.MaxValue * 4)
                Throw.Argument($"Should be less or equal to {System.UInt16.MaxValue * 4}", nameof(data.Length));


            var length = data.Length;
            Reset();
            var step = Unsafe.SizeOf<u32>();
            var numChunks = (u16)((length / step) + 1);

            // TODO: move out this expansion from here
            if (chunks.Length < numChunks)
            {
                Chunks = new u32ArrayMemory(new u32[numChunks]); // call it once to stay expanded forever
            }

            // data must be 4 or 8 bytes i64 because 32 and 64 machines https://gafferongames.com/post/reading_and_writing_packets/
            // TODO: possible to optimize to avoid copy? some kind of unsafe cast?
            // TODO: try u64 for performance as most of devices will be 64 bit?
            // https://github.com/nxrighthere/NetStack/issues/1#issuecomment-475212246
            for (u16 i = 0; i < numChunks; i++)
            {
                i32 dataIdx = (i32)i * step;
                u32 chunk = 0;
                // TODO: ref into data and do block copy of all 4bytes, copy only last 3 bytes by hand
                // may optimize by calculating variable her and doing zero init of remaining blocks
                // may reintepret unsafe as uint, and then if less than 3 then only read last as 1 2 3
                if (dataIdx < length)
                    chunk = (u32)data[dataIdx];

                if (dataIdx + 1 < length)
                    chunk = chunk | (u32)data[dataIdx + 1] << 8;

                if (dataIdx + 2 < length)
                    chunk = chunk | (u32)data[dataIdx + 2] << 16;

                if (dataIdx + 3 < length)
                    chunk = chunk | (u32)data[dataIdx + 3] << 24;

                chunks[i] = chunk;
            }

            // TODO: write sets 1 bit in the end. so we may read all remaining zeroes, minis 1 bit for flag, and make total less
            // TODO: should be do that at all? Consoder multiple buffers writing sequenctially or other length indicator (like prefix)
            // var leadingZeros = BitOperations.LeadingZeroCount(data[length - 1]);
            // totalNumberBits = 8 * length - leadingZeros - 1;
        }
    }
}