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
    partial class BitBufferWriter<T> : IRawWriter
        where T : unmanaged, ICompression<BitBufferWriter<T>> // https://en.wikipedia.org/wiki/Curiously_recurring_template_pattern
    {
        // true if has not capacity to write numberOfBits
        public bool CannotAdd(i32 numberOfBits) => BitsWritten + numberOfBits > state.totalNumberBits;

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
                var indexInBits = state.chunkIndex * 32;
                var over = state.scratchUsedBits != 0 ? 1 : 0; // TODO: speed up with bit hacking
                return indexInBits + over * Math.Abs(state.scratchUsedBits);
            }
        }

        /// <summary>
        /// Hom much bits can be yet written into buffer before it cannot add bits more.
        /// </summary>
        public i32 BitsAvailable => state.totalNumberBits - BitsWritten;

        /// <summary>
        /// Store value in specified number of bits.
        /// </summary>
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public void raw(u32 value, i32 numberOfBits)
        {
#if !NO_EXCEPTIONS
            if (numberOfBits <= 0)
                Throw.ArgumentOutOfRange($"{nameof(numberOfBits)} should be positive", nameof(numberOfBits));

            if (numberOfBits > 32) // Unsafe.Sizeof<uint>() * 8
                Throw.ArgumentOutOfRange($"{nameof(numberOfBits)} should be less than or equal to 32", nameof(numberOfBits));

            if (BitsWritten + numberOfBits > state.totalNumberBits)
                Throw.InvalidOperation($"Writing {numberOfBits} bits will exceed maximal capacity of {state.totalNumberBits}, while {BitsWritten} bits written");

            if (value > (u32)((1ul << numberOfBits) - 1))
                Throw.Argument(nameof(value), $"{value} is too big, won't fit in requested {numberOfBits} number of bits");
#endif
            internalRaw(value, numberOfBits);
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        private void internalRaw(u32 value, i32 numberOfBits)
        {
            value &= (u32)((1ul << numberOfBits) - 1);
            state.scratch |= ((u64)value) << state.scratchUsedBits;
            // maintain with bool index increase, do not reuse - looses 5% of performance on .NET Core event if AggressiveInlining
            state.scratchUsedBits += numberOfBits;
            SIndexInc();
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public void u8(u8 value)
        {
            state.scratch |= ((u64)value) << state.scratchUsedBits;
            state.scratchUsedBits += 8;
            SIndexInc();
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        private void SIndexInc()
        {
            if (state.scratchUsedBits >= 32)
            {
#if !NO_EXCEPTIONS
                if (state.chunkIndex >= state.totalNumChunks) Throw.IndexOutOfRange("Pushing failed, buffer is full.");
#endif
                // TODO: how much it will cost to cast ref byte into ref uint and set scratch (to allow FromArray with no copy)
                // TODO: will it be improvement to for chunks to be (u)long?
                state.chunks[state.chunkIndex] = (u32)(state.scratch);
                state.scratch >>= 32;
                state.scratchUsedBits -= 32;
                state.chunkIndex++;
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
            if (value) state.scratch |= 1ul << state.scratchUsedBits;
            state.scratchUsedBits += 1;
            SIndexInc();
        }

        /// <summary>
        /// Adds value 7 bit encoded value.
        /// Store seven right bits, if more than 8 with 1, then set 1 to continue.
        /// </summary>
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public void u32(u32 value)
        {
            T encoder = default;
            encoder.u32(this, value);
        }

        /// <summary>
        /// Store value ZigZag and 7 bits encoded.
        /// </summary>
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public void i32(i32 value)
        {
            T encoder = default;
            encoder.i32(this, value);
        }

        /// <summary>
        /// Store value ZigZag encoded in number of bits.
        /// </summary>
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public void i32(i32 value, i32 numberOfBits)
        {
            T encoder = default;
            encoder.i32(this, value, numberOfBits);
        }
    }
}