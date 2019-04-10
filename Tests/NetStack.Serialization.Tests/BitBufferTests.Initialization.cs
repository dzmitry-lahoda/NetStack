using System;
using System.Numerics;
using Xunit;

namespace NetStack.Serialization
{
    public partial class BitBufferTests
    {
        [Fact]
        public void RandomManyTimes()
        {
            var buffer = new BitBuffer();
            var random = new Random(42);
            for (var i = 0; i < short.MaxValue; i++)
            {
                for (var j = 0; j < BitBuffer.MtuIeee802; j++)
                {
                    if (random.Next() % 11 == 0)
                        buffer.AddLong(long.MaxValue);
                    if (random.Next() % 7 == 0)
                        buffer.AddInt(int.MaxValue);
                    if (random.Next() % 5 == 0)
                        buffer.AddShort(short.MaxValue);
                    if (random.Next() % 3 == 0)
                        buffer.AddBool(true);                        
                }

                var result = buffer.ToArray();
                buffer.FromArray(result);
                buffer.Clear();
            }
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
            Span<byte> span = new byte[buffer.LengthWritten];
            ReadOnlySpan<byte> read = span;
            buffer.ToSpan(span);
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
