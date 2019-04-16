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
    partial class BitBufferWriter<T> : IRawWriter 
        where T:unmanaged, ICompression<BitBufferWriter<T>> // https://en.wikipedia.org/wiki/Curiously_recurring_template_pattern
    {
        // true if has not capacity to write numberOfBits
        public bool CannotAdd(i32 numberOfBits) => BitsWritten + numberOfBits > totalNumberBits;

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

        /// <summary>
        /// Hom much bits can be yet written into buffer before it cannot add bits more.
        /// </summary>
        public i32 BitsAvailable => totalNumberBits - BitsWritten;

        /// <summary>
        /// Store value in specified number of bits.
        /// </summary>
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public void raw(u32 value, i32 numberOfBits)
        {
#if DEBUG || NETSTACK_VALIDATE
            if (numberOfBits <= 0)
                throw ArgumentOutOfRange($"{nameof(numberOfBits)} should be positive", nameof(numberOfBits));

            if (numberOfBits > 32) // Unsafe.Sizeof<uint>() * 8
                throw ArgumentOutOfRange($"{nameof(numberOfBits)} should be less than or equal to 32", nameof(numberOfBits));

            if (BitsWritten + numberOfBits > totalNumberBits)
                throw InvalidOperation($"Writing {numberOfBits} bits will exceed maximal capacity of {totalNumberBits}, while {BitsWritten} bits written");

            if (value > (u32)((1ul << numberOfBits) - 1))
                throw Argument(nameof(value), $"{value} is too big, won't fit in requested {numberOfBits} number of bits");
#endif
            internalRaw(value, numberOfBits);
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
        public void u8(u8 value)
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
#if DEBUG || NETSTACK_VALIDATE
                if (chunkIndex >= totalNumChunks) throw IndexOutOfRange("Pushing failed, buffer is full.");
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