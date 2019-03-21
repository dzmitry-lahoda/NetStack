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
    // core untyped data specific part of bit buffer
    partial class BitBuffer
    {
        private int chunkIndex;
        private uint[] chunks;
        private int totalNumChunks;
        private int totalNumBits;
        private int bitsRead;
        private int bitsWritten;

        private int scratchUsedBits;
        private ulong scratch;

        // TODO: create separate method to read bool (see why AddBool faster)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Peek(int numBits)
        {
            Debug.Assert(numBits > 0, "reading negative bits");
            Debug.Assert(numBits <= 32, "reading too many bits");
            Debug.Assert(bitsRead + numBits <= totalNumBits, "reading more bits than in buffer");

            Debug.Assert(scratchUsedBits >= 0 && scratchUsedBits <= 64, "Too many bits used in scratch, Overflow?");

            if (scratchUsedBits < numBits)
            {
                Debug.Assert(chunkIndex < totalNumChunks, "reading more than buffer size");

                scratch |= ((ulong)(chunks[chunkIndex])) << scratchUsedBits;
                scratchUsedBits += 32;
                chunkIndex++;
            }

            Debug.Assert(scratchUsedBits >= numBits, "Too many bits requested from scratch");

            uint output = (uint)(scratch & ((((ulong)1) << numBits) - 1));

            scratch >>= numBits;
            scratchUsedBits -= numBits;

            return output;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(int numBits, uint value)
        {
            Debug.Assert(numBits > 0, "Pushing negative or zero bits");
            Debug.Assert(numBits <= 32, "Pushing too many bits");
            Debug.Assert(bitsWritten + numBits <= totalNumBits, "Pushing failed, buffer is full.");
            Debug.Assert(value <= (uint)((1ul << numBits) - 1), "value too big, won't fit in requested number of bits."); // 

            value &= (uint)((1ul << numBits) - 1);

            scratch |= ((ulong)value) << scratchUsedBits;

            // maintain with bool index increase, do not reuse - looses 5% of performance on .NET Core event if AggressiveInlining
            scratchUsedBits += numBits;

            if (scratchUsedBits >= 32)
            {
                Debug.Assert(chunkIndex < totalNumChunks, "Pushing failed, buffer is full.");
                // TODO: how much it will cost to cast ref byte into ref uint and set scratch (to allow FromArray with no copy)
                chunks[chunkIndex] = (uint)(scratch);
                scratch >>= 32;
                scratchUsedBits -= 32;
                chunkIndex++;
            }

            bitsWritten += numBits;
        }

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
                // TODO: how much it will cost to cast ref byte into ref uint and set scratch (to allow FromArray with no copy)
                chunks[chunkIndex] = (uint)(scratch);
                scratch >>= 32;
                scratchUsedBits -= 32;
                chunkIndex++;
            }

            bitsWritten += 1;
        }        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int PeekInt()
        {
            uint value = PeekUInt();
            int zagzig = (int)((value >> 1) ^ (-(int)(value & 1)));
            return zagzig;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int PeekInt(int numBits)
        {
            uint value = Peek(numBits);
            int zagzig = (int)((value >> 1) ^ (-(int)(value & 1)));
            return zagzig;
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
        public uint ReadUInt()
        {
            uint buffer = 0x0u;
            uint value = 0x0u;
            int shift = 0;

            do
            {
                buffer = Read(8);

                value |= (buffer & 0b0111_1111u) << shift;
                shift += 7;
            }
            while ((buffer & 0b1000_0000u) > 0);

            return value;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadInt()
        {
            uint value = ReadUInt();
            int zagzig = (int)((value >> 1) ^ (-(int)(value & 1)));
            return zagzig;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadInt(int numBits)
        {
            uint value = Read(numBits);
            int zagzig = (int)((value >> 1) ^ (-(int)(value & 1)));
            return zagzig;
        }
    }
}