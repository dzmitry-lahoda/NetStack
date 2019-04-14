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
    public interface ISpan
    {
        u32 this[i32 index]
        {
            [MethodImpl(MyMethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MyMethodImplOptions.AggressiveInlining)]
            set;
        }

        int Length
        {
            [MethodImpl(MyMethodImplOptions.AggressiveInlining)]
            get;
        }
    }

    public struct data : ISpan
    {
        public uint[] chunks;

        public uint this[int index]
        {
            [MethodImpl(MyMethodImplOptions.AggressiveInlining)]
            get => chunks[index];
            [MethodImpl(MyMethodImplOptions.AggressiveInlining)]
            set => chunks[index] = value;
        }

        public int Length
        {
            [MethodImpl(MyMethodImplOptions.AggressiveInlining)]
            get => chunks.Length;
        }
    }



    public struct data2 : ISpan
    {
        public Memory<uint> chunks;

        public uint this[int index]
        {
            [MethodImpl(MyMethodImplOptions.AggressiveInlining)]
            get => chunks.Span[index];
            [MethodImpl(MyMethodImplOptions.AggressiveInlining)]
            set => chunks.Span[index] = value;
        }

        public int Length
        {
            [MethodImpl(MyMethodImplOptions.AggressiveInlining)]
            get => chunks.Length;
        }

    }

    /// <summary>
    /// Bit level compression by ranged values.
    /// </summary>
    public partial struct GenericBitBufferWriter<T, TAcc>
    {
        public static int BitsRequired(int min, int max) =>
    (min == max) ? 1 : BitOperations.Log2((uint)(max - min)) + 1;

        public static int BitsRequired(uint min, uint max) =>
            (min == max) ? 1 : BitOperations.Log2(max - min) + 1;

        private TAcc chunks;

        private int totalNumChunks;
        private int totalNumberBits;
        private TAcc Chunks
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


        private BitBufferOptions config;

        public BitBufferOptions Options => config;

        private static BitBufferOptions defaultConfig = new BitBufferOptions();
        public const int DefaultCapacityUInt = BitBufferLimits.MtuIeee802Dot3 / 4;

        /// <summary>
        /// Creates new instance with its own buffer. 
        /// </summary>
        /// <param name="buffer">Custom buffer.</param>
        public GenericBitBufferWriter(TAcc buffer, BitBufferOptions config = default)
        {
            // TODO: try inline config as struct to improve access perfromance? Test it via benchmark
            this.config = config ?? defaultConfig;
            // not performance critical path so fine to check and throw
            if (buffer.Length == 0)
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