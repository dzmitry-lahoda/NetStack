using System;
using System.Numerics;
using Xunit;

namespace NetStack.Serialization
{
    partial class BitBufferTests
    {
        [Fact]
        public void CodePagesBitsRequited()
        {
            var bits = BitBuffer.BitsRequired("0",1);
            var bitBuffer = new BitBufferWriter<SevenBitEncoding>();
            bitBuffer.chars("0");
            Assert.Equal(bits, bitBuffer.BitsWritten);
            var result = bitBuffer.ToArray();
        }

        [Fact]
        public void ЁBitsRequited()
        {
            var bits = BitBuffer.BitsRequired("Ё",1);
            var bitBuffer = new BitBufferWriter<SevenBitEncoding>();
            bitBuffer.chars("Ё");
            Assert.Equal(bits, bitBuffer.BitsWritten);
            var result = bitBuffer.ToArray();
        }

        [Fact]
        public void StringNullBitsRequited()
        {
            BitBuffer.BitsRequired(null,42);
        }

        [Fact]
        public void AnsiStringWriteRead()
        {
            var writer = new BitBufferWriter<SevenBitEncoding>();
            writer.chars("123456789");
            writer.Finish();
            var allocated = new byte[ushort.MaxValue];
            writer.ToArray(allocated);
            var reader = new BitBufferReader<SevenBitDecoding>(allocated.Length);
            reader.FromArray(allocated);
            Assert.Equal("123456789", reader.String());
        }

        [Fact]
        public void StringWriteRead()
        {
            var writer = new BitBufferWriter<SevenBitEncoding>();
            writer.chars("lahoda.prо/минск");
            var allocated = new byte[ushort.MaxValue];
            writer.ToArray(allocated);
            var reader = new BitBufferReader<SevenBitDecoding>(allocated.Length);
            reader.FromArray(allocated);
            Assert.Equal("lahoda.prо/минск", reader.String());
        }
    }
}