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
    /// <summary>
    /// Bit level compression by ranged values.
    /// </summary>
    public partial class BitBufferWriter<T> 
    {
        private BitBufferOptions config;

        public BitBufferOptions Options => config;

        private static BitBufferOptions defaultConfig = new BitBufferOptions();

        /// <summary>
        /// Creates new instance with its own buffer. Create once and reuse to avoid GC.
        /// </summary>
        /// <param name="capacity">Count of 4 byte integers used as internal buffer.</param>
        public BitBufferWriter(u16 capacity = BitBuffer.DefaultU32Capacity, BitBufferOptions config = default)
        : this(new u32[capacity], config)
        {
        }

        /// <summary>
        /// Creates new instance with its own buffer. 
        /// </summary>
        /// <param name="buffer">Custom buffer.</param>
        public BitBufferWriter(u32[] buffer, BitBufferOptions config = default) : base(new u32ArrayMemory(buffer))
        {
            // TODO: try inline config as struct to improve access performance? Test it via benchmark
            this.config = config.Equals(default) ? BitBufferOptions.Default : config;
        }

        /// <summary>
        /// Starts writing into buffer for previous buffer after <see cref="Align"/>
        /// </summary>
        public BitBufferWriter(BitBuffer<u32ArrayMemory> startFrom) : base(startFrom.Chunks) 
        {
            scratch = startFrom.scratch;
            scratchUsedBits = startFrom.scratchUsedBits;
            chunkIndex = startFrom.chunkIndex;
            Align();
        }
    }
}