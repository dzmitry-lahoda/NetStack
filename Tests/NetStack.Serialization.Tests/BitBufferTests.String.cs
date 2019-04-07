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
            var bitBuffer = new BitBuffer();
            bitBuffer.AddString("0");
            Assert.Equal(bits, bitBuffer.BitsLength);
            var result = bitBuffer.ToArray();
        }

        [Fact]
        public void ЁBitsRequited()
        {
            var bits = BitBuffer.BitsRequired("Ё",1);
            var bitBuffer = new BitBuffer();
            bitBuffer.AddString("Ё");
            Assert.Equal(bits, bitBuffer.BitsLength);
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
            var buffer = new BitBuffer();
            buffer.AddString("123456789");
            buffer.Finish();
            var allocated = new byte[ushort.MaxValue];
            buffer.ToArray(allocated);
            var reader = new BitBuffer(allocated.Length);
            reader.FromArray(allocated);
            Assert.Equal("123456789", reader.ReadString());
        }

        [Fact]
        public void StringWriteRead()
        {
            var buffer = new BitBuffer();
            buffer.AddString("lahoda.prо/минск");
            buffer.Finish();
            var allocated = new byte[ushort.MaxValue];
            buffer.ToArray(allocated);
            var reader = new BitBuffer(allocated.Length);
            reader.FromArray(allocated);
            Assert.Equal("lahoda.prо/минск", reader.ReadString());
        }
    }
}