using System;
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

namespace NetStack.Serialization
{

    // core untyped data specific part of bit buffer
    public abstract partial class BitBuffer
    {
        
        public const i32 DefaultU32Capacity = BitBufferLimits.MtuIeee802Dot3 / 4;

        internal BitBuffer()
        {
            // dot not allow inheritance outside of assembly to simplify move to stuct only code    
        }

        public static i32 BitsRequired(i32 min, i32 max) =>
            (min == max) ? 1 : BitOperations.Log2((u32)(max - min)) + 1;

        public static i32 BitsRequired(u32 min, u32 max) =>
            (min == max) ? 1 : BitOperations.Log2(max - min) + 1;

        protected internal uint[] chunks;        
        protected i32 totalNumChunks;        
        protected i32 totalNumberBits;  
        protected internal uint[] Chunks
        {
            set 
            {
                chunks = value;
                totalNumChunks = chunks.Length;
                totalNumberBits = totalNumChunks * 8 * Unsafe.SizeOf<uint>();   
            }
        }


        // bit index onto current head
        protected internal i32 chunkIndex;
        protected internal i32 scratchUsedBits;
        
        // last partially read value
        protected internal u64 scratch;

        /// <summary>
        /// Sets buffer cursor to zero. Can start writing again.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {            
            chunkIndex = 0;
            scratch = 0;
            scratchUsedBits = 0;
        }

        /// <summary>
        /// Call aligns remaining bits to full bytes.
        /// </summary>
        public void Align()
        {
            if (scratchUsedBits != 0)
            {
                #if DEBUG || NETSTACK_VALIDATE
                if (chunkIndex >= totalNumChunks) throw IndexOutOfRange("buffer overflow when trying to finalize stream");
                #endif
                chunks[chunkIndex] = (u32)(scratch & 0xFFFFFFFF);
                scratch >>= 32;
                scratchUsedBits -= 32;
                chunkIndex++;
            }
        }        

        public override string ToString()
        {
            var toStringBuilder = new StringBuilder(chunks.Length * 8);

            for (i32 i = chunks.Length - 1; i >= 0; i--)
            {
                toStringBuilder.Append(Convert.ToString(chunks[i], 2).PadLeft(32, '0'));
            }

            var spaced = new StringBuilder();

            for (i32 i = 0; i < toStringBuilder.Length; i++)
            {
                spaced.Append(toStringBuilder[i]);

                if (((i + 1) % 8) == 0)
                    spaced.Append(" ");
            }

            return spaced.ToString();
        }        
    }
}