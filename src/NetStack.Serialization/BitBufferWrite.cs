﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
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
    public partial class BitBufferWrite : BitBuffer
    {
        private BitBufferOptions config;

        public BitBufferOptions Options => config;

        private static BitBufferOptions defaultConfig = new BitBufferOptions();
        public const int DefaultCapacityUInt = BitBufferLimits.MtuIeee802Dot3 / 4;

        /// <summary>
        /// Creates new instance with its own buffer. Create once and reuse to avoid GC.
        /// Call <see cref="FromArray"/> to reinitialize with copy of data.
        /// </summary>
        /// <param name="capacity">Count of 4 byte integers used as internal buffer.</param>
        public BitBufferWrite(int capacity = DefaultCapacityUInt, BitBufferOptions config = default)
        : this(new uint[capacity], config)
        {
        }

        /// <summary>
        /// Creates new instance with its own buffer. 
        /// </summary>
        /// <param name="buffer">Custom buffer.</param>
        public BitBufferWrite(uint[] buffer, BitBufferOptions config = default)
        {
            // TODO: try inline config as struct to improve access perfromance? Test it via benchmark
            this.config = config ?? defaultConfig;
            // not performance critical path so fine to check and throw
            if (buffer == null || buffer.Length == 0)
                throw new ArgumentException("Buffer should be non null or empty", nameof(buffer));

            builder = new StringBuilder(this.config.StringLengthMax);
            Chunks = buffer;
            Clear();
        }
    }
}