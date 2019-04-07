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
    partial class BitBuffer
    {
        private int bitsRead;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ReadBool() 
        {
            var result = PeekBool();
            bitsRead++;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool PeekBool()
        {
#if DEBUG || NETSTACK_VALIDATE
            if (bitsRead >= totalNumberBits) throw new InvalidOperationException("reading more bits than in buffer");
#endif
            if (scratchUsedBits < 1)
            {
                Debug.Assert(chunkIndex < totalNumChunks, "reading more than buffer size");

                scratch |= ((ulong)(chunks[chunkIndex])) << scratchUsedBits;
                scratchUsedBits += 32;
                chunkIndex++;
            }

            Debug.Assert(scratchUsedBits >= 1, "Too many bits requested from scratch");

            uint output = (uint)(scratch & ((((ulong)1) << 1) - 1));

            scratch >>= 1;
            scratchUsedBits -= 1;

            return output > 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Peek(int numBits)
        {
            Debug.Assert(numBits > 0, "reading negative bits");
            Debug.Assert(numBits <= 32, "reading too many bits");
            Debug.Assert(bitsRead + numBits <= totalNumberBits, "reading more bits than in buffer");

            Debug.Assert(scratchUsedBits >= 0 && scratchUsedBits <= 64, $"{scratchUsedBits} Too many bits used in scratch, Overflow?");

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
        public int PeekInt()
        {
            uint value = PeekUInt();
            int zagzig = (int)((value >> 1) ^ (-(int)(value & 1)));
            return zagzig;
        }

        /// <summary>
        /// Reads value without progressing bits position.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int PeekInt(int numBits)
        {
            uint value = Peek(numBits);
            int zagzig = (int)((value >> 1) ^ (-(int)(value & 1)));
            return zagzig;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadUInt()
        {
            uint buffer = 0x0u;
            uint value = 0x0u;
            int shift = 0;

            do
            {
                buffer = Read(8);

                value |= (buffer & 0b0111_1111u) << shift;
                shift += 7;
            }
            while ((buffer & 0b1000_0000u) > 0);

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadInt()
        {
            uint value = ReadUInt();
            int zagzig = (int)((value >> 1) ^ (-(int)(value & 1)));
            return zagzig;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadInt(int numBits)
        {
            uint value = Read(numBits);
            int zagzig = (int)((value >> 1) ^ (-(int)(value & 1)));
            return zagzig;
        }
    }
}