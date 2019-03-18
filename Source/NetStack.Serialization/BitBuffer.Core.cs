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
    partial class BitBuffer
    {
        private int chunkIndex;
        private uint[] chunks;
        private int totalNumChunks;
        private int totalNumBits;
        private int bitsRead;
        private int bitsWritten;

        private int scratchUsedBits;

        private ulong scratch;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Peek(int numBits)
        {
            Debug.Assert(numBits > 0, "reading negative bits");
            Debug.Assert(numBits <= 32, "reading too many bits");
            Debug.Assert(bitsRead + numBits <= totalNumBits, "reading more bits than in buffer");

            Debug.Assert(scratchUsedBits >= 0 && scratchUsedBits <= 64, "Too many bits used in scratch, Overflow?");

            if (scratchUsedBits < numBits)
            {
                Debug.Assert(chunkIndex < totalNumChunks, "reading more than buffer size");

                scratch |= ((ulong)(chunks[chunkIndex])) << scratchUsedBits;
                scratchUsedBits += 32;
                chunkIndex++;
            }

            Debug.Assert(scratchUsedBits >= numBits, "Too many bits requested from scratch");

            uint output = (uint)(scratch & ((((ulong)1) << numBits) - 1));

            scratch >>= numBits;
            scratchUsedBits -= numBits;

            return output;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(int numBits, uint value)
        {
            Debug.Assert(numBits > 0, "Pushing negative or zero bits");
            Debug.Assert(numBits <= 32, "Pushing too many bits");
            Debug.Assert(bitsWritten + numBits <= totalNumBits, "Pushing failed, buffer is full.");
            Debug.Assert(value <= (uint)((1ul << numBits) - 1), "value too big, won't fit in requested number of bits."); // 

            value &= (uint)((1ul << numBits) - 1);

            scratch |= ((ulong)value) << scratchUsedBits;

            scratchUsedBits += numBits;

            if (scratchUsedBits >= 32)
            {
                Debug.Assert(chunkIndex < totalNumChunks, "Pushing failed, buffer is full.");
                chunks[chunkIndex] = (uint)(scratch);
                scratch >>= 32;
                scratchUsedBits -= 32;
                chunkIndex++;
            }

            bitsWritten += numBits;
        }
    }
}