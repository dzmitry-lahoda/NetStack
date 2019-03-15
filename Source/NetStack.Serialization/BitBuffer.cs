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

namespace NetStack.Serialization
{
    /// <summary>
    /// Bit level compression by ranged values. Quaternion compression, string UTF-16 support.
    /// You have to finalize the stream. Fill the buffer with Add commands and if you are done you have to call the Finish.
    /// ToArray or ToSpan it will automatically invoke the Finish.
    /// </summary>
    public class BitBuffer
    {
        private const int defaultCapacity = 375; // 375 * 4 = 1500 bytes default MTU. don't have to grow.
        private const int stringLengthMax = 255;
        private const int stringLengthBits = 8;
        private const int bitsASCII = 7;
        private const int bitsLATIN1 = 8;
        private const int bitsLATINEXT = 9;
        private const int bitsUTF16 = 16;
        private int bitsRead;
        private int bitsWriten;
        private uint[] chunks;
        private ulong scratch;
        private int totalNumBits;
        private int totalNumChunks;
        private int chunkIndex;
        private int scratchUsedBits;


        public BitBuffer(int capacity = defaultCapacity)
        {
            bitsRead = 0;
            bitsWriten = 0;
            chunks = new uint[capacity];
            totalNumChunks = capacity;// / 4;
            totalNumBits = capacity * 32;
            chunkIndex = 0;
            scratch = 0;
            scratchUsedBits = 0;
        }

        public int Length
        {
            get
            {
                return (bitsWriten >> 3) + 1;
            }
        }

        public int LengthInBits
        {
            get
            {
                return bitsWriten;
            }
        }

        public bool IsFinished
        {
            get
            {
                return bitsWriten == bitsRead;
            }
        }

        public int BitsAvailable
        {
            get
            {
                return totalNumBits - bitsWriten;
            }
        }

        public bool WouldOverflow(int bits)
        {
            return bitsRead + bits > totalNumBits;
        }

        [MethodImpl(256)]
        public void Clear()
        {
            bitsRead = 0;
            bitsWriten = 0;

            chunkIndex = 0;
            scratch = 0;
            scratchUsedBits = 0;
        }

        [MethodImpl(256)]
        public void ResetReadPos()
        {
            Finish();
            bitsRead = 0;
        }

        [MethodImpl(256)]
        public void Add(int numBits, uint value)
        {
            Debug.Assert(numBits > 0, "Pushing negative bits");
            Debug.Assert(numBits <= 32, "Pushing too many bits");
            Debug.Assert(bitsWriten + numBits <= totalNumBits, "Pushing failed, buffer is full.");
            Debug.Assert(value <= (uint)((1ul << numBits) - 1), "value too big, won't fit in requested number of bits."); // 

            value &= (uint)((1ul << numBits) - 1);

            scratch |= ((ulong)value) << scratchUsedBits;

            scratchUsedBits += numBits;

            if (scratchUsedBits >= 32)
            {
                Debug.Assert(chunkIndex < totalNumChunks, "Pushing failed, buffer is full.");
                chunks[chunkIndex] = (uint)(scratch);
                scratch >>= 32;
                scratchUsedBits -= 32;
                chunkIndex++;
            }

            bitsWriten += numBits;
        }

        [MethodImpl(256)]
        public uint Read(int numBits)
        {
            uint result = Peek(numBits);

            bitsRead += numBits;

            return result;
        }

        [MethodImpl(256)]
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

        [MethodImpl(256)]
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

        public int ToArray(byte[] data)
        {
            Add(1, 1);

            Finish();

            int numChunks = (bitsWriten >> 5) + 1;
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

            int positionInByte = Utils.FindHighestBitPosition(data[length - 1]);

            bitsWriten = ((length - 1) * 8) + (positionInByte - 1);
            bitsRead = 0;
        }

        public int ToSpan(ref Span<byte> data) {
            Add(1, 1);

            Finish();

            int numChunks = (bitsWriten >> 5) + 1;
            int length = data.Length;

            for (int i = 0; i < numChunks; i++) {
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

        public void FromSpan(ref ReadOnlySpan<byte> data, int length) {
            int numChunks = (length / 4) + 1;

            if (chunks.Length < numChunks) {
                chunks = new uint[numChunks];
                totalNumChunks = numChunks;// / 4;
                totalNumBits = numChunks * 32;
            }

            for (int i = 0; i < numChunks; i++) {
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

            int positionInByte = Utils.FindHighestBitPosition(data[length - 1]);

            bitsWriten = ((length - 1) * 8) + (positionInByte - 1);
            bitsRead = 0;
        }

        [MethodImpl(256)]
        public BitBuffer AddBool(bool value)
        {
            Add(1, value ? 1U : 0U);

            return this;
        }

        [MethodImpl(256)]
        public bool ReadBool()
        {
            return Read(1) > 0;
        }

        [MethodImpl(256)]
        public bool PeekBool()
        {
            return Peek(1) > 0;
        }

        [MethodImpl(256)]
        public BitBuffer AddByte(byte value)
        {
            Add(8, value);

            return this;
        }

        [MethodImpl(256)]
        public BitBuffer AddByte(byte value, int numBits)
        {
            AddUInt(value, numBits);

            return this;
        }

        [MethodImpl(256)]
        public BitBuffer AddByte(byte value, byte min, byte max)
        {
            AddUInt(value, min, max);

            return this;
        }

        [MethodImpl(256)]
        public byte ReadByte()
        {
            return (byte)Read(8);
        }

        [MethodImpl(256)]
        public byte ReadByte(int numBits)
        {
            return (byte)ReadUInt(numBits);
        }

        [MethodImpl(256)]
        public byte ReadByte(byte min, byte max)
        {
            return (byte)ReadUInt(min, max);
        }

        [MethodImpl(256)]
        public byte PeekByte()
        {
            return (byte)Peek(8);
        }

        [MethodImpl(256)]
        public byte PeekByte(int numBits)
        {
            return (byte)PeekUInt(numBits);
        }

        [MethodImpl(256)]
        public byte PeekByte(byte min, byte max)
        {
            return (byte)PeekUInt(min, max);
        }

        [MethodImpl(256)]
        public BitBuffer AddShort(short value)
        {
            AddInt(value);

            return this;
        }

        [MethodImpl(256)]
        public BitBuffer AddShort(short value, int numBits)
        {
            AddInt(value, numBits);

            return this;
        }

        [MethodImpl(256)]
        public BitBuffer AddShort(short value, short min, short max)
        {
            AddInt(value, min, max);

            return this;
        }

        [MethodImpl(256)]
        public short ReadShort()
        {
            return (short)ReadInt();
        }

        [MethodImpl(256)]
        public short ReadShort(int numBits)
        {
            return (short)ReadInt(numBits);
        }

        [MethodImpl(256)]
        public short ReadShort(short min, short max)
        {
            return (short)ReadInt(min, max);
        }

        [MethodImpl(256)]
        public short PeekShort()
        {
            return (short)PeekInt();
        }

        [MethodImpl(256)]
        public short PeekShort(int numBits)
        {
            return (short)PeekInt(numBits);
        }

        [MethodImpl(256)]
        public short PeekShort(short min, short max)
        {
            return (short)PeekInt(min, max);
        }

        [MethodImpl(256)]
        public BitBuffer AddUShort(ushort value)
        {
            AddUInt(value);

            return this;
        }

        [MethodImpl(256)]
        public BitBuffer AddUShort(ushort value, int numBits)
        {
            AddUInt(value, numBits);

            return this;
        }

        [MethodImpl(256)]
        public BitBuffer AddUShort(ushort value, ushort min, ushort max)
        {
            AddUInt(value, min, max);

            return this;
        }

        [MethodImpl(256)]
        public ushort ReadUShort()
        {
            return (ushort)ReadUInt();
        }

        [MethodImpl(256)]
        public ushort ReadUShort(int numBits)
        {
            return (ushort)ReadUInt(numBits);
        }

        [MethodImpl(256)]
        public ushort ReadUShort(ushort min, ushort max)
        {
            return (ushort)ReadUInt(min, max);
        }

        [MethodImpl(256)]
        public ushort PeekUShort()
        {
            return (ushort)PeekUInt();
        }

        [MethodImpl(256)]
        public ushort PeekUShort(int numBits)
        {
            return (ushort)PeekUInt(numBits);
        }

        [MethodImpl(256)]
        public ushort PeekUShort(ushort min, ushort max)
        {
            return (ushort)PeekUInt(min, max);
        }

        [MethodImpl(256)]
        public BitBuffer AddInt(int value)
        {
            uint zigzag = (uint)((value << 1) ^ (value >> 31));

            AddUInt(zigzag);

            return this;
        }

        [MethodImpl(256)]
        public BitBuffer AddInt(int value, int numBits)
        {
            uint zigzag = (uint)((value << 1) ^ (value >> 31));

            Add(numBits, zigzag);

            return this;
        }

        [MethodImpl(256)]
        public BitBuffer AddInt(int value, int min, int max)
        {
            Debug.Assert(min < max, "minus is not lower than max");
            Debug.Assert(value >= min, "value is lower than minimal");
            Debug.Assert(value <= max, "value is higher than maximal");
            int bits = Utils.BitsRequired(min, max);
            Add(bits, (uint)(value - min));

            return this;
        }

        [MethodImpl(256)]
        public int ReadInt()
        {
            uint value = ReadUInt();
            int zagzig = (int)((value >> 1) ^ (-(int)(value & 1)));

            return zagzig;
        }

        [MethodImpl(256)]
        public int ReadInt(int numBits)
        {
            uint value = Read(numBits);
            int zagzig = (int)((value >> 1) ^ (-(int)(value & 1)));

            return zagzig;
        }

        [MethodImpl(256)]
        public int ReadInt(int min, int max)
        {
            Debug.Assert(min < max, "minus is not lower than max");

            int bits = Utils.BitsRequired(min, max);
            Debug.Assert(bits < totalNumBits, "reading too many bits for requested range");

            return (int)(Read(bits) + min);
        }

        [MethodImpl(256)]
        public int PeekInt()
        {
            uint value = PeekUInt();
            int zagzig = (int)((value >> 1) ^ (-(int)(value & 1)));

            return zagzig;
        }

        [MethodImpl(256)]
        public int PeekInt(int numBits)
        {
            uint value = Peek(numBits);
            int zagzig = (int)((value >> 1) ^ (-(int)(value & 1)));

            return zagzig;
        }

        [MethodImpl(256)]
        public int PeekInt(int min, int max)
        {
            Debug.Assert(min < max, "minus is not lower than max");

            int bits = Utils.BitsRequired(min, max);
            Debug.Assert(bits < totalNumBits, "reading too many bits for requested range");

            return (int)(Peek(bits) + min);
        }

        [MethodImpl(256)]
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

        [MethodImpl(256)]
        public BitBuffer AddUInt(uint value, int numBits)
        {
            Add(numBits, value);

            return this;
        }

        [MethodImpl(256)]
        public BitBuffer AddUInt(uint value, uint min, uint max)
        {
            Debug.Assert(min < max, "minus is not lower than max");
            Debug.Assert(value >= min, "value is lower than minimal");
            Debug.Assert(value <= max, "value is higher than maximal");
            int bits = Utils.BitsRequired(min, max);
            Add(bits, (value - min));

            return this;
        }

        [MethodImpl(256)]
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

        [MethodImpl(256)]
        public uint ReadUInt(int numBits)
        {
            return Read(numBits);
        }

        [MethodImpl(256)]
        public uint ReadUInt(uint min, uint max)
        {
            Debug.Assert(min < max, "minus is not lower than max");

            int bits = Utils.BitsRequired(min, max);
            Debug.Assert(bits < totalNumBits, "reading too many bits for requested range");

            return (Read(bits) + min);
        }

        [MethodImpl(256)]
        public uint PeekUInt()
        {
            int tempPosition = bitsRead;
            uint value = ReadUInt();

            bitsRead = tempPosition;

            return value;
        }

        [MethodImpl(256)]
        public uint PeekUInt(int numBits)
        {
            return Peek(numBits);
        }

        [MethodImpl(256)]
        public uint PeekUInt(uint min, uint max)
        {
            Debug.Assert(min < max, "minus is not lower than max");

            int bits = Utils.BitsRequired(min, max);
            Debug.Assert(bits < totalNumBits, "reading too many bits for requested range");

            return (Peek(bits) + min);
        }

        [MethodImpl(256)]
        public BitBuffer AddLong(long value)
        {
            AddInt((int)(value & uint.MaxValue));
            AddInt((int)(value >> 32));

            return this;
        }

        [MethodImpl(256)]
        public long ReadLong()
        {
            int low = ReadInt();
            int high = ReadInt();
            long value = high;

            return value << 32 | (uint)low;
        }

        [MethodImpl(256)]
        public long PeekLong()
        {
            int tempPosition = bitsRead;
            long value = ReadLong();

            bitsRead = tempPosition;

            return value;
        }

        [MethodImpl(256)]
        public BitBuffer AddULong(ulong value)
        {
            AddUInt((uint)(value & uint.MaxValue));
            AddUInt((uint)(value >> 32));

            return this;
        }

        [MethodImpl(256)]
        public ulong ReadULong()
        {
            uint low = ReadUInt();
            uint high = ReadUInt();

            return (ulong)high << 32 | low;
        }

        [MethodImpl(256)]
        public ulong PeekULong()
        {
            int tempPosition = bitsRead;
            ulong value = ReadULong();

            bitsRead = tempPosition;

            return value;
        }

        [MethodImpl(256)]
        public BitBuffer AddFloat(float value)
        {
            var union = new Utils.FloatUintUnion { single = value };
            Add(32, union.uint32);

            return this;
        }

        [MethodImpl(256)]
        public BitBuffer AddFloat(float value, float min, float max, float precision)
        {
            float range = max - min;
            float invPrecision = 1.0f / precision;
            float maxVal = range * invPrecision;
            int numBits = Utils.Log2((uint)(maxVal + 0.5f)) + 1;
            float adjusted = (value - min) * invPrecision;

            Add(numBits, (uint)(adjusted + 0.5f));

            return this;
        }

        [MethodImpl(256)]
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

        [MethodImpl(256)]
        public float ReadFloat()
        {
            var union = new Utils.FloatUintUnion { uint32 = Read(32) };
            return union.single;
        }

        [MethodImpl(256)]
        public float ReadFloat(float min, float max, float precision)
        {
            float range = max - min;
            float invPrecision = 1.0f / precision;
            float maxVal = range * invPrecision;
            int numBits = Utils.Log2((uint)(maxVal + 0.5f)) + 1;

            return Read(numBits) * precision + min;
        }

        [MethodImpl(256)]
        public float ReadFloat(float min, float max, int numBits)
        {
            var maxvalue = (1 << numBits) - 1;
            float range = max - min;
            var precision = range / maxvalue;

            return Read(numBits) * precision + min;
        }

        [MethodImpl(256)]
        public float PeekFloat(float min, float max, float precision)
        {
            float range = max - min;
            float invPrecision = 1.0f / precision;
            float maxVal = range * invPrecision;
            int numBits = Utils.Log2((uint)(maxVal + 0.5f)) + 1;

            return Peek(numBits) * precision + min;
        }

        [MethodImpl(256)]
        public float PeekFloat(float min, float max, int numBits)
        {
            var maxvalue = (1 << numBits) - 1;
            float range = max - min;
            var precision = range / maxvalue;

            return Peek(numBits) * precision + min;
        }

        [MethodImpl(256)]
        public float PeekFloat()
        {
            var union = new Utils.FloatUintUnion { uint32 = Peek(32) };
            return union.single;
        }

        [MethodImpl(256)]
        public BitBuffer AddString(string value)
        {
            Debug.Assert(value != null, "String is null");

            int length = value.Length;
            if (length > stringLengthMax)
                length = stringLengthMax;

            if (length * 17 + 10 > (totalNumBits - bitsWriten)) // possible overflow
            {
                if (Utils.GetStringBitSize(value, length) > (totalNumBits - bitsWriten))
                    throw new ArgumentOutOfRangeException("String would not fit in bitstream.");
            }

            uint codePage = 0; // Ascii
            for (int i = 0; i < length; i++)
            {
                var val = value[i];
                if (val > 127)
                {
                    codePage = 1; // Latin1
                    if (val > 255)
                    {
                        codePage = 2; // LatinExtended 
                        if (val > 511)
                        {
                            codePage = 3; // UTF-16
                            break;
                        }
                    }
                }
            }

            Add(2, codePage);
            Add(stringLengthBits, (uint)length);

            if (codePage == 0)
                for (int i = 0; i < length; i++)
                {
                    Add(bitsASCII, value[i]);
                }
            else if (codePage == 1)
                for (int i = 0; i < length; i++)
                {
                    Add(bitsLATIN1, value[i]);
                }
            else if (codePage == 2)
                for (int i = 0; i < length; i++)
                {
                    Add(bitsLATINEXT, value[i]);
                }
            else if (codePage == 3)
                for (int i = 0; i < length; i++)
                {
                    if (value[i] > 127)
                    {
                        Add(1, 1);
                        Add(bitsUTF16, value[i]);
                    }
                    else
                    {
                        Add(1, 0);
                        Add(bitsASCII, value[i]);
                    }
                }

            return this;
        }

        private StringBuilder builder = new StringBuilder(stringLengthMax);

        [MethodImpl(256)]
        public string ReadString()
        {
            builder.Length = 0;

            uint codePage = Read(2);
            uint length = Read(stringLengthBits);

            if (codePage == 0)
                for (int i = 0; i < length; i++)
                {
                    builder.Append((char)Read(bitsASCII));
                }
            else if (codePage == 1)
                for (int i = 0; i < length; i++)
                {
                    builder.Append((char)Read(bitsLATIN1));
                }
            else if (codePage == 2)
                for (int i = 0; i < length; i++)
                {
                    builder.Append((char)Read(bitsLATINEXT));
                }
            else if (codePage == 3)
                for (int i = 0; i < length; i++)
                {
                    var needs16 = Read(1);
                    if (needs16 == 1)
                        builder.Append((char)Read(bitsUTF16));
                    else
                        builder.Append((char)Read(bitsASCII));
                }

            return builder.ToString();
        }

        public override string ToString()
        {
            builder.Length = 0;

            for (int i = chunks.Length - 1; i >= 0; i--)
            {
                builder.Append(Convert.ToString(chunks[i], 2).PadLeft(32, '0'));
            }

            StringBuilder spaced = new StringBuilder();

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