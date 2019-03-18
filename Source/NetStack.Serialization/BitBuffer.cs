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
    // TODO: Split into BitBuffer.Core for maintenance
    /// <summary>
    /// Bit level compression by ranged values. String  UTF-16 support.
    /// </summary>
    // TODO: add custom visualizer here (like array one)
    public partial class BitBuffer
    {
        private const int defaultCapacity = 375; // 375 * 4 = 1500 bytes default MTU. don't have to grow.

        private const int defaultByteArrLengthBits = 9;

        private const int defaultStringLengthBits = 8;

        private int byteArrLengthMax;

        public int ByteArrLengthMax => byteArrLengthMax;

        private int byteArrLengthBits;

        public static int BitsRequired(int min, int max) =>
            (min == max) ? 1 : BitOperations.Log2((uint)(max - min)) + 1;

        public static int BitsRequired(uint min, uint max) =>
            (min == max) ? 1 : BitOperations.Log2(max - min) + 1;

        /// <summary>
        /// Creates new instance with its own bufffer.
        /// </summary>
        /// <param name="capacity">Count of 4 byte integers used as internal buffer.</param>
        /// <param name="stringLengthBits">Bits used to store length of strings.</param>
        /// <param name="byteArrLengthBits">Bits used to store length of byte arrays.</param>
        public BitBuffer(int capacity = defaultCapacity, int stringLengthBits = defaultStringLengthBits, int byteArrLengthBits = defaultByteArrLengthBits)
        {
            bitsRead = 0;
            bitsWritten = 0;
            chunks = new uint[capacity];
            totalNumChunks = capacity;// / 4;
            totalNumBits = capacity * 32;
            chunkIndex = 0;
            scratch = 0;
            scratchUsedBits = 0;
            this.byteArrLengthBits = byteArrLengthBits;
            byteArrLengthMax = (1 << byteArrLengthBits) - 1;
            this.stringLengthBits = stringLengthBits;
            stringLengthMax = (1 << stringLengthBits) - 1;
            builder = new StringBuilder(stringLengthMax);
        }

        /// <summary>
        /// Count of written bytes.
        /// </summary>
        public int Length => ((bitsWritten - 1) >> 3) + 1;

        public int LengthInBits => bitsWritten;

        public bool IsFinished => bitsWritten == bitsRead;

        public int BitsRead => bitsRead;

        public int BitsAvailable => totalNumBits - bitsWritten;

        public bool WouldOverflow(int bits) => bitsRead + bits > totalNumBits;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            bitsRead = 0;
            bitsWritten = 0;

            chunkIndex = 0;
            scratch = 0;
            scratchUsedBits = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ResetReadPosition()
        {
            Finish();
            bitsRead = 0;
        }

        // TODO: change API to be more safe on bit buffer operations (protect from misuse)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetReadPosition(int bitsRead)
        {
            Debug.Assert(bitsRead >= 0, "Pushing negative bits");
            Debug.Assert(bitsRead <= totalNumBits, "Pushing too many bits");
            this.bitsRead = bitsRead;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Read(int numBits)
        {
            uint result = Peek(numBits);
            bitsRead += numBits;
            return result;
        }

        /// <summary>
        /// Call after all <see cref="Add"/> commands.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Finish()
        {
            if (scratchUsedBits != 0)
            {
                Debug.Assert(chunkIndex < totalNumChunks, "buffer overflow when trying to finalize stream");
                chunks[chunkIndex] = (uint)(scratch & 0xFFFFFFFF);
                scratch >>= 32;
                scratchUsedBits -= 32;
                chunkIndex++;
            }
        }

        /// <summary>
        /// Calls <see cref="Finish"/> and copies all internal data into array.
        /// </summary>
        /// <param name="data">The output buffer.</param>
        /// <returns>Count of bytes written.</returns>
        public int ToArray(byte[] data)
        {
            Add(1, 1);

            Finish();

            int numChunks = (bitsWritten >> 5) + 1;
            int length = data.Length;

            for (int i = 0; i < numChunks; i++)
            {
                int dataIdx = i * 4;
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

        public void FromArray(byte[] data, int length)
        {
            int numChunks = (length / 4) + 1;

            if (chunks.Length < numChunks)
            {
                chunks = new uint[numChunks];
                totalNumChunks = numChunks;// / 4;
                totalNumBits = numChunks * 32;
            }

            for (int i = 0; i < numChunks; i++)
            {
                int dataIdx = i * 4;
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

            int positionInByte = 8 - BitOperations.LeadingZeroCount(data[length - 1]);

            bitsWritten = ((length - 1) * 8) + (positionInByte - 1);
            bitsRead = 0;
        }

        /// <summary>
        /// Calls <see cref="Finish"/> and copies all internal data into span.
        /// </summary>
        /// <param name="data">The output buffer.</param>
        /// <returns>Count of bytes written.</returns>
        public int ToSpan(ref Span<byte> data)
        {
            Add(1, 1);

            Finish();

            int numChunks = (bitsWritten >> 5) + 1;
            int length = data.Length;

            for (int i = 0; i < numChunks; i++)
            {
                int dataIdx = i * 4;
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

        public void FromSpan(ref ReadOnlySpan<byte> data, int length)
        {
            int numChunks = (length / 4) + 1;

            if (chunks.Length < numChunks)
            {
                chunks = new uint[numChunks];
                totalNumChunks = numChunks;// / 4;
                totalNumBits = numChunks * 32;
            }

            for (int i = 0; i < numChunks; i++)
            {
                int dataIdx = i * 4;
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

            int positionInByte = 8 - BitOperations.LeadingZeroCount(data[length - 1]);

            bitsWritten = ((length - 1) * 8) + (positionInByte - 1);
            bitsRead = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitBuffer AddBool(bool value)
        {
            Add(1, value ? 1U : 0U);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ReadBool() => Read(1) > 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool PeekBool() => Peek(1) > 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitBuffer AddByte(byte value)
        {
            Add(8, value);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitBuffer AddByte(byte value, int numBits)
        {
            AddUInt(value, numBits);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitBuffer AddByte(byte value, byte min, byte max)
        {
            AddUInt(value, min, max);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte ReadByte() => (byte)Read(8);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte ReadByte(int numBits) => (byte)ReadUInt(numBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte ReadByte(byte min, byte max) => (byte)ReadUInt(min, max);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte PeekByte() => (byte)Peek(8);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte PeekByte(int numBits) => (byte)PeekUInt(numBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte PeekByte(byte min, byte max) => (byte)PeekUInt(min, max);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitBuffer AddShort(short value)
        {
            AddInt(value);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitBuffer AddShort(short value, int numBits)
        {
            AddInt(value, numBits);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitBuffer AddShort(short value, short min, short max)
        {
            AddInt(value, min, max);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short ReadShort() => (short)ReadInt();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short ReadShort(int numBits) => (short)ReadInt(numBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short ReadShort(short min, short max) => (short)ReadInt(min, max);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short PeekShort() => (short)PeekInt();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short PeekShort(int numBits) => (short)PeekInt(numBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short PeekShort(short min, short max) => (short)PeekInt(min, max);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitBuffer AddUShort(ushort value)
        {
            AddUInt(value);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitBuffer AddUShort(ushort value, int numBits)
        {
            AddUInt(value, numBits);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitBuffer AddUShort(ushort value, ushort min, ushort max)
        {
            AddUInt(value, min, max);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort ReadUShort() => (ushort)ReadUInt();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort ReadUShort(int numBits) => (ushort)ReadUInt(numBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort ReadUShort(ushort min, ushort max) => (ushort)ReadUInt(min, max);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort PeekUShort() => (ushort)PeekUInt();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort PeekUShort(int numBits) => (ushort)PeekUInt(numBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort PeekUShort(ushort min, ushort max) => (ushort)PeekUInt(min, max);

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
        public BitBuffer AddInt(int value, int min, int max)
        {
            Debug.Assert(min < max, "minus is not lower than max");
            Debug.Assert(value >= min, "value is lower than minimal");
            Debug.Assert(value <= max, "value is higher than maximal");
            int bits = BitsRequired(min, max);
            Add(bits, (uint)(value - min));

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadInt(int min, int max)
        {
            Debug.Assert(min < max, "minus is not lower than max");

            int bits = BitsRequired(min, max);
            Debug.Assert(bits < totalNumBits, "reading too many bits for requested range");

            return (int)(Read(bits) + min);
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
        public int PeekInt(int min, int max)
        {
            Debug.Assert(min < max, "minus is not lower than max");

            int bits = BitsRequired(min, max);
            Debug.Assert(bits < totalNumBits, "reading too many bits for requested range");

            return (int)(Peek(bits) + min);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitBuffer AddUInt(uint value)
        {
            do
            {
                var buffer = value & 0x7Fu;
                value >>= 7;

                if (value > 0)
                    buffer |= 0x80u;

                Add(8, buffer);
            }
            while (value > 0);

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitBuffer AddUInt(uint value, int numBits)
        {
            Add(numBits, value);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitBuffer AddUInt(uint value, uint min, uint max)
        {
            Debug.Assert(min < max, "minus is not lower than max");
            Debug.Assert(value >= min, "value is lower than minimal");
            Debug.Assert(value <= max, "value is higher than maximal");
            int bits = BitsRequired(min, max);
            Add(bits, (value - min));

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

                value |= (buffer & 0x7Fu) << shift;
                shift += 7;
            }
            while ((buffer & 0x80u) > 0);

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadUInt(int numBits) => Read(numBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadUInt(uint min, uint max)
        {
            Debug.Assert(min < max, "minus is not lower than max");

            int bits = BitsRequired(min, max);
            Debug.Assert(bits < totalNumBits, "reading too many bits for requested range");

            return (Read(bits) + min);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint PeekUInt()
        {
            int tempPosition = bitsRead;
            uint value = ReadUInt();

            bitsRead = tempPosition;

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint PeekUInt(int numBits) => Peek(numBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint PeekUInt(uint min, uint max)
        {
            Debug.Assert(min < max, "minus is not lower than max");

            int bits = BitsRequired(min, max);
            Debug.Assert(bits < totalNumBits, "reading too many bits for requested range");

            return (Peek(bits) + min);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitBuffer AddLong(long value)
        {
            AddInt((int)(value & uint.MaxValue));
            AddInt((int)(value >> 32));

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long ReadLong()
        {
            int low = ReadInt();
            int high = ReadInt();
            long value = high;

            return value << 32 | (uint)low;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long PeekLong()
        {
            int tempPosition = bitsRead;
            long value = ReadLong();

            bitsRead = tempPosition;

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitBuffer AddULong(ulong value)
        {
            AddUInt((uint)(value & uint.MaxValue));
            AddUInt((uint)(value >> 32));
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong ReadULong()
        {
            uint low = ReadUInt();
            uint high = ReadUInt();
            return (ulong)high << 32 | low;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong PeekULong()
        {
            int tempPosition = bitsRead;
            ulong value = ReadULong();
            bitsRead = tempPosition;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitBuffer AddFloat(in float value)
        {
            uint union = Unsafe.As<float, uint>(ref Unsafe.AsRef<float>(in value));
            Add(32, union);

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitBuffer AddFloat(float value, float min, float max, float precision)
        {
            float range = max - min;
            float invPrecision = 1.0f / precision;
            float maxVal = range * invPrecision;
            int numBits = BitOperations.Log2((uint)(maxVal + 0.5f)) + 1;
            float adjusted = (value - min) * invPrecision;

            Add(numBits, (uint)(adjusted + 0.5f));

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitBuffer AddFloat(float value, float min, float max, int numBits)
        {
            var maxvalue = (1 << numBits) - 1;

            float range = max - min;
            var precision = range / maxvalue;
            var invPrecision = 1.0f / precision;

            float adjusted = (value - min) * invPrecision;

            Add(numBits, (uint)(adjusted + 0.5f));

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float ReadFloat()
        {
            var value = Read(32);
            return Unsafe.As<uint, float>(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float ReadFloat(float min, float max, float precision)
        {
            float range = max - min;
            float invPrecision = 1.0f / precision;
            float maxVal = range * invPrecision;
            int numBits = BitOperations.Log2((uint)(maxVal + 0.5f)) + 1;

            return Read(numBits) * precision + min;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float ReadFloat(float min, float max, int numBits)
        {
            var maxvalue = (1 << numBits) - 1;
            float range = max - min;
            var precision = range / maxvalue;

            return Read(numBits) * precision + min;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float PeekFloat(float min, float max, float precision)
        {
            float range = max - min;
            float invPrecision = 1.0f / precision;
            float maxVal = range * invPrecision;
            int numBits = BitOperations.Log2((uint)(maxVal + 0.5f)) + 1;

            return Peek(numBits) * precision + min;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float PeekFloat(float min, float max, int numBits)
        {
            var maxvalue = (1 << numBits) - 1;
            float range = max - min;
            var precision = range / maxvalue;

            return Peek(numBits) * precision + min;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float PeekFloat()
        {
            var value = Peek(32);
            return Unsafe.As<uint, float>(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitBuffer AddByteArray(byte[] value)
        {
            AddByteArray(value, 0, value.Length);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitBuffer AddByteArray(byte[] value, int length)
        {
            AddByteArray(value, 0, length);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitBuffer AddByteArray(byte[] value, int offset, int length)
        {
            Debug.Assert(value != null, "Supplied bytearray is null");
            Debug.Assert(length <= byteArrLengthMax, "Byte array too big, raise the byteArrLengthMax value or split the array.");

            if (length > byteArrLengthMax)
                length = byteArrLengthMax;

            Debug.Assert(length + 9 <= (totalNumBits - bitsWritten), "Byte array too big for buffer.");

            Add(byteArrLengthBits, (uint)length);

            for (int index = offset; index < length; index++)
            {
                AddByte(value[index]);
            }

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadByteArray(ref byte[] outValue) => ReadByteArray(ref outValue, out var length, 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadByteArray(ref byte[] outValue, out int length) => ReadByteArray(ref outValue, out length, 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadByteArray(ref byte[] outValue, out int length, int offset)
        {
            Debug.Assert(outValue != null, "Supplied bytearray is null");

            length = (int)Read(byteArrLengthBits);

            Debug.Assert(length <= outValue.Length + offset, "The supplied byte array is too small for requested read");

            for (int index = offset; index < length; index++)
            {
                outValue[index] = ReadByte();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int PeekByteArrayLength() => (int)Peek(byteArrLengthBits);

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