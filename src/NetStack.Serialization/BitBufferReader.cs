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
    /// <summary>
    /// Bit level compression by ranged values.
    /// </summary>
    public partial class BitBufferReader<T> : BitBuffer
    {
        private BitBufferOptions config;

        public BitBufferOptions Options => config;

        private static BitBufferOptions defaultConfig = new BitBufferOptions();
        public const i32 DefaultCapacityUInt = BitBufferLimits.MtuIeee802Dot3 / 4;

        /// <summary>
        /// Creates new instance with its own buffer. Create once and reuse to avoid GC.
        /// Call <see cref="FromArray"/> to reinitialize with copy of data.
        /// </summary>
        /// <param name="capacity">Count of 4 byte integers used as internal buffer.</param>
        public BitBufferReader(i32 capacity = DefaultCapacityUInt, BitBufferOptions config = default)
        : this(new uint[capacity], config)
        {
        }

        /// <summary>
        /// Creates new instance with its own buffer. 
        /// </summary>
        /// <param name="buffer">Custom buffer.</param>
        public BitBufferReader(uint[] buffer, BitBufferOptions config = default)
        {
            // TODO: try inline config as struct to improve access perfromance? Test it via benchmark
            this.config = config == null  ? BitBufferOptions.Default : config;
            // not performance critical path so fine to check and throw
            if (buffer == null || buffer.Length == 0)
                throw new ArgumentException("Buffer should be non null or empty", nameof(buffer));

            Chunks = buffer;
            Clear();
        }

        public BitBufferReader(BitBuffer startFrom)
        {
            Chunks = startFrom.chunks;
            scratch = startFrom.scratch;
            scratchUsedBits = startFrom.scratchUsedBits;
            chunkIndex = startFrom.chunkIndex;
            Finish();
        }

        private void Finish()
        {
            if (scratchUsedBits != 0)
            {
                #if DEBUG || NETSTACK_VALIDATE
                if (chunkIndex >= totalNumChunks) throw new IndexOutOfRangeException("buffer overflow when trying to finalize stream");
                #endif
                chunks[chunkIndex] = (u32)(scratch & 0xFFFFFFFF);
                scratch >>= 32;
                scratchUsedBits -= 32;
                chunkIndex++;
            }
        }

    }
}