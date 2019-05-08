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
using System.Buffers;
using System.Diagnostics;
using System.Numerics;

namespace NetStack.Serialization
{
    public class RawBitWriter<TMemory> : BitBuffer<TMemory>, IRawWriter
       where  TMemory : struct, IMemory<u32>
    {
        public RawBitWriter(TMemory chunks)
        {
            if (default(TMemory).Equals(chunks) || chunks.Length == 0)
                Throw.Argument("Buffer should be non null or empty", nameof(chunks));
            Chunks = chunks;
            Reset();
        }

        /// <summary>
        /// Count of written bytes.
        /// </summary>
        public i32 LengthWritten => ((BitsWritten - 1) >> 3) + 1;

        /// <summary>
        /// Gets total count of used bits since buffer start.
        /// </summary>
        public i32 BitsWritten
        {
            get
            {
                var indexInBits = chunkIndex * 32;
                var over = scratchUsedBits != 0 ? 1 : 0; // TODO: speed up with bit hacking
                return indexInBits + over * Math.Abs(scratchUsedBits);
            }
        }

        /// <inheritdoc/>
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public void u32(u32 value, u8 numberOfBits)
        {
#if !NO_EXCEPTIONS
            if (numberOfBits <= 0)
                Throw.ArgumentOutOfRange($"{nameof(numberOfBits)} should be positive", nameof(numberOfBits));

            if (numberOfBits > 32) // Unsafe.Sizeof<u32>() * 8
                Throw.ArgumentOutOfRange($"{nameof(numberOfBits)} should be less than or equal to 32", nameof(numberOfBits));

            if (BitsWritten + numberOfBits > totalNumberBits)
                Throw.InvalidOperation($"Writing {numberOfBits} bits will exceed maximal capacity of {totalNumberBits}, while {BitsWritten} bits written");

            if (value > (u32)((1ul << numberOfBits) - 1))
                Throw.Argument(nameof(value), $"{value} is too big, won't fit in requested {numberOfBits} number of bits");
#endif
            internalRaw(value, numberOfBits);
        }

        /// <inheritdoc/>
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public void u16(u16 value, u8 numberOfBits) 
        {
#if !NO_EXCEPTIONS
            if (numberOfBits <= 0)
                Throw.ArgumentOutOfRange($"{nameof(numberOfBits)} should be positive", nameof(numberOfBits));

            if (numberOfBits > 16) // Unsafe.Sizeof<u16>() * 8
                Throw.ArgumentOutOfRange($"{nameof(numberOfBits)} should be less than or equal to 16", nameof(numberOfBits));

            if (BitsWritten + numberOfBits > totalNumberBits)
                Throw.InvalidOperation($"Writing {numberOfBits} bits will exceed maximal capacity of {totalNumberBits}, while {BitsWritten} bits written");

            if (value > (u16)((1ul << numberOfBits) - 1))
                Throw.Argument(nameof(value), $"{value} is too big, won't fit in requested {numberOfBits} number of bits");
#endif            
            var part = value & (u32)((1ul << numberOfBits) - 1);
            scratch |= ((u64)part) << scratchUsedBits;
            scratchUsedBits += numberOfBits;
            SIndexInc();
        }

        /// <inheritdoc/>
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public void u8(u8 value, u8 numberOfBits) 
        {
#if !NO_EXCEPTIONS
            if (numberOfBits <= 0)
                Throw.ArgumentOutOfRange($"{nameof(numberOfBits)} should be positive", nameof(numberOfBits));

            if (numberOfBits > 8) // Unsafe.Sizeof<u8>() * 8
                Throw.ArgumentOutOfRange($"{nameof(numberOfBits)} should be less than or equal to {8}", nameof(numberOfBits));

            if (BitsWritten + numberOfBits > totalNumberBits)
                Throw.InvalidOperation($"Writing {numberOfBits} bits will exceed maximal capacity of {totalNumberBits}, while {BitsWritten} bits written");

            if (value > (u16)((1ul << numberOfBits) - 1))
                Throw.Argument(nameof(value), $"{value} is too big, won't fit in requested {numberOfBits} number of bits");
#endif            
            var part = value & (u32)((1ul << numberOfBits) - 1);
            scratch |= ((u64)part) << scratchUsedBits;
            scratchUsedBits += numberOfBits;
            SIndexInc();
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        private void internalRaw(u32 value, i32 numberOfBits)
        {
            value &= (u32)((1ul << numberOfBits) - 1);
            scratch |= ((u64)value) << scratchUsedBits;
            // maintain with bool index increase, do not reuse - looses 5% of performance on .NET Core event if AggressiveInlining
            scratchUsedBits += numberOfBits;
            SIndexInc();
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public void u8Raw(u8 value)
        {
            scratch |= ((u64)value) << scratchUsedBits;
            scratchUsedBits += 8;
            SIndexInc();
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        private void SIndexInc()
        {
            if (scratchUsedBits >= 32)
            {
#if !NO_EXCEPTIONS
                if (chunkIndex >= totalNumChunks) Throw.IndexOutOfRange("Pushing failed, buffer is full.");
#endif
                // TODO: how much it will cost to cast ref byte into ref uint and set scratch (to allow FromArray with no copy)
                // TODO: will it be improvement to for chunks to be (u)long?
                chunks[chunkIndex] = (u32)(scratch);
                scratch >>= 32;
                scratchUsedBits -= 32;
                chunkIndex++;
            }
        }

        //      Method |     N |     Mean |     Error |    StdDev |   Median |
        // ----------- |------ |---------:|----------:|----------:|---------:|
        //  BoolViaInt | 10000 | 1.998 ms | 0.0666 ms | 0.1943 ms | 1.942 ms |
        //    BoolFast | 10000 | 1.592 ms | 0.0493 ms | 0.1429 ms | 1.564 ms |        
        /// <summary>
        /// Writes boolean value into buffer.
        /// </summary>
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public void b(bool value)
        {
            if (value) scratch |= 1ul << scratchUsedBits;
            scratchUsedBits += 1;
            SIndexInc();
        }      
    }
}