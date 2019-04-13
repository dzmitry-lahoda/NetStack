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
    public partial class BitBufferWriter<T> : BitBuffer
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
        public BitBufferWriter(i32 capacity = DefaultCapacityUInt, BitBufferOptions config = default)
        : this(new u32[capacity], config)
        {
        }

        /// <summary>
        /// Creates new instance with its own buffer. 
        /// </summary>
        /// <param name="buffer">Custom buffer.</param>
        public BitBufferWriter(u32[] buffer, BitBufferOptions config = default)
        {
            // TODO: try inline config as struct to improve access perfromance? Test it via benchmark
            this.config = config == null  ? BitBufferOptions.Default : config;
            // not performance critical path so fine to check and throw
            if (buffer == null || buffer.Length == 0)
                throw Argument("Buffer should be non null or empty", nameof(buffer));
            Chunks = buffer;
            Clear();
        }
    }
}