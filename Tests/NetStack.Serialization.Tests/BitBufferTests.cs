using System;
using System.Numerics;
using Xunit;

namespace NetStack.Serialization
{
    partial class BitBufferTests
    {
        [Fact]
        public void UShortReadWrite()
        {
            var buffer = new BitBuffer();
            buffer.AddUShort(ushort.MinValue);
            buffer.AddUShort(ushort.MaxValue / 2);
            buffer.AddUShort(ushort.MaxValue);
            var allocated = new byte[ushort.MaxValue];
            buffer.ToArray(allocated);
            var reader = new BitBuffer(allocated.Length);
            reader.FromArray(allocated);
            Assert.Equal(ushort.MinValue, reader.ReadUShort());
            ushort half = ushort.MaxValue / 2;
            Assert.Equal(half, reader.ReadUShort());
            Assert.Equal(ushort.MaxValue, reader.ReadUShort());
        }

        [Fact]
        public void SByteReadWrite()
        {
            var buffer = new BitBuffer();
            buffer.AddSByte(sbyte.MinValue);
            buffer.AddSByte(sbyte.MaxValue / 2);
            buffer.AddSByte(sbyte.MaxValue);
            buffer.Finish();
            var allocated = new byte[ushort.MaxValue];
            buffer.ToArray(allocated);
            var reader = new BitBuffer(allocated.Length);
            reader.FromArray(allocated);
            Assert.Equal(sbyte.MinValue, reader.ReadSByte());
            sbyte half = sbyte.MaxValue / 2;
            Assert.Equal(half, reader.ReadSByte());
            Assert.Equal(sbyte.MaxValue, reader.ReadSByte());
        }        

        [Fact]
        public void ShortReadWrite()
        {
            var buffer = new BitBuffer();
            buffer.AddShort(short.MinValue);
            buffer.AddShort(short.MinValue / 2);
            buffer.AddShort(0);
            buffer.AddShort(short.MaxValue / 2);
            buffer.AddShort(short.MaxValue);
            buffer.Finish();
            var allocated = new byte[ushort.MaxValue];
            buffer.ToArray(allocated);
            var reader = new BitBuffer(allocated.Length);
            reader.FromArray(allocated);
            Assert.Equal(short.MinValue, reader.ReadShort());
            Assert.Equal(short.MinValue / 2, reader.ReadShort());
            Assert.Equal(0, reader.ReadShort());
            short half = short.MaxValue / 2;
            Assert.Equal(half, reader.ReadShort());
            Assert.Equal(short.MaxValue, reader.ReadShort());
        }

        [Fact]
        public void FloatReadWrite()
        {
            var buffer = new BitBuffer();
            buffer.AddFloat(123.456f);
            buffer.Finish();
            var allocated = new byte[ushort.MaxValue];
            buffer.ToArray(allocated);
            var reader = new BitBuffer(allocated.Length);
            reader.FromArray(allocated);
            Assert.Equal(123.456f, reader.ReadFloat());
        }

        [Fact]
        public void LongReadWrite()
        {
            var buffer = new BitBuffer();
            buffer.AddLong(long.MaxValue);
            buffer.Finish();
            var allocated = new byte[ushort.MaxValue];
            buffer.ToArray(allocated);
            var reader = new BitBuffer(allocated.Length);
            reader.FromArray(allocated);
            Assert.Equal(long.MaxValue, reader.ReadLong());
        }

        [Fact]
        public void UintReadWrite()
        {
            var buffer = new BitBuffer();
            buffer.AddUInt(uint.MinValue);
            buffer.AddUInt(uint.MaxValue);
            buffer.Finish();
            var allocated = new byte[ushort.MaxValue];
            buffer.ToArray(allocated);
            var reader = new BitBuffer(allocated.Length);
            reader.FromArray(allocated);
            Assert.Equal(uint.MinValue, reader.ReadUInt());
            Assert.Equal(uint.MaxValue, reader.ReadUInt());
        }

        [Fact]
        public void DoubleReadWrite()
        {
            var buffer = new BitBuffer();
            buffer.AddDouble(double.MaxValue);
            buffer.Finish();
            var allocated = new byte[ushort.MaxValue];
            buffer.ToArray(allocated);
            var reader = new BitBuffer(allocated.Length);
            reader.FromArray(allocated);
            Assert.Equal(double.MaxValue, reader.ReadDouble());
        }

        [Fact]
        public void IntReadWrite()
        {
            var buffer = new BitBuffer();
            buffer.AddInt(int.MinValue);
            buffer.AddInt(0);
            buffer.AddInt(int.MaxValue);
            buffer.Finish();
            var allocated = new byte[ushort.MaxValue];
            buffer.ToArray(allocated);
            var reader = new BitBuffer(allocated.Length);
            reader.FromArray(allocated);
            Assert.Equal(int.MinValue, reader.ReadInt());
            Assert.Equal(0, reader.ReadInt());
            Assert.Equal(int.MaxValue, reader.ReadInt());
        }     

        [Fact]
        public void IntMinMaxRequired()
        {
            var buffer = new BitBuffer();
            buffer.AddInt(12345, 0, 123456);
            buffer.Finish();
            var allocated = new byte[ushort.MaxValue];
            buffer.ToArray(allocated);
            var reader = new BitBuffer(allocated.Length);
            reader.FromArray(allocated);
            Assert.Equal(12345, reader.ReadInt(0, 123456));
        }

        [Fact]
        public void FloatMinMaxRequired()
        {
            var buffer = new BitBuffer();
            buffer.AddFloat(1234.5f, 0, 12345.6f, 0.01f);
            buffer.Finish();
            var allocated = new byte[ushort.MaxValue];
            buffer.ToArray(allocated);
            var reader = new BitBuffer(allocated.Length);
            reader.FromArray(allocated);
            Assert.Equal(1234.5f, reader.ReadFloat(0, 12345.6f, 0.01f));
        }

        [Fact]
        public void ByteArrayWriteRead()
        {
            var buffer = new BitBuffer();
            var input = new byte[] { 1, 2, 3, 4, 5 };
            buffer.AddByteArray(input);
            buffer.Finish();
            var allocated = new byte[ushort.MaxValue];
            buffer.ToArray(allocated);
            var reader = new BitBuffer(allocated.Length);
            reader.FromArray(allocated);
            var output = new byte[5];
            reader.ReadByteArray(ref output, out var length);
            Assert.Equal(input, output);
        }

        [Fact]
        public void ByteArrayMaxWriteRead()
        {
            var buffer = new BitBuffer();
            var input = new byte[buffer.ByteArrLengthMax];
            buffer.AddByteArray(input);
            buffer.Finish();
            var allocated = new byte[ushort.MaxValue];
            buffer.ToArray(allocated);
            var reader = new BitBuffer(allocated.Length);
            reader.FromArray(allocated);
            Assert.Equal(buffer.ByteArrLengthMax, reader.PeekByteArrayLength());
        }

        [Fact]
        public void ToFromArrayPosition()
        {
            var buffer = new BitBuffer();
            var input = new byte[buffer.ByteArrLengthMax];
            buffer.AddByte(13);
            buffer.AddLong(long.MaxValue);
            buffer.Finish();
            var allocated = new byte[ushort.MaxValue];
            buffer.ToArray(allocated, 10, 100);
            var reader = new BitBuffer(allocated.Length);
            reader.FromArray(allocated, 10, 100);
            Assert.Equal(13, reader.ReadByte());
            Assert.Equal(long.MaxValue, reader.ReadLong());
        }
   }
}
