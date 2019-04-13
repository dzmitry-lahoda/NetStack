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
        public void u8ArrayWriteRead()
        {
            var writer = new BitBufferWriter<SevenBit>();
            var input = new byte[] { 1, 2, 3, 4, 5 };
            writer.u8(input);
            writer.Finish();
            var allocated = new u8[ushort.MaxValue];
            writer.ToArray(allocated);
            var reader = new BitBufferReader<SevenBitRe>(allocated.Length);
            reader.FromArray(allocated);
            var output = new byte[5];
            var length = reader.u8(output);
            Assert.Equal(input, output);
        }

        [Fact]
        public void u8ArrayMaxWriteRead()
        {
            var writer = new BitBufferWriter<SevenBit>();
            var input = new byte[writer.Options.ByteArrLengthMax];
            writer.u8(input);
            writer.Finish();
            var allocated = new byte[ushort.MaxValue];
            writer.ToArray(allocated);
            var reader = new BitBufferReader<SevenBitRe>(allocated.Length);
            reader.FromArray(allocated);
            Assert.Equal(writer.Options.ByteArrLengthMax, reader.PeekByteArrayLength());
        }

        [Fact]
        public void ToFromArrayPosition()
        {
            var writer = new BitBufferWriter<SevenBit>();
            var input = new byte[writer.Options.ByteArrLengthMax];
            writer.u8(13);
            writer.i64(i64.MaxValue);
            writer.Finish();
            var allocated = new byte[ushort.MaxValue];
            writer.ToArray(allocated, 10, 100);
            var reader = new BitBufferReader<SevenBitRe>(allocated.Length);
            reader.FromArray(allocated, 10, 100);
            Assert.Equal(13, reader.u8());
            Assert.Equal(long.MaxValue, reader.i64());
        }
    }
}
