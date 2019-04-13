using System;
using System.Runtime.CompilerServices;
using Xunit;

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

        [Fact]
        public void SimpleStructWrite()
        {
            var writer = new BitBufferWriter<SevenBitEncoding>();
            var expected = new SimpleStruct { a = 1.2f, b = 123, c = byte.MaxValue, d = ushort.MaxValue };
            writer.block(in expected);
            Assert.True(writer.LengthWritten < Unsafe.SizeOf<SimpleStruct>());
        }

        [Fact]
        public void SimpleStructReadWrite()
        {
            var buffer = new BitBufferWriter<SevenBitEncoding>();
            var expected = new SimpleStruct { a = 1.2f, b = 123, c = byte.MaxValue, d = ushort.MaxValue };
            buffer.block(expected);
            buffer.Finish();
            var allocated = new byte[ushort.MaxValue];
            buffer.ToArray(allocated);
            var reader = new BitBufferReader<SevenBitDecoding>(allocated.Length);
            reader.CopyFrom(allocated);
            Assert.Equal(expected, reader.block<SimpleStruct>());
        }          
    }
}
