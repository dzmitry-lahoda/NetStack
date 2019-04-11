using System;
using System.Numerics;
using Xunit;

namespace NetStack.Serialization
{
    partial class BitBufferTests
    {
        [Fact]
        public void IntReadWrite()
        {
            var buffer = new BitBufferWrite();
            buffer.i32(int.MinValue);
            buffer.i32(0);
            buffer.i32(int.MaxValue);
            buffer.Finish();
            var allocated = new byte[ushort.MaxValue];
            buffer.ToArray(allocated);
            var reader = new BitBufferReader(allocated.Length);
            reader.FromArray(allocated);
            Assert.Equal(int.MinValue, reader.i32());
            Assert.Equal(0, reader.i32());
            Assert.Equal(int.MaxValue, reader.i32());
        }

        [Fact]
        public void IntMinMaxRequired()
        {
            var buffer = new BitBufferWrite();
            buffer.i32(12345, 0, 123456);
            buffer.i32(1);
            buffer.i32(42, -1, 43);
            buffer.i32(1, 0, 10);
            buffer.i32(2, 3);
            buffer.i32(0);
            var bitsWritten = buffer.BitsWritten;
            var data = buffer.ToArray();
            var reader = new BitBufferReader();
            reader.FromArray(data);
            Assert.Equal(12345, reader.i32(0, 123456));
            Assert.Equal(1, reader.i32());
            Assert.Equal(42, reader.i32(-1, 43));
            Assert.Equal(1, reader.i32(0, 10));
            Assert.Equal(2, reader.i32(3));
            Assert.Equal(0, reader.i32());
            Assert.Equal(bitsWritten, reader.BitsRead);
        }
    }
}
