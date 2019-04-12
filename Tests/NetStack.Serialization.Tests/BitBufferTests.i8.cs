using System;
using System.Numerics;
using Xunit;
using i8 = System.SByte;
using i16 = System.Int16;
using i32 = System.Int32;
using i64 = System.Int64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;
using f32 = System.Single;
using f64 = System.Double;


namespace NetStack.Serialization
{
    partial class BitBufferTests
    {
        [Fact]
        public void i8ReadWrite()
        {
            var writer = new BitBufferWriter<SevenBit>();
            writer.i8(i8.MinValue);
            writer.i8(i8.MaxValue / 2);
            writer.i8(i8.MaxValue);
            var data = writer.ToArray();
            var reader = new BitBufferReader();
            reader.FromArray(data);
            Assert.Equal(i8.MinValue, reader.i8());
            i8 half = i8.MaxValue / 2;
            Assert.Equal(half, reader.i8());
            Assert.Equal(i8.MaxValue, reader.i8());
        }

        [Fact]
        public void i8ReadWriteLimits()
        {
            var writer = new BitBufferWriter<SevenBit>();
            writer.i8(13, 0, 14);
            writer.i8(2, 4);
            var bitsWritten = writer.BitsWritten;
            var data = writer.ToArray();
            var reader = new BitBufferReader();
            reader.FromArray(data);
            Assert.Equal(13, reader.i8(0, 14));
            Assert.Equal(2, reader.i8(4));
            Assert.Equal(bitsWritten, reader.BitsRead);
        }        
   }
}
