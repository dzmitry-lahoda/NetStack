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
using System.Diagnostics;
using System.Numerics;

namespace NetStack.Serialization
{
    partial class BitBufferReader<T> : IRawReader
         where T:unmanaged, IDecompression<BitBufferReader<T>> 
    {        
        public bool CanReadMore => state.totalNumberBits > BitsRead;
    
        // total count of used bits since buffer start
        public i32 BitsRead 
        {
            get 
            {
                var indexInBits = state.chunkIndex * 32;
                var over = state.scratchUsedBits != 0 ? 1 : 0; // TODO: speed up with bit hacking
                return indexInBits - over * state.scratchUsedBits;
            }            
        }

        /// <summary>
        /// Reads one bit boolean.
        /// </summary>        
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public bool b()
        {
#if !NO_EXCEPTIONS
            if (BitsRead >= state.totalNumberBits) Throw.InvalidOperation("reading more bits than in buffer");
            if (state.scratchUsedBits < 1 && state.chunkIndex >= state.totalNumChunks) Throw.InvalidOperation("reading more than buffer size");
#endif
            if (state.scratchUsedBits < 1)
            {
                state.scratch |= ((u64)(state.chunks[state.chunkIndex])) << state.scratchUsedBits;
                state.scratchUsedBits += 32;
                state.chunkIndex++;
            }

#if DEBUG
            if (state.scratchUsedBits == 0) Throw.InvalidOperation("Too many bits requested from scratch");
#endif
            u32 output = (u32)(state.scratch & 1);

            state.scratch >>= 1;
            state.scratchUsedBits -= 1;

            return output > 0;
        }

        /// <summary>
        /// Reads raw data.
        /// </summary>
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public u32 raw(i32 numberOfBits)
        {
#if !NO_EXCEPTIONS
            if (numberOfBits <= 0 || numberOfBits > 32) Throw.ArgumentOutOfRange(nameof(numberOfBits), $"Should read from 1 to 32. Cannot read {numberOfBits}"); 
            if (BitsRead + numberOfBits > state.totalNumberBits)Throw.InvalidOperation("reading more bits than in buffer");
            if (state.scratchUsedBits < 0 || state.scratchUsedBits > 64) Throw.InvalidProgram($"{state.scratchUsedBits} Too many bits used in scratch, Overflow?");
#endif

            if (state.scratchUsedBits < numberOfBits)
            {
#if !NO_EXCEPTIONS                
                if (state.chunkIndex >= state.totalNumChunks) Throw.InvalidOperation("reading more than buffer size");
#endif
                state.scratch |= ((u64)(state.chunks[state.chunkIndex])) << state.scratchUsedBits;
                state.scratchUsedBits += 32;
                state.chunkIndex++;
            }

#if DEBUG
            if (state.scratchUsedBits < numberOfBits) Throw.InvalidOperation("Too many bits requested from scratch");
#endif
            u32 output = (u32)(state.scratch & ((((u64)1) << numberOfBits) - 1));

            state.scratch >>= numberOfBits;
            state.scratchUsedBits -= numberOfBits;

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
#if !NO_EXCEPTIONS        
        if (bitsRead < 0) Throw.Argument("Pushing negative bits", nameof(bitsRead));
        if (bitsRead > state.totalNumberBits) Throw.Argument("Pushing too many bits", nameof(bitsRead));
#endif            
           state.chunkIndex = bitsRead / 32;
           state.scratchUsedBits = bitsRead % 32;
           if (state.scratchUsedBits != 0)
           {
               state.scratch = ((u64)(state.chunks[state.chunkIndex])) >> state.scratchUsedBits;
               state.chunkIndex += 1;
           }
           else
           {
               state.scratch = 0;
           }
        }
    }
}