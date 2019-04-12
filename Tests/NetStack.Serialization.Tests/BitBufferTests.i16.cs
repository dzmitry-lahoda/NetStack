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
        public void i16ReadWrite()
        {
            var writer = new BitBufferWriter();
            writer.i16(i16.MinValue);
            writer.i16(i16.MinValue / 2);
            writer.i16(0);
            writer.i16(i16.MaxValue / 2);
            writer.i16(i16.MaxValue);
            writer.Finish();
            var allocated = new byte[i16.MaxValue];
            writer.ToArray(allocated);
            var reader = new BitBufferReader(allocated.Length);
            reader.FromArray(allocated);
            Assert.Equal(i16.MinValue, reader.i16());
            Assert.Equal(i16.MinValue / 2, reader.i16());
            Assert.Equal(0, reader.i16());
            i16 half = i16.MaxValue / 2;
            Assert.Equal(half, reader.i16());
            Assert.Equal(i16.MaxValue, reader.i16());
        }

        [Fact]
        public void i16ReadWriteLimits()
        {
            var writer = new BitBufferWriter();
            writer.i16(-1, -2, 2);
            writer.i16(-1, 4);
            var allocated = new byte[i16.MaxValue];
            writer.ToArray(allocated);
            var reader = new BitBufferReader(allocated.Length);
            reader.FromArray(allocated);
            Assert.Equal(-1, reader.i16(-2, 2));
            Assert.Equal(-1, reader.i16(4));
        }
    }
}
