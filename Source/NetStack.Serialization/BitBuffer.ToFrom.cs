﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
#if !(ENABLE_MONO || ENABLE_IL2CPP)
using System.Diagnostics;
using System.Numerics;
#else
using UnityEngine;
#endif

// Until migration to .NET Standard 2.1
using BitOperations = System.Numerics.BitOperations;

namespace NetStack.Serialization
{
    // TODO: add custom visualizer here (like array one)
    public partial class BitBuffer
    {
        /// <summary>
        /// Dot not use for production. GC allocated array.
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray()
        {
            var data = new byte[Length];
            ToArray(data);
            return data;
        }

        /// <summary>
        /// Calls <see cref="Finish"/> and copies all internal data into array.
        /// </summary>
        /// <param name="data">The output buffer.</param>
        /// <returns>Count of bytes written.</returns>
        public int ToArray(byte[] data)
        {
            int length = data.Length;
            ToArray(data, 0, length);
            return Length;
        }

        public void ToArray(byte[] data, int position, int length)
        {
            // may throw here as not hot path
            if (length <= 0)
                throw new ArgumentException("Should be positive", nameof(length));
            if (position < 0)
                throw new ArgumentException("Should be non negative", nameof(position));
            var step = Unsafe.SizeOf<uint>();
            Add(1, 1);

            Finish();

            int numChunks = (bitsWritten >> 5) + 1;

            for (int i = 0; i < numChunks; i++)
            {
                int dataIdx = i * step;
                uint chunk = chunks[i];

                if (dataIdx < length)
                    data[position + dataIdx] = (byte)(chunk);

                if (dataIdx + 1 < length)
                    data[position + dataIdx + 1] = (byte)(chunk >> 8);

                if (dataIdx + 2 < length)
                    data[position + dataIdx + 2] = (byte)(chunk >> 16);

                if (dataIdx + 3 < length)
                    data[position + dataIdx + 3] = (byte)(chunk >> 24);
            }
        }

        public void FromArray(byte[] data)
        {
            int length = data.Length;
            FromArray(data, 0, length);
        }

        /// <summary>
        /// Copies data from array.
        /// </summary>
        public void FromArray(byte[] data, int position, int length)
        {
            // may throw here as not hot path
            if (length <= 0)
                throw new ArgumentException("Should be positive", nameof(length));
            if (position < 0)
                throw new ArgumentException("Should be non negative", nameof(position));

            Clean();

            var step = Unsafe.SizeOf<uint>();
            int numChunks = ((length - position) / step) + 1;

            if (chunks.Length < numChunks)
            {
                chunks = new uint[numChunks]; // call it once to stay expanded forever
                totalNumChunks = numChunks;// / 4;
                totalNumBits = numChunks * step * 8;
            }

            
            // data must be 4 or 8 bytes long because 32 and 64 machines https://gafferongames.com/post/reading_and_writing_packets/
            // TODO: possible to optimize to avoid copy? some kind of unsafe cast?
            // TODO: try ulong for performance as most of devices will be 64 bit?
            // https://github.com/nxrighthere/NetStack/issues/1#issuecomment-475212246
            for (int i = 0; i < numChunks; i++)
            {
                int dataIdx = i * step;
                uint chunk = 0;

                // may optimize by calculating variable her and doing zero init of remaining blocks
                // may reintepret unsafe as uint, and then if less than 3 then only read last as 1 2 3
                if (dataIdx < length)
                    chunk = (uint)data[position + dataIdx];

                if (dataIdx + 1 < length)
                    chunk = chunk | (uint)data[position + dataIdx + 1] << 8;

                if (dataIdx + 2 < length)
                    chunk = chunk | (uint)data[position + dataIdx + 2] << 16;

                if (dataIdx + 3 < length)
                    chunk = chunk | (uint)data[position + dataIdx + 3] << 24;

                chunks[i] = chunk;
            }

            var leadingZeros = BitOperations.LeadingZeroCount(data[position + length - 1]);
            bitsWritten = 8 * length - leadingZeros - 1;
            bitsRead = 0;
        }

        /// <summary>
        /// Calls <see cref="Finish"/> and copies all internal data into span.
        /// </summary>
        /// <param name="data">The output buffer.</param>
        /// <returns>Count of bytes written.</returns>
        public int ToSpan(ref Span<byte> data)
        {
            // may throw here as not hot path, check span length

            Add(1, 1);

            Finish();

            int numChunks = (bitsWritten >> 5) + 1;
            int length = data.Length;
            var step = Unsafe.SizeOf<uint>();
            for (int i = 0; i < numChunks; i++)
            {
                int dataIdx = i * step;
                uint chunk = chunks[i];

                if (dataIdx < length)
                    data[dataIdx] = (byte)(chunk);

                if (dataIdx + 1 < length)
                    data[dataIdx + 1] = (byte)(chunk >> 8);

                if (dataIdx + 2 < length)
                    data[dataIdx + 2] = (byte)(chunk >> 16);

                if (dataIdx + 3 < length)
                    data[dataIdx + 3] = (byte)(chunk >> 24);
            }

            return Length;
        }


        public void FromSpan(in ReadOnlySpan<byte> data) => FromSpan(in data, data.Length);

        public void FromSpan(in ReadOnlySpan<byte> data, int length)
        {
            // may throw here as not hot path
            if (length <= 0)
                throw new ArgumentException("Should be positive", nameof(length));
            if (data.Length <= 0)
                throw new ArgumentException("Should be positive", nameof(data.Length));
                
            Clean();
            var step = Unsafe.SizeOf<uint>();
            int numChunks = (length / step) + 1;

            if (chunks.Length < numChunks)
            {
                chunks = new uint[numChunks];
                totalNumChunks = numChunks;// / 4;
                totalNumBits = numChunks * step * 8;
            }

            for (int i = 0; i < numChunks; i++)
            {
                int dataIdx = i * step;
                uint chunk = 0;

                if (dataIdx < length)
                    chunk = (uint)data[dataIdx];

                if (dataIdx + 1 < length)
                    chunk = chunk | (uint)data[dataIdx + 1] << 8;

                if (dataIdx + 2 < length)
                    chunk = chunk | (uint)data[dataIdx + 2] << 16;

                if (dataIdx + 3 < length)
                    chunk = chunk | (uint)data[dataIdx + 3] << 24;

                chunks[i] = chunk;
            }

            var leadingZeros = BitOperations.LeadingZeroCount(data[length - 1]);
            bitsWritten = 8 * length - leadingZeros - 1;
            bitsRead = 0;
        }

        public override string ToString()
        {
            builder.Length = 0;

            for (int i = chunks.Length - 1; i >= 0; i--)
            {
                builder.Append(Convert.ToString(chunks[i], 2).PadLeft(32, '0'));
            }

            var spaced = new StringBuilder();

            for (int i = 0; i < builder.Length; i++)
            {
                spaced.Append(builder[i]);

                if (((i + 1) % 8) == 0)
                    spaced.Append(" ");
            }

            return spaced.ToString();
        }

    }
}