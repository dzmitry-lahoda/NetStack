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
            var bitBuffer = new BitBufferWriter<SevenBit>();
            bitBuffer.String("0");
            Assert.Equal(bits, bitBuffer.BitsWritten);
            var result = bitBuffer.ToArray();
        }

        [Fact]
        public void ЁBitsRequited()
        {
            var bits = BitBuffer.BitsRequired("Ё",1);
            var bitBuffer = new BitBufferWriter<SevenBit>();
            bitBuffer.String("Ё");
            Assert.Equal(bits, bitBuffer.BitsWritten);
            var result = bitBuffer.ToArray();
        }

#if DEBUG || NETSTACK_VALIDATE
        [Fact]
        public void StringNullBitsRequited()
        {
           Assert.Throws<ArgumentNullException>(()=> BitBuffer.BitsRequired(null,42));
        }
#endif

        [Fact]
        public void AnsiStringWriteRead()
        {
            var writer = new BitBufferWriter<SevenBit>();
            writer.String("123456789");
            writer.Finish();
            var allocated = new byte[ushort.MaxValue];
            writer.ToArray(allocated);
            var reader = new BitBufferReader<SevenBitRe>(allocated.Length);
            reader.FromArray(allocated);
            Assert.Equal("123456789", reader.String());
        }

        [Fact]
        public void StringWriteRead()
        {
            var writer = new BitBufferWriter<SevenBit>();
            writer.String("lahoda.prо/минск");
            var allocated = new byte[ushort.MaxValue];
            writer.ToArray(allocated);
            var reader = new BitBufferReader<SevenBitRe>(allocated.Length);
            reader.FromArray(allocated);
            Assert.Equal("lahoda.prо/минск", reader.String());
        }
    }
}