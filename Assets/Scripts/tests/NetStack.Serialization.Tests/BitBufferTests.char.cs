using System;
using System.Numerics;
using NUnit.Framework;

namespace NetStack.Serialization
{
    partial class BitBufferTests
    {
        [Test]
        public void CodePagesBitsRequited()
        {
            var bits = BitBuffer.BitsRequired("0".AsSpan(),1);
            var bitBuffer = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            bitBuffer.c("0".AsSpan());
            Assert.AreEqual(bits, bitBuffer.BitsWritten);
            var result = bitBuffer.ToArray();
        }

        [Test]
        public void ЁBitsRequited()
        {
            var bits = BitBuffer.BitsRequired("Ё".AsSpan(),1);
            var bitBuffer = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            bitBuffer.c("Ё");
            Assert.AreEqual(bits, bitBuffer.BitsWritten);
            var result = bitBuffer.ToArray();
        }

        [Test]
        public void StringNullBitsRequited()
        {
            BitBuffer.BitsRequired(null, 42);
        }

        [Test]
        public void AnsiStringWriteRead()
        {
            var writer = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            writer.c("123456789");
            writer.Align();
            var allocated = new byte[ushort.MaxValue];
            writer.ToSpan(allocated);
            var reader = new BitBufferReader<SevenBitDecoding>(allocated.Length);
            reader.CopyFrom(allocated);
            Assert.AreEqual("123456789", reader.String());
        }

        [Test]
        public void StringWriteRead()
        {
            var writer = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            writer.c("lahoda.prо/минск");
            var allocated = new byte[ushort.MaxValue];
            writer.ToSpan(allocated);
            var reader = new BitBufferReader<SevenBitDecoding>(allocated.Length);
            reader.CopyFrom(allocated);
            Assert.AreEqual("lahoda.prо/минск", reader.String());
        }
    }
}