using System;
using System.Numerics;
using Xunit;

namespace NetStack.Serialization
{
    public partial class BitBufferTests
    {
        [Fact]
        public void Capacity()
        {
            var writer = new BitBufferWriter<SevenBit>(100);
            Assert.Equal(0, writer.LengthWritten);
            var received = new byte[2000];
            var reader = new BitBufferReader();
            reader.FromArray(received);
            Assert.Equal(0, writer.LengthWritten);
        }

        [Fact]
        public void BitsRead()
        {
            var writer = new BitBufferWriter<SevenBit>();

            Assert.Equal(0, writer.BitsWritten);
            writer.b(true);
            Assert.Equal(1, writer.BitsWritten);
            writer.u8(123);
            Assert.Equal(9, writer.BitsWritten);
            writer.i16(12345);
            Assert.Equal(33, writer.BitsWritten);
            writer.i32(1234567890);
            Assert.Equal(73, writer.BitsWritten);
            var data = writer.ToArray();
            var reader = new BitBufferReader();
            reader.FromArray(data);

            Assert.Equal(0, reader.BitsRead);
            reader.b();
            Assert.Equal(1, reader.BitsRead);
            reader.u8();
            Assert.Equal(9, reader.BitsRead);
            reader.i16();
            Assert.Equal(33, reader.BitsRead);
            reader.i32();
            Assert.Equal(73, reader.BitsRead);
        }
    }
}
