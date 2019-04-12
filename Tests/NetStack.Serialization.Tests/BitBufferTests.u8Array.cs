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
        public void ByteArrayWriteRead()
        {
            var buffer = new BitBufferWriter<SevenBit>();
            var input = new byte[] { 1, 2, 3, 4, 5 };
            buffer.AddByteArray(input);
            buffer.Finish();
            var allocated = new byte[ushort.MaxValue];
            buffer.ToArray(allocated);
            var reader = new BitBufferReader(allocated.Length);
            reader.FromArray(allocated);
            var output = new byte[5];
            reader.ReadByteArray(output, out var length);
            Assert.Equal(input, output);
        }

        [Fact]
        public void ByteArrayMaxWriteRead()
        {
            var buffer = new BitBufferWriter<SevenBit>();
            var input = new byte[buffer.Options.ByteArrLengthMax];
            buffer.AddByteArray(input);
            buffer.Finish();
            var allocated = new byte[ushort.MaxValue];
            buffer.ToArray(allocated);
            var reader = new BitBufferReader(allocated.Length);
            reader.FromArray(allocated);
            Assert.Equal(buffer.Options.ByteArrLengthMax, reader.PeekByteArrayLength());
        }

        [Fact]
        public void ToFromArrayPosition()
        {
            var buffer = new BitBufferWriter<SevenBit>();
            var input = new byte[buffer.Options.ByteArrLengthMax];
            buffer.u8(13);
            buffer.i64(i64.MaxValue);
            buffer.Finish();
            var allocated = new byte[ushort.MaxValue];
            buffer.ToArray(allocated, 10, 100);
            var reader = new BitBufferReader(allocated.Length);
            reader.FromArray(allocated, 10, 100);
            Assert.Equal(13, reader.u8());
            Assert.Equal(long.MaxValue, reader.i64());
        }
    }
}
