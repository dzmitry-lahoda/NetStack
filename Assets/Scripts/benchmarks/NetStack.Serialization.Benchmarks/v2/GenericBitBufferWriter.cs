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
    public partial struct MemoryBufferWriter<T> 
    {
                public static int BitsRequired(int min, int max) =>
            (min == max) ? 1 : BitOperations.Log2((uint)(max - min)) + 1;

        public static int BitsRequired(uint min, uint max) =>
            (min == max) ? 1 : BitOperations.Log2(max - min) + 1;

        private uint[] chunks;        
        private int totalNumChunks;        
        private int totalNumberBits;  
        private uint[] Chunks
        {
            set 
            {
                chunks = value;
                totalNumChunks = chunks.Length;
                totalNumberBits = totalNumChunks * 8 * Unsafe.SizeOf<uint>();   
            }
        }


        // bit index onto current head
        private int chunkIndex;
        private int scratchUsedBits;
        
        // last partially read value
        private ulong scratch;

        /// <summary>
        /// Sets buffer cursor to zero. Can start writing again.
        /// </summary>
        [MethodImpl(MyMethodImplOptions.AggressiveInlining)]
        public void Reset()
        {            
            chunkIndex = 0;
            scratch = 0;
            scratchUsedBits = 0;
        }

        private BitBufferOptionsOrig config;

        public BitBufferOptionsOrig Options => config;

        private static BitBufferOptionsOrig defaultConfig = new BitBufferOptionsOrig();
        public const int DefaultCapacityUInt = BitBufferLimits.MtuIeee802Dot3 / 4;

        /// <summary>
        /// Creates new instance with its own buffer. Create once and reuse to avoid GC.
        /// Call <see cref="FromArray"/> to reinitialize with copy of data.
        /// </summary>
        /// <param name="capacity">Count of 4 byte integers used as internal buffer.</param>
        public MemoryBufferWriter(i32 capacity = DefaultCapacityUInt, BitBufferOptionsOrig config = default)
        : this(new u32[capacity], config)
        {
        }

        /// <summary>
        /// Creates new instance with its own buffer. 
        /// </summary>
        /// <param name="buffer">Custom buffer.</param>
        public MemoryBufferWriter(u32[] buffer, BitBufferOptionsOrig config = default)
        {
            // TODO: try inline config as struct to improve access perfromance? Test it via benchmark
            this.config = config ?? defaultConfig;
            // not performance critical path so fine to check and throw
            if (buffer == null || buffer.Length == 0)
                throw Argument("Buffer should be non null or empty", nameof(buffer));

                chunks = buffer;
                totalNumChunks = chunks.Length;
                totalNumberBits = totalNumChunks * 8 * Unsafe.SizeOf<uint>();   

            chunkIndex = 0;
            scratch = 0;
            scratchUsedBits = 0;
        }
    }
}