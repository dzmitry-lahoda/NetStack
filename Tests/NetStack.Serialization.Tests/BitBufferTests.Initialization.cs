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
            var buffer = new BitBufferWrite();
            var reader = new BitBufferReader();
            var random = new Random(42);
            for (var i = 0; i < short.MaxValue; i++)
            {
                for (var j = 0; j < BitBufferLimits.MtuIeee802; j++)
                {
                    if (random.Next() % 11 == 0)
                        buffer.AddLong(long.MaxValue);
                    if (random.Next() % 7 == 0)
                        buffer.i32(int.MaxValue);
                    if (random.Next() % 5 == 0)
                        buffer.i16(short.MaxValue);
                    if (random.Next() % 3 == 0)
                        buffer.AddBool(true);                        
                }


                var result = buffer.ToArray();
                reader.FromArray(result);
                buffer.Clear();
            }
        }

        [Fact]
        public void ToArrayFromFrom()
        {
            var buffer = new BitBufferWrite(100);
            buffer.AddLong(long.MaxValue);
            buffer.i32(int.MaxValue);
            buffer.i16(short.MaxValue);
            var result = buffer.ToArray();
            var reader = new BitBufferReader();
            reader.FromArray(result);
            Assert.Equal(long.MaxValue, reader.ReadLong());
            Assert.Equal(int.MaxValue, reader.i32());
            Assert.Equal(short.MaxValue, reader.ReadShort());
            reader.FromArray(result);
            Assert.Equal(long.MaxValue, reader.ReadLong());
            Assert.Equal(int.MaxValue, reader.i32());
            Assert.Equal(short.MaxValue, reader.ReadShort());
        }

        [Fact]
        public void ToSpanFromFrom()
        {
            var buffer = new BitBufferWrite(100);
            buffer.AddLong(long.MaxValue);
            buffer.i32(int.MaxValue);
            buffer.i16(short.MaxValue);
            Span<byte> span = new byte[buffer.LengthWritten];
            ReadOnlySpan<byte> read = span;
            buffer.ToSpan(span);
            var reader = new BitBufferReader();            
            reader.FromSpan(read);
            Assert.Equal(long.MaxValue, reader.ReadLong());
            Assert.Equal(int.MaxValue, reader.i32());
            Assert.Equal(short.MaxValue, reader.ReadShort());
            reader.FromSpan(read);
            Assert.Equal(long.MaxValue, reader.ReadLong());
            Assert.Equal(int.MaxValue, reader.i32());
            Assert.Equal(short.MaxValue, reader.ReadShort());
        }
    }
}
