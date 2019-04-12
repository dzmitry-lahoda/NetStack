

using System;
using System.Numerics;
using Xunit;

namespace NetStack.Serialization
{
    partial class BitBufferTests
    {
        [Fact]
        public void PeekBool()
        {
            var writer = new BitBufferWriter();
            writer.b(true);
            var data = writer.ToArray();
            var reader = new BitBufferReader();
            reader.FromArray(data);
            Assert.True(reader.PeekBool());
            Assert.True(reader.b());
        }

        [Fact]
        public void BoolWritePeekRead1024()
        {
            var write = new BitBufferWriter();
            for (int i = 0; i < 1024; i++)
            {
                write.b(true);
            }

            var data = write.ToArray();
            var reader = new BitBufferReader();
            reader.FromArray(data);
            for (int i = 0; i < 1024; i++)
            {
                Assert.True(reader.PeekBool());
                Assert.True(reader.b());
            }
        }

        [Fact]
        public void BoolWritePeek128()
        {
            var writer = new BitBufferWriter();
            writer.b(true);
            var data = writer.ToArray();
            var reader = new BitBufferReader();
            reader.FromArray(data);
            for (int i = 0; i < 128; i++)
                Assert.True(reader.PeekBool());
        }

        [Fact]
        public void BoolReadWrite()
        {
            var writer = new BitBufferWriter();
            writer.b(true);
            writer.Finish();
            var allocated = new byte[ushort.MaxValue];
            writer.ToArray(allocated);
            var reader = new BitBufferReader(allocated.Length);
            reader.FromArray(allocated);
            Assert.True(reader.b());
        }

        [Fact]
        public void TrueFalseTrueReadWrite()
        {
            var writer = new BitBufferWriter();
            writer.b(true);
            writer.b(false);
            writer.b(true);
            writer.Finish();
            var allocated = new byte[ushort.MaxValue];
            writer.ToArray(allocated);
            var reader = new BitBufferReader(allocated.Length);
            reader.FromArray(allocated);
            Assert.True(reader.b());
            Assert.False(reader.b());
            Assert.True(reader.b());
        }
    }
}