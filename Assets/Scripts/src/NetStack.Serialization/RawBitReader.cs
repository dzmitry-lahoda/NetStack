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
    public class RawBitReader<TStorage> : BitBuffer<TStorage>, IRawReader
        where TStorage : IMemory<u32>
    {
        public RawBitReader(TStorage storage)
        {
            Chunks = storage;
        }

        public bool CanReadMore => totalNumberBits > BitsRead;

        // total count of used bits since buffer start
        public u32 BitsRead
        {
            get
            {
                var indexInBits = chunkIndex * 32;
                var over = scratchUsedBits != 0 ? 1 : 0; // TODO: speed up with bit hacking
                return (u32)(indexInBits - over * scratchUsedBits);
            }
        }

        /// <summary>
        /// Reads one bit boolean.
        /// </summary>        
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public bool b()
        {
#if !NO_EXCEPTIONS
            if (BitsRead >= totalNumberBits) Throw.InvalidOperation("reading more bits than in buffer");
            if (scratchUsedBits < 1 && chunkIndex >= totalNumChunks) Throw.InvalidOperation("reading more than buffer size");
#endif
            if (scratchUsedBits < 1) SIndexInc();
#if !NO_EXCEPTIONS
            if (scratchUsedBits == 0) Throw.InvalidOperation("Too many bits requested from scratch");
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
        public u32 u32(u8 numberOfBits)
        {
#if !NO_EXCEPTIONS
            if (numberOfBits <= 0 || numberOfBits > 32) Throw.ArgumentOutOfRange(nameof(numberOfBits), $"Should read from 1 to {32}. Cannot read {numberOfBits}");
            if (BitsRead + numberOfBits > totalNumberBits) Throw.InvalidOperation("reading more bits than in buffer");
            if (scratchUsedBits < 0 || scratchUsedBits > 64) Throw.InvalidProgram($"{scratchUsedBits} Too many bits used in scratch, Overflow?");
#endif
            if (scratchUsedBits < numberOfBits) SIndexInc();
#if !NO_EXCEPTIONS
            if (scratchUsedBits < numberOfBits) Throw.InvalidOperation("Too many bits requested from scratch");
#endif
            u32 output = (u32)(scratch & ((((u64)1) << numberOfBits) - 1));
            scratch >>= numberOfBits;
            scratchUsedBits -= numberOfBits;

            return output;
        }

        /// <summary>
        /// Reads raw data.
        /// </summary>
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public u8 u8Raw()
        {
#if !NO_EXCEPTIONS
            if (BitsRead + 8 > totalNumberBits) Throw.InvalidOperation("reading more bits than in buffer");
            if (scratchUsedBits < 0 || scratchUsedBits > 64) Throw.InvalidProgram($"{scratchUsedBits} Too many bits used in scratch, Overflow?");
#endif
            if (scratchUsedBits < 8) SIndexInc();
#if !NO_EXCEPTIONS
            if (scratchUsedBits < 8) Throw.InvalidOperation("Too many bits requested from scratch");
#endif
            u32 output = (u32)(scratch & ((((u64)1) << 8) - 1));
            scratch >>= 8;
            scratchUsedBits -= 8;

            return (u8)output;
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        private void SIndexInc()
        {
#if !NO_EXCEPTIONS
            if (chunkIndex >= totalNumChunks) Throw.InvalidOperation("reading more than buffer size");
#endif
            scratch |= ((u64)(chunks[chunkIndex])) << scratchUsedBits;
            scratchUsedBits += 32;
            chunkIndex++;
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public void SetPosition(u32 bitsRead)
        {
#if !NO_EXCEPTIONS
            if (bitsRead < 0) Throw.Argument("Pushing negative bits", nameof(bitsRead));
            if (bitsRead > totalNumberBits) Throw.Argument("Pushing too many bits", nameof(bitsRead));
#endif
            chunkIndex =  (u16)(bitsRead / (4 * 8));
            scratchUsedBits = (i32) (bitsRead % 32u);
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

        /// <inheritdoc/>
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public u16 u16(u8 numberOfBits)
        {
#if !NO_EXCEPTIONS
            if (numberOfBits <= 0 || numberOfBits > 16) Throw.ArgumentOutOfRange(nameof(numberOfBits), $"Should read from 1 to {16}. Cannot read {numberOfBits}");
            if (BitsRead + numberOfBits > totalNumberBits) Throw.InvalidOperation("reading more bits than in buffer");
            if (scratchUsedBits < 0 || scratchUsedBits > 64) Throw.InvalidProgram($"{scratchUsedBits} Too many bits used in scratch, Overflow?");
#endif            
            if (scratchUsedBits < numberOfBits) SIndexInc();
#if !NO_EXCEPTIONS
            if (scratchUsedBits < numberOfBits) Throw.InvalidOperation("Too many bits requested from scratch");
#endif            
            u16 output = (u16)(scratch & ((((u64)1) << numberOfBits) - 1));
            scratch >>= numberOfBits;
            scratchUsedBits -= numberOfBits;
            return output;
        }

        /// <inheritdoc/>
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public u8 u8(u8 numberOfBits)
        {
#if !NO_EXCEPTIONS
            if (numberOfBits <= 0 || numberOfBits > 8) Throw.ArgumentOutOfRange(nameof(numberOfBits), $"Should read from 1 to {8}. Cannot read {numberOfBits}");
            if (BitsRead + numberOfBits > totalNumberBits) Throw.InvalidOperation("reading more bits than in buffer");
            if (scratchUsedBits < 0 || scratchUsedBits > 64) Throw.InvalidProgram($"{scratchUsedBits} Too many bits used in scratch, Overflow?");
#endif            
            if (scratchUsedBits < numberOfBits) SIndexInc();
#if !NO_EXCEPTIONS
            if (scratchUsedBits < numberOfBits) Throw.InvalidOperation("Too many bits requested from scratch");
#endif            
            u8 output = (u8)(scratch & ((((u64)1) << numberOfBits) - 1));
            scratch >>= numberOfBits;
            scratchUsedBits -= numberOfBits;
            return output;
        }        
    }
}