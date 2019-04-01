

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
        private int bitsWritten;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(int numberOfBits, uint value)
        {
#if DEBUG // allows to switch off checks in production
            if (numberOfBits <= 0)
                throw new ArgumentOutOfRangeException($"{nameof(numberOfBits)} should be positive", nameof(numberOfBits));

            if (numberOfBits > 32) // Unsafe.Sizeof<uint>() * 8
                throw new ArgumentOutOfRangeException($"{nameof(numberOfBits)} should be less than or equal to 32", nameof(numberOfBits));

            if (bitsWritten + numberOfBits > totalNumberBits)
                throw new InvalidOperationException($"Writing {numberOfBits} bits will exceed maximal capacity of {totalNumberBits}, while {bitsWritten} bits written");

            if (value > (uint)((1ul << numberOfBits) - 1))
                throw new InvalidOperationException($"{value} is too big, won't fit in requested {numberOfBits} number of bits");
#endif
            value &= (uint)((1ul << numberOfBits) - 1);

            scratch |= ((ulong)value) << scratchUsedBits;

            // maintain with bool index increase, do not reuse - looses 5% of performance on .NET Core event if AggressiveInlining
            scratchUsedBits += numberOfBits;

            if (scratchUsedBits >= 32)
            {
                Debug.Assert(chunkIndex < totalNumChunks, "Pushing failed, buffer is full.");
                // TODO: how much it will cost to cast ref byte into ref uint and set scratch (to allow FromArray with no copy)
                chunks[chunkIndex] = (uint)(scratch);
                scratch >>= 32;
                scratchUsedBits -= 32;
                chunkIndex++;
            }

            bitsWritten += numberOfBits;
        }

        // TODO: add special handling for https://en.wikipedia.org/wiki/Nibble or other stuff unity uses fir skips 
        // https://github.com/Unity-Technologies/FPSSample  https://www.youtube.com/watch?v=k6JTaFE7SYI
        //      Method |     N |     Mean |     Error |    StdDev |   Median |
        // ----------- |------ |---------:|----------:|----------:|---------:|
        //  BoolViaInt | 10000 | 1.998 ms | 0.0666 ms | 0.1943 ms | 1.942 ms |
        //    BoolFast | 10000 | 1.592 ms | 0.0493 ms | 0.1429 ms | 1.564 ms |        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddBool(bool value)
        {
            if (value)
                scratch |= 1ul << scratchUsedBits;

            scratchUsedBits += 1;

            if (scratchUsedBits >= 32)
            {
                Debug.Assert(chunkIndex < totalNumChunks, "Pushing failed, buffer is full.");
                // TODO: will it be improvement to for chunks to be (u)long?
                chunks[chunkIndex] = (uint)(scratch);
                scratch >>= 32;
                scratchUsedBits -= 32;
                chunkIndex++;
            }

            bitsWritten += 1;
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitBuffer AddUInt(uint value)
        {
            do
            {
                // store seven right bits, if more than 8 with 1, then set 1 to continue
                var buffer = value & 0b0111_1111u;
                value >>= 7;

                if (value > 0)
                    buffer |= 0b1000_0000u;

                Add(8, buffer);
            }
            while (value > 0);

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitBuffer AddInt(int value)
        {
            uint zigzag = (uint)((value << 1) ^ (value >> 31));
            AddUInt(zigzag);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitBuffer AddInt(int value, int numBits)
        {
            uint zigzag = (uint)((value << 1) ^ (value >> 31));
            Add(numBits, zigzag);
            return this;
        }
    }
}