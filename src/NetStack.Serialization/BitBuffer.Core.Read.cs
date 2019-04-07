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
        public int BitsRead 
        {
            get 
            {
                var indexInBits = chunkIndex * 32;
                var over = scratchUsedBits != 0 ? 1 : 0; // TODO: speed up with bit hacking
                return indexInBits - over * scratchUsedBits;
            }            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ReadBool()
        {
#if DEBUG || NETSTACK_VALIDATE
            if (BitsRead >= totalNumberBits) throw new InvalidOperationException("reading more bits than in buffer");
            if (scratchUsedBits < 1 && chunkIndex >= totalNumChunks) throw new InvalidOperationException("reading more than buffer size");
#endif
            if (scratchUsedBits < 1)
            {
                scratch |= ((ulong)(chunks[chunkIndex])) << scratchUsedBits;
                scratchUsedBits += 32;
                chunkIndex++;
            }

#if DEBUG
            if (scratchUsedBits == 0) throw new InvalidOperationException("Too many bits requested from scratch");
#endif
            uint output = (uint)(scratch & 1);

            scratch >>= 1;
            scratchUsedBits -= 1;

            return output > 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadRaw(int numberOfBits)
        {
#if DEBUG
    var oldScratchUsedBits = scratchUsedBits;
#endif

#if DEBUG || NETSTACK_VALIDATE
            if (numberOfBits <= 0 || numberOfBits > 32) throw new ArgumentOutOfRangeException(nameof(numberOfBits), $"Should read from 1 to 32. Cannot read {numberOfBits}"); 
            if (BitsRead + numberOfBits > totalNumberBits)throw new InvalidOperationException("reading more bits than in buffer");
            if (scratchUsedBits < 0 || scratchUsedBits > 64) throw new InvalidProgramException($"{scratchUsedBits} Too many bits used in scratch, Overflow?");
#endif

            if (scratchUsedBits < numberOfBits)
            {
#if DEBUG || NETSTACK_VALIDATE                
                if (chunkIndex >= totalNumChunks) throw new InvalidOperationException("reading more than buffer size");
#endif
                scratch |= ((ulong)(chunks[chunkIndex])) << scratchUsedBits;
                scratchUsedBits += 32;
                chunkIndex++;
            }

#if DEBUG
            if (scratchUsedBits < numberOfBits) throw new InvalidOperationException("Too many bits requested from scratch");
#endif
            uint output = (uint)(scratch & ((((ulong)1) << numberOfBits) - 1));

            scratch >>= numberOfBits;
            scratchUsedBits -= numberOfBits;

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
        public int PeekInt(int numberOfBits)
        {
            uint value = ReadRaw(numberOfBits);
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
                buffer = ReadRaw(8);

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
        public int ReadInt(int numberOfBits)
        {
            uint value = ReadRaw(numberOfBits);
            int zagzig = (int)((value >> 1) ^ (-(int)(value & 1)));
            return zagzig;
        }
    }
}