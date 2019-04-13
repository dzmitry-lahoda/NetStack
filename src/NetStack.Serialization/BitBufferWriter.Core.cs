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
    partial class BitBufferWriter<T> : IRawWriter where T:unmanaged, ICompression<BitBufferWriter<T>> 
    {
        // true if has not capacity to write numberOfBits
        public bool CannotAdd(int numberOfBits) => BitsWritten + numberOfBits > totalNumberBits;

        /// <summary>
        /// Count of written bytes.
        /// </summary>
        public int LengthWritten => ((BitsWritten - 1) >> 3) + 1;

        // total count of used bits since buffer start
        public int BitsWritten
        {
            get
            {
                var indexInBits = chunkIndex * 32;
                var over = scratchUsedBits != 0 ? 1 : 0; // TODO: speed up with bit hacking
                return indexInBits + over * Math.Abs(scratchUsedBits);
            }
        }

        /// <summary>
        /// Call after all write commands.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Finish()
        {
            if (scratchUsedBits != 0)
            {
                #if DEBUG || NETSTACK_VALIDATE
                if (chunkIndex >= totalNumChunks) throw new IndexOutOfRangeException("buffer overflow when trying to finalize stream");
                #endif
                chunks[chunkIndex] = (u32)(scratch & 0xFFFFFFFF);
                scratch >>= 32;
                scratchUsedBits -= 32;
                chunkIndex++;
            }
        }

        /// <summary>
        /// Hom much bits can be yet written into buffer before it <see cref="IsReadFinished"/>.
        /// </summary>
        public int BitsAvailable => totalNumberBits - BitsWritten;

        /// <summary>
        /// Store value in specified number of bits.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void raw(u32 value, int numberOfBits)
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
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void internalRaw(u32 value, int numberOfBits)
        {
            value &= (u32)((1ul << numberOfBits) - 1);

            scratch |= ((ulong)value) << scratchUsedBits;

            // maintain with bool index increase, do not reuse - looses 5% of performance on .NET Core event if AggressiveInlining
            scratchUsedBits += numberOfBits;

            if (scratchUsedBits >= 32)
            {
                #if DEBUG || NETSTACK_VALIDATE
                    if (chunkIndex >= totalNumChunks) throw new IndexOutOfRangeException("Pushing failed, buffer is full.");
                #endif                
                // TODO: how much it will cost to cast ref byte into ref uint and set scratch (to allow FromArray with no copy)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void b(bool value)
        {
            if (value)
                scratch |= 1ul << scratchUsedBits;

            scratchUsedBits += 1;

            if (scratchUsedBits >= 32)
            {
                Debug.Assert(chunkIndex < totalNumChunks, "Pushing failed, buffer is full.");
                // TODO: will it be improvement to for chunks to be (u)long?
                chunks[chunkIndex] = (u32)(scratch);
                scratch >>= 32;
                scratchUsedBits -= 32;
                chunkIndex++;
            }
        }

        /// <summary>
        /// Adds value 7 bit encoded value.
        /// Store seven right bits, if more than 8 with 1, then set 1 to continue.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void u32(u32 value)
        {
            T encoder = default;
            encoder.u32(this, value);
        }

        /// <summary>
        /// Store value ZigZag and 7 bits encoded.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void i32(int value)
        {
            T encoder = default;
            encoder.i32(this, value);
        }

        /// <summary>
        /// Store value ZigZag encoded in number of bits.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void i32(int value, int numberOfBits)
        {
            T encoder = default;
            raw(encoder.encode(value), numberOfBits);
        }
    }
}