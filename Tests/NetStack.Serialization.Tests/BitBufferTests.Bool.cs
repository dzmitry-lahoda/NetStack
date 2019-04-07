

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
            var buffer = new BitBuffer();
            buffer.AddBool(true);
            var data = buffer.ToArray();
            var reader = new BitBuffer();
            reader.FromArray(data);
            Assert.True(reader.PeekBool());
            Assert.True(reader.ReadBool());
        }

        [Fact]
        public void BoolWritePeekRead1024()
        {
            var buffer = new BitBuffer();
            for (int i = 0; i < 1024; i++)
            {
                buffer.AddBool(true);
            }

            var data = buffer.ToArray();
            var reader = new BitBuffer();
            reader.FromArray(data);
            for (int i = 0; i < 1024; i++)
            {
                Assert.True(reader.PeekBool());
                Assert.True(reader.ReadBool());
            }
        }

        [Fact]
        public void BoolWritePeek128()
        {
            var buffer = new BitBuffer();
            buffer.AddBool(true);
            var data = buffer.ToArray();
            var reader = new BitBuffer();
            reader.FromArray(data);
            for (int i = 0; i < 128; i++)
            {
                Assert.True(reader.PeekBool());
            }
        }

        [Fact]
        public void BoolReadWrite()
        {
            var buffer = new BitBuffer();
            buffer.AddBool(true);
            buffer.Finish();
            var allocated = new byte[ushort.MaxValue];
            buffer.ToArray(allocated);
            var reader = new BitBuffer(allocated.Length);
            reader.FromArray(allocated);
            Assert.True(reader.ReadBool());
        }

        [Fact]
        public void TrueFalseTrueReadWrite()
        {
            var buffer = new BitBuffer();
            buffer.AddBool(true);
            buffer.AddBool(false);
            buffer.AddBool(true);
            buffer.Finish();
            var allocated = new byte[ushort.MaxValue];
            buffer.ToArray(allocated);
            var reader = new BitBuffer(allocated.Length);
            reader.FromArray(allocated);
            Assert.True(reader.ReadBool());
            Assert.False(reader.ReadBool());
            Assert.True(reader.ReadBool());
        }
    }
}