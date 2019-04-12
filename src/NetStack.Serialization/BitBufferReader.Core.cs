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
    partial class BitBufferReader<T> : IRaw2
         where T:unmanaged, IDecompression<BitBufferReader<T>> 
    {        
        public bool CanReadMore => totalNumberBits > BitsRead;
    
        // total count of used bits since buffer start
        public int BitsRead 
        {
            get 
            {
                var indexInBits = chunkIndex * 32;
                var over = scratchUsedBits != 0 ? 1 : 0; // TODO: speed up with bit hacking
                return indexInBits - over * scratchUsedBits;
            }            
        }

        /// <summary>
        /// Reads one bit boolean.
        /// </summary>        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool b()
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

        /// <summary>
        /// Reads raw data.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint raw(int numberOfBits)
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

        /// <summary>
        /// Reads int, but does not move cursor.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int i32Peek()
        {
            uint value = u32Peek();
            int zagzig = (int)((value >> 1) ^ (-(int)(value & 1)));
            return zagzig;
        }

        /// <summary>
        /// Reads uint, but does not move cursor.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint u32Peek()
        {
            uint value = u32();
            return value;
        }

        /// <summary>
        /// Reads int value without progressing bits position.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int i32Peek(int numberOfBits)
        {
            uint value = raw(numberOfBits);
            int zagzig = (int)((value >> 1) ^ (-(int)(value & 1)));
            return zagzig;
        }

        /// <summary>
        /// Reads 7 bit encoded uint value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint u32()
        {
            uint buffer = 0x0u;
            uint value = 0x0u;
            int shift = 0;

            do
            {
                buffer = raw(8);

                value |= (buffer & 0b0111_1111u) << shift;
                shift += 7;
            }
            while ((buffer & 0b1000_0000u) > 0);

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int i32()
        {
            uint value = u32();
            int zagzig = (int)((value >> 1) ^ (-(int)(value & 1)));
            return zagzig;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int i32(int numberOfBits)
        {
            uint value = raw(numberOfBits);
            int zagzig = (int)((value >> 1) ^ (-(int)(value & 1)));
            return zagzig;
        }

        // TODO: change API to be more safe on bit buffer operations (protect from misuse)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetReadPosition(int bitsRead)
        {
#if DEBUG || NETSTACK_VALIDATE        
        if (bitsRead < 0) throw new ArgumentException("Pushing negative bits", nameof(bitsRead));
        if (bitsRead > totalNumberBits) throw new ArgumentException("Pushing too many bits", nameof(bitsRead));
#endif            
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ResetReadPosition()
        {
            scratchUsedBits = 0;
            chunkIndex = 0;
            scratch = 0;
        }        

    }
}