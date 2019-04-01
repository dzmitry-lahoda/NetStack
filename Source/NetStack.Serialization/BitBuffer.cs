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
    /// <summary>
    /// Bit level compression by ranged values.
    /// </summary>
    public partial class BitBuffer
    {
        public const int DefaultCapacityUInt = BitBuffer.MtuIeee802Dot3 / 4;

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
        /// Creates new instance with its own buffer. Create once and reuse to avoid GC.
        /// Call <see cref="FromArray"/> to reinitialize with copy of data.
        /// </summary>
        /// <param name="capacity">Count of 4 byte integers used as internal buffer.</param>
        /// <param name="stringLengthBits">Bits used to store length of strings.</param>
        /// <param name="byteArrLengthBits">Bits used to store length of byte arrays.</param>
        public BitBuffer(int capacity = DefaultCapacityUInt, int stringLengthBits = defaultStringLengthBits, int byteArrLengthBits = defaultByteArrLengthBits) 
        : this(new uint[capacity],  stringLengthBits  , byteArrLengthBits  )
        {
        }

        /// <summary>
        /// Creates new instance with its own buffer. 
        /// </summary>
        /// <param name="buffer">Custom buffer.</param>
        /// <param name="stringLengthBits">Bits used to store length of strings.</param>
        /// <param name="byteArrLengthBits">Bits used to store length of byte arrays.</param>
         public BitBuffer(uint[] buffer, int stringLengthBits = defaultStringLengthBits, int byteArrLengthBits = defaultByteArrLengthBits)
        {
            // not performance critical path so fine to check and throw
            if (buffer == null || buffer.Length == 0)
                throw new ArgumentException("Buffer should be non null or empty", nameof(buffer));

            if (stringLengthBits <= 0)
                throw new ArgumentException("Should be positive", nameof(stringLengthBits));

            if (byteArrLengthBits <= 0)
                throw new ArgumentException("Should be positive", nameof(byteArrLengthBits));

            // one time setup
            this.byteArrLengthBits = byteArrLengthBits;
            byteArrLengthMax = (1 << byteArrLengthBits) - 1;
            this.stringLengthBits = stringLengthBits;
            stringLengthMax = (1 << stringLengthBits) - 1;
            builder = new StringBuilder(stringLengthMax);

            chunks = buffer;
            totalNumChunks = buffer.Length;
            totalNumberBits = buffer.Length * Unsafe.SizeOf<uint>() * 8;

            Clear();
        }


        /// <summary>
        /// Count of written bytes.
        /// </summary>
        public int Length => ((bitsWritten - 1) >> 3) + 1;

        /// <summary>
        /// Counts of bits in buffer.
        /// </summary>
        public int BitsLength => bitsWritten; 

        public bool IsFinished => bitsWritten == bitsRead;

        public int BitsRead => bitsRead;

        /// <summary>
        /// Hom much bits can be yet written into buffer before it <see cref="IsFinished"/>.
        /// </summary>
        public int BitsAvailable => totalNumberBits - bitsWritten;

        public bool WouldOverflow(int bits) => bitsRead + bits > totalNumberBits;

        /// <summary>
        /// Sets buffer cursor to zero. Can start writing again.
        /// </summary>
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
            Debug.Assert(bitsRead <= totalNumberBits, "Pushing too many bits");
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ReadBool() => Read(1) > 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool PeekBool() => Peek(1) > 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddByte(byte value) => Add(8, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddByte(byte value, int numBits) => AddUInt(value, numBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddByte(byte value, byte min, byte max) => AddUInt(value, min, max);  

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
        public void AddSByte(sbyte value) => AddInt(value, 8);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddSByte(sbyte value, int numBits) => AddInt(value, numBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddSByte(sbyte value, sbyte min, sbyte max) => AddInt(value, min, max);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sbyte ReadSByte() => (sbyte)ReadInt(8);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sbyte ReadSByte(int numBits) => (sbyte)ReadInt(numBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sbyte ReadSByte(sbyte min, sbyte max) => (sbyte)ReadInt(min, max);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sbyte PeekSByte() => (sbyte)Peek(8);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sbyte PeekSByte(int numBits) => (sbyte)PeekInt(numBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sbyte PeekSByte(sbyte min, sbyte max) => (sbyte)PeekInt(min, max);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddShort(short value) => AddInt(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddShort(short value, int numBits) => AddInt(value, numBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddShort(short value, short min, short max) => AddInt(value, min, max);

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
        public void AddUShort(ushort value) => AddUInt(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddUShort(ushort value, int numBits) => AddUInt(value, numBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddUShort(ushort value, ushort min, ushort max) => AddUInt(value, min, max);

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
        public void AddInt(int value, int min, int max)
        {
            Debug.Assert(min < max, "minus is not lower than max");
            Debug.Assert(value >= min, "value is lower than minimal");
            Debug.Assert(value <= max, "value is higher than maximal");
            int bits = BitsRequired(min, max);
            Add(bits, (uint)(value - min));            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadInt(int min, int max)
        {
            Debug.Assert(min < max, "minus is not lower than max");

            int bits = BitsRequired(min, max);
            Debug.Assert(bits < totalNumberBits, "reading too many bits for requested range");

            return (int)(Read(bits) + min);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int PeekInt(int min, int max)
        {
            Debug.Assert(min < max, "minus is not lower than max");

            int bits = BitsRequired(min, max);
            Debug.Assert(bits < totalNumberBits, "reading too many bits for requested range");

            return (int)(Peek(bits) + min);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddUInt(uint value, int numBits) => Add(numBits, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddUInt(uint value, uint min, uint max)
        {
            Debug.Assert(min < max, "minus is not lower than max");
            Debug.Assert(value >= min, "value is lower than minimal");
            Debug.Assert(value <= max, "value is higher than maximal");
            int bits = BitsRequired(min, max);
            Add(bits, (value - min));            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadUInt(int numBits) => Read(numBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadUInt(uint min, uint max)
        {
            Debug.Assert(min < max, "minus is not lower than max");

            int bits = BitsRequired(min, max);
            Debug.Assert(bits < totalNumberBits, "reading too many bits for requested range");

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
            Debug.Assert(bits < totalNumberBits, "reading too many bits for requested range");

            return (Peek(bits) + min);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddLong(long value)
        {
            AddInt((int)(value & uint.MaxValue));
            AddInt((int)(value >> 32));            
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
        public void AddULong(ulong value)
        {
            AddUInt((uint)(value & uint.MaxValue));
            AddUInt((uint)(value >> 32));            
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
        public void AddFloat(in float value)
        {
            uint reinterpreted = Unsafe.As<float, uint>(ref Unsafe.AsRef<float>(in value));
            Add(32, reinterpreted);            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddFloat(float value, float min, float max, float precision)
        {
            float range = max - min;
            float invPrecision = 1.0f / precision;
            float maxVal = range * invPrecision;
            int numBits = BitOperations.Log2((uint)(maxVal + 0.5f)) + 1;
            float adjusted = (value - min) * invPrecision;
            Add(numBits, (uint)(adjusted + 0.5f));            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddFloat(float value, float min, float max, int numBits)
        {
            var maxvalue = (1 << numBits) - 1;

            float range = max - min;
            var precision = range / maxvalue;
            var invPrecision = 1.0f / precision;

            float adjusted = (value - min) * invPrecision;

            Add(numBits, (uint)(adjusted + 0.5f));            
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
        public void AddDouble(in double value)
        {
            ulong reinterpreted = Unsafe.As<double, ulong>(ref Unsafe.AsRef<double>(in value));
            AddULong(reinterpreted);            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double ReadDouble()
        {
            var value = ReadULong();
            return Unsafe.As<ulong, double>(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double PeekDouble()
        {
            var value = PeekULong();
            return Unsafe.As<ulong, double>(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddByteArray(byte[] value)
        {
            AddByteArray(value, 0, value.Length);            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddByteArray(byte[] value, int length) => AddByteArray(value, 0, length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddByteArray(byte[] value, int offset, int length)
        {
            Debug.Assert(value != null, "Supplied bytearray is null");
            Debug.Assert(length <= byteArrLengthMax, $"Byte array too big, raise the {nameof(byteArrLengthBits)} value or split the array.");

            if (length > byteArrLengthMax)
                length = byteArrLengthMax;

            Debug.Assert(length + 9 <= (totalNumberBits - bitsWritten), "Byte array too big for buffer.");

            Add(byteArrLengthBits, (uint)length);

            for (int index = offset; index < length; index++)
            {
                AddByte(value[index]);
            }            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadByteArray(ref byte[] outValue) => ReadByteArray(ref outValue, out var length, 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadByteArray(ref byte[] outValue, out int length) => ReadByteArray(ref outValue, out length, 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadByteArray(ref byte[] outValue, out int length, int offset)
        {
            // may throw here consider array to be non one or couple of elements, but larger - not hot path
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
    }
}