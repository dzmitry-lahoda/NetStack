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
            var buffer = new BitBufferWrite();
            buffer.AddUShort(ushort.MinValue);
            buffer.AddUShort(ushort.MaxValue / 2);
            buffer.AddUShort(ushort.MaxValue);
            var allocated = new byte[ushort.MaxValue];
            buffer.ToArray(allocated);
            var reader = new BitBufferRead(allocated.Length);
            reader.FromArray(allocated);
            Assert.Equal(ushort.MinValue, reader.ReadUShort());
            ushort half = ushort.MaxValue / 2;
            Assert.Equal(half, reader.ReadUShort());
            Assert.Equal(ushort.MaxValue, reader.ReadUShort());
        }

        [Fact]
        public void SByteReadWrite()
        {
            var buffer = new BitBufferWrite();
            buffer.i8(sbyte.MinValue);
            buffer.i8(sbyte.MaxValue / 2);
            buffer.i8(sbyte.MaxValue);
            buffer.Finish();
            var allocated = new byte[ushort.MaxValue];
            buffer.ToArray(allocated);
            var reader = new BitBufferRead(allocated.Length);
            reader.FromArray(allocated);
            Assert.Equal(sbyte.MinValue, reader.ReadSByte());
            sbyte half = sbyte.MaxValue / 2;
            Assert.Equal(half, reader.ReadSByte());
            Assert.Equal(sbyte.MaxValue, reader.ReadSByte());
        }        

        [Fact]
        public void ShortReadWrite()
        {
            var buffer = new BitBufferWrite();
            buffer.i16(short.MinValue);
            buffer.i16(short.MinValue / 2);
            buffer.i16(0);
            buffer.i16(short.MaxValue / 2);
            buffer.i16(short.MaxValue);
            buffer.Finish();
            var allocated = new byte[ushort.MaxValue];
            buffer.ToArray(allocated);
            var reader = new BitBufferRead(allocated.Length);
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
            var buffer = new BitBufferWrite();
            buffer.AddFloat(123.456f);
            buffer.Finish();
            var allocated = new byte[ushort.MaxValue];
            buffer.ToArray(allocated);
            var reader = new BitBufferRead(allocated.Length);
            reader.FromArray(allocated);
            Assert.Equal(123.456f, reader.ReadFloat());
        }

        [Fact]
        public void LongReadWrite()
        {
            var buffer = new BitBufferWrite();
            buffer.AddLong(long.MaxValue);
            buffer.Finish();
            var allocated = new byte[ushort.MaxValue];
            buffer.ToArray(allocated);
            var reader = new BitBufferRead(allocated.Length);
            reader.FromArray(allocated);
            Assert.Equal(long.MaxValue, reader.ReadLong());
        }

        [Fact]
        public void UintReadWrite()
        {
            var buffer = new BitBufferWrite();
            buffer.u32(uint.MinValue);
            buffer.u32(uint.MaxValue);
            buffer.Finish();
            var allocated = new byte[ushort.MaxValue];
            buffer.ToArray(allocated);
            var reader = new BitBufferRead(allocated.Length);
            reader.FromArray(allocated);
            Assert.Equal(uint.MinValue, reader.u32());
            Assert.Equal(uint.MaxValue, reader.u32());
        }

        [Fact]
        public void DoubleReadWrite()
        {
            var buffer = new BitBufferWrite();
            buffer.AddDouble(double.MaxValue);
            buffer.Finish();
            var allocated = new byte[ushort.MaxValue];
            buffer.ToArray(allocated);
            var reader = new BitBufferRead(allocated.Length);
            reader.FromArray(allocated);
            Assert.Equal(double.MaxValue, reader.ReadDouble());
        }

     
        [Fact]
        public void FloatMinMaxRequired()
        {
            var buffer = new BitBufferWrite();
            buffer.AddFloat(1234.5f, 0, 12345.6f, 0.01f);
            buffer.Finish();
            var allocated = new byte[ushort.MaxValue];
            buffer.ToArray(allocated);
            var reader = new BitBufferRead(allocated.Length);
            reader.FromArray(allocated);
            Assert.Equal(1234.5f, reader.ReadFloat(0, 12345.6f, 0.01f));
        }

        [Fact]
        public void ByteArrayWriteRead()
        {
            var buffer = new BitBufferWrite();
            var input = new byte[] { 1, 2, 3, 4, 5 };
            buffer.AddByteArray(input);
            buffer.Finish();
            var allocated = new byte[ushort.MaxValue];
            buffer.ToArray(allocated);
            var reader = new BitBufferRead(allocated.Length);
            reader.FromArray(allocated);
            var output = new byte[5];
            reader.ReadByteArray(output, out var length);
            Assert.Equal(input, output);
        }

        [Fact]
        public void ByteArrayMaxWriteRead()
        {
            var buffer = new BitBufferWrite();
            var input = new byte[buffer.Options.ByteArrLengthMax];
            buffer.AddByteArray(input);
            buffer.Finish();
            var allocated = new byte[ushort.MaxValue];
            buffer.ToArray(allocated);
            var reader = new BitBufferRead(allocated.Length);
            reader.FromArray(allocated);
            Assert.Equal(buffer.Options.ByteArrLengthMax, reader.PeekByteArrayLength());
        }

        [Fact]
        public void ToFromArrayPosition()
        {
            var buffer = new BitBufferWrite();
            var input = new byte[buffer.Options.ByteArrLengthMax];
            buffer.u8(13);
            buffer.AddLong(long.MaxValue);
            buffer.Finish();
            var allocated = new byte[ushort.MaxValue];
            buffer.ToArray(allocated, 10, 100);
            var reader = new BitBufferRead(allocated.Length);
            reader.FromArray(allocated, 10, 100);
            Assert.Equal(13, reader.u8());
            Assert.Equal(long.MaxValue, reader.ReadLong());
        }
   }
}
