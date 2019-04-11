﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Buffers;
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
    public partial class BitBufferReader
    {
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

            Clear();

            var step = Unsafe.SizeOf<uint>();
            int numChunks = ((length - position) / step) + 1;

            if (chunks.Length < numChunks)
            {
                Chunks = new uint[numChunks]; // call it once to stay expanded forever
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
        }
        public void FromSpan(ReadOnlySpan<byte> data) => FromSpan(data, data.Length);

        public void FromSpan(ReadOnlySpan<byte> data, int length)
        {
            // may throw here as not hot path
            if (length <= 0)
                throw new ArgumentException("Should be positive", nameof(length));
            if (data.Length <= 0)
                throw new ArgumentException("Should be positive", nameof(data.Length));
                
            Clear();
            var step = Unsafe.SizeOf<uint>();
            int numChunks = (length / step) + 1;

            if (chunks.Length < numChunks)
            {
                Chunks = new uint[numChunks];
            }

            for (int i = 0; i < numChunks; i++)
            {
                int dataIdx = i * step;
                uint chunk = 0;
                // TODO: ref into data and do block copy of all 4bytes, copy only last 3 bytes by hand
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
        }
    }
}