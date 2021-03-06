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
    public static partial class BitBuffer
    {
        public static u8 BitsRequired(i32 min, i32 max) =>
              (u8)((min == max) ? 1 : BitOperations.Log2((u32)(max - min)) + 1);

        public static u8 BitsRequired(u32 min, u32 max) =>
            (u8)((min == max) ? 1 : BitOperations.Log2(max - min) + 1);

        public const u16 DefaultU32Capacity = BitBufferLimits.MtuIeee802Dot3 / 4;
    }

    // core untyped data specific part of bit buffer
    public abstract partial class BitBuffer<TStorage>
        where TStorage : IMemory<u32>
    {
        internal BitBuffer()
        {
            // dot not allow inheritance outside of assembly to simplify move to struct only code    
        }

        #region BState
        // putting BState in struct degrades performance on 10% on .NET Core 2.2. 
        // How to do struct inheritance? May be should do Auto layout?

        // having this struct inline does not hurt perf
        protected internal TStorage chunks;

        protected u16 totalNumChunks;
        protected u32 totalNumberBits;
        protected internal TStorage Chunks
        {
            set
            {
                chunks = value;
                totalNumChunks = chunks.Length;
                totalNumberBits = (u32)(totalNumChunks * 8 * Unsafe.SizeOf<u32>());
            }

            get => chunks;
        }
        #endregion

        #region SIndex

        // bit index onto current head
        // trying to put these 2 or 3 into one struct degrade performance on .NET Core 2.1 x86-64
        // have tried Auto and Explicit with 2 i32
        protected internal u16 chunkIndex;
        protected internal i32 scratchUsedBits;

        // last partially read value
        protected internal u64 scratch;

        /// <summary>
        /// Sets buffer cursor to zero. Can start writing again.
        /// </summary>
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public void Reset()
        {
            chunkIndex = 0;
            scratch = 0;
            scratchUsedBits = 0;
        }

        public (u16 ChunkIndex, i32 ScratchUsedBits, u64 Scratch) SIndex
        {
            get => (chunkIndex, scratchUsedBits, scratch);
            set
            {
                chunkIndex = value.ChunkIndex;
                scratchUsedBits = value.ScratchUsedBits;
                scratch = value.Scratch;
            }
        }

        #endregion

        /// <summary>
        /// Call aligns remaining bits to full bytes.
        /// </summary>
        public void Align()
        {
            if (scratchUsedBits != 0)
            {
#if !NO_EXCEPTIONS
                if (chunkIndex >= totalNumChunks) Throw.IndexOutOfRange("buffer overflow when trying to finalize stream");
#endif
                chunks[chunkIndex] = (u32)(scratch & 0xFFFFFFFF);
                scratch >>= 32;
                scratchUsedBits -= 32;
                chunkIndex++;
            }
        }

        public override string ToString()
        {
            var toStringBuilder = new StringBuilder((i32)chunks.Length * 8);

            for (var i = chunks.Length - 1; i >= 0; i--)
                toStringBuilder.Append(Convert.ToString(chunks[(u16)i], 2).PadLeft(32, '0'));

            var spaced = new StringBuilder();
            for (var i = 0; i < toStringBuilder.Length; i++)
            {
                spaced.Append(toStringBuilder[i]);

                if (((i + 1) % 8) == 0)
                    spaced.Append(" ");
            }

            return spaced.ToString();
        }
    }
}