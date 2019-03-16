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
            var buffer = new BitBuffer();
            var expected = new SimpleStruct { a = 1.2f, b = 123, c = byte.MaxValue, d = ushort.MaxValue };
            buffer.AddUnsafe(in expected);
            Assert.True(buffer.Length < Unsafe.SizeOf<SimpleStruct>());
        }

        [Fact]
        public void SimpleStructReadWrite()
        {
            var buffer = new BitBuffer();
            var expected = new SimpleStruct { a = 1.2f, b = 123, c = byte.MaxValue, d = ushort.MaxValue };
            buffer.AddUnsafe(expected);
            buffer.Finish();
            var allocated = new byte[ushort.MaxValue];
            buffer.ToArray(allocated);
            var reader = new BitBuffer(allocated.Length);
            reader.FromArray(allocated, allocated.Length);
            Assert.Equal(expected, reader.ReadUnsafe<SimpleStruct>());
        }          
    }
}
