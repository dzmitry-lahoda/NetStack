using System;
using System.Runtime.CompilerServices;
using NUnit.Framework;

namespace NetStack.Serialization
{
    public class BitBufferExtensionsTests
    {
        struct SimpleStruct
        {
            public float a;
            public long b;

            public byte c;

            public int d;

            public bool e;
        }

        [Test]
        public void SimpleStructWrite()
        {
            var writer = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            var expected = new SimpleStruct { a = 1.2f, b = 123, c = byte.MaxValue, d = ushort.MaxValue };
            writer.block(in expected);
            Assert.True(writer.LengthWritten < Unsafe.SizeOf<SimpleStruct>());
        }

        [Test]
        public void SimpleStructReadWrite()
        {
            var buffer = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            var expected = new SimpleStruct { a = 1.2f, b = 123, c = byte.MaxValue, d = ushort.MaxValue };
            buffer.block(expected);
            buffer.Align();
            var allocated = new byte[ushort.MaxValue];
            buffer.ToSpan(allocated);
            var reader = new BitBufferReader<SevenBitDecoding>(allocated.Length);
            reader.CopyFrom(allocated);
            Assert.AreEqual(expected, reader.block<SimpleStruct>());
        }          
    }
}
