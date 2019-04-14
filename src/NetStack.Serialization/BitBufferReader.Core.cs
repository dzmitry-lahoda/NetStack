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
    partial class BitBufferReader<T> : IRawReader
         where T:unmanaged, IDecompression<BitBufferReader<T>> 
    {        
        public bool CanReadMore => totalNumberBits > BitsRead;
    
        // total count of used bits since buffer start
        public i32 BitsRead 
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
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public bool b()
        {
#if DEBUG || NETSTACK_VALIDATE
            if (BitsRead >= totalNumberBits) throw InvalidOperation("reading more bits than in buffer");
            if (scratchUsedBits < 1 && chunkIndex >= totalNumChunks) throw InvalidOperation("reading more than buffer size");
#endif
            if (scratchUsedBits < 1)
            {
                scratch |= ((u64)(chunks[chunkIndex])) << scratchUsedBits;
                scratchUsedBits += 32;
                chunkIndex++;
            }

#if DEBUG
            if (scratchUsedBits == 0) throw InvalidOperation("Too many bits requested from scratch");
#endif
            u32 output = (u32)(scratch & 1);

            scratch >>= 1;
            scratchUsedBits -= 1;

            return output > 0;
        }

        /// <summary>
        /// Reads raw data.
        /// </summary>
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public u32 raw(i32 numberOfBits)
        {
#if DEBUG || NETSTACK_VALIDATE
            if (numberOfBits <= 0 || numberOfBits > 32) throw ArgumentOutOfRange(nameof(numberOfBits), $"Should read from 1 to 32. Cannot read {numberOfBits}"); 
            if (BitsRead + numberOfBits > totalNumberBits)throw InvalidOperation("reading more bits than in buffer");
            if (scratchUsedBits < 0 || scratchUsedBits > 64) throw InvalidProgram($"{scratchUsedBits} Too many bits used in scratch, Overflow?");
#endif

            if (scratchUsedBits < numberOfBits)
            {
#if DEBUG || NETSTACK_VALIDATE                
                if (chunkIndex >= totalNumChunks) throw InvalidOperation("reading more than buffer size");
#endif
                scratch |= ((u64)(chunks[chunkIndex])) << scratchUsedBits;
                scratchUsedBits += 32;
                chunkIndex++;
            }

#if DEBUG
            if (scratchUsedBits < numberOfBits) throw InvalidOperation("Too many bits requested from scratch");
#endif
            u32 output = (u32)(scratch & ((((u64)1) << numberOfBits) - 1));

            scratch >>= numberOfBits;
            scratchUsedBits -= numberOfBits;

            return output;
        }        

        /// <summary>
        /// Reads 7 bit encoded uint value.
        /// </summary>
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public u32 u32()
        {
            T decoder = default;
            return decoder.u32(this);
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public i32 i32()
        {
            T dencoder = default;
            return dencoder.i32(this);
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public i32 i32(i32 numberOfBits)
        {
            T encoder = default;
            return encoder.i32(this, numberOfBits);
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public void SetPosition(i32 bitsRead)
        {
#if DEBUG || NETSTACK_VALIDATE        
        if (bitsRead < 0) throw Argument("Pushing negative bits", nameof(bitsRead));
        if (bitsRead > totalNumberBits) throw Argument("Pushing too many bits", nameof(bitsRead));
#endif            
           chunkIndex = bitsRead / 32;
           scratchUsedBits = bitsRead % 32;
           if (scratchUsedBits != 0)
           {
               scratch = ((u64)(chunks[chunkIndex])) >> scratchUsedBits;
               chunkIndex += 1;
           }
           else
           {
               scratch = 0;
           }
        }
    }
}