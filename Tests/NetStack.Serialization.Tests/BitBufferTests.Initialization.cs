using System;
using System.Numerics;
using Xunit;

namespace NetStack.Serialization
{
    public partial class BitBufferTests
    {
        [Fact]
        public void Capacity()
        {
            var buffer = new BitBuffer(100);
            Assert.Equal(0, buffer.Length);
            var received = new byte[2000];
            buffer.FromArray(received);
            Assert.Equal(1999, buffer.Length);
        }

        [Fact]
        public void ToArrayFromFrom()
        {
            var buffer = new BitBuffer(100);
            buffer.AddLong(long.MaxValue);
            buffer.AddInt(int.MaxValue);
            buffer.AddShort(short.MaxValue);
            var result = buffer.ToArray();
            buffer.FromArray(result);
            Assert.Equal(long.MaxValue, buffer.ReadLong());
            Assert.Equal(int.MaxValue, buffer.ReadInt());
            Assert.Equal(short.MaxValue, buffer.ReadShort());
            buffer.FromArray(result);
            Assert.Equal(long.MaxValue, buffer.ReadLong());
            Assert.Equal(int.MaxValue, buffer.ReadInt());
            Assert.Equal(short.MaxValue, buffer.ReadShort());
        }

        [Fact]
        public void ToSpanFromFrom()
        {
            var buffer = new BitBuffer(100);
            buffer.AddLong(long.MaxValue);
            buffer.AddInt(int.MaxValue);
            buffer.AddShort(short.MaxValue);
            Span<byte> span = new byte[buffer.Length];
            ReadOnlySpan<byte> read = span;
            buffer.ToSpan(ref span);
            buffer.FromSpan(in read);
            Assert.Equal(long.MaxValue, buffer.ReadLong());
            Assert.Equal(int.MaxValue, buffer.ReadInt());
            Assert.Equal(short.MaxValue, buffer.ReadShort());
            buffer.FromSpan(in read);
            Assert.Equal(long.MaxValue, buffer.ReadLong());
            Assert.Equal(int.MaxValue, buffer.ReadInt());
            Assert.Equal(short.MaxValue, buffer.ReadShort());
        }        
    }
}
