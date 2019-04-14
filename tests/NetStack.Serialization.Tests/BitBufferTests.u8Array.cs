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
            var writer = new BitBufferWriter<SevenBitEncoding>();
            var input = new byte[] { 1, 2, 3, 4, 5 };
            writer.u8(input);
            writer.Align();
            var allocated = new u8[ushort.MaxValue];
            writer.ToSpan(allocated);
            var reader = new BitBufferReader<SevenBitDecoding>(allocated.Length);
            reader.CopyFrom(allocated);
            var output = new byte[5];
            var lengthPeek = reader.u8SpanLengthPeek();
            var length = reader.u8(output);
            Assert.Equal(lengthPeek, length);
            Assert.Equal(input, output);
        }

        [Fact]
        public void u8ArrayMaxWriteRead()
        {
            var writer = new BitBufferWriter<SevenBitEncoding>();
            var input = new byte[writer.Options.U8SpanLengthMax];
            writer.u8(input);
            var allocated = new byte[ushort.MaxValue];
            writer.ToSpan(allocated);
            var reader = new BitBufferReader<SevenBitDecoding>(allocated.Length);
            reader.CopyFrom(allocated);
            Assert.Equal(writer.Options.U8SpanLengthMax, reader.u8SpanLengthPeek());
        }

        [Fact]
        public void u8ArrayWriteLimit()
        {
            var writer = new BitBufferWriter<SevenBitEncoding>();
            var input = new byte[writer.Options.U8SpanLengthMax + 1];
            Assert.Throws<ArgumentException>(()=> writer.u8(input));
        }

        [Fact]
        public void u8ArrayReadLimit()
        {
            var writer = new BitBufferWriter<SevenBitEncoding>();
            var input = new byte[writer.Options.U8SpanLengthMax];
            writer.u8(input);
            var data = writer.ToArray();
            var reader = new BitBufferReader<SevenBitDecoding>();
            reader.CopyFrom(data);
            
            Assert.Throws<ArgumentException>(()=> reader.u8(new byte[1]));
            Assert.Throws<ArgumentException>(()=> reader.u8(new byte[writer.Options.U8SpanLengthMax], writer.Options.U8SpanLengthMax + 1 ));
        }
    }
}
