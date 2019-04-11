using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
#if !(ENABLE_MONO || ENABLE_IL2CPP)
using System.Diagnostics;
using System.Numerics;
#else
using UnityEngine;
#endif

namespace NetStack.Serialization
{
    // core untyped data specific part of bit buffer
    public partial class BitBufferBase
    {
        public static int BitsRequired(int min, int max) =>
            (min == max) ? 1 : BitOperations.Log2((uint)(max - min)) + 1;

        public static int BitsRequired(uint min, uint max) =>
            (min == max) ? 1 : BitOperations.Log2(max - min) + 1;

        protected uint[] chunks;        
        protected int totalNumChunks;        
        protected int totalNumberBits;  
        internal uint[] Chunks
        {
            set 
            {
                chunks = value;
                totalNumChunks = chunks.Length;
                totalNumberBits = totalNumChunks * 8 * Unsafe.SizeOf<uint>();   
            }
        }


        // bit index onto current head
        protected int chunkIndex;
        protected int scratchUsedBits;
        
        // last partially read value
        protected ulong scratch;

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

        public override string ToString()
        {
            var toStringBuilder = new StringBuilder(chunks.Length * 8);

            for (int i = chunks.Length - 1; i >= 0; i--)
            {
                toStringBuilder.Append(Convert.ToString(chunks[i], 2).PadLeft(32, '0'));
            }

            var spaced = new StringBuilder();

            for (int i = 0; i < toStringBuilder.Length; i++)
            {
                spaced.Append(toStringBuilder[i]);

                if (((i + 1) % 8) == 0)
                    spaced.Append(" ");
            }

            return spaced.ToString();
        }        
    }
}