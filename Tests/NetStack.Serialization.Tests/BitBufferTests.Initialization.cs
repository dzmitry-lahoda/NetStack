using System;
using System.Numerics;
using Xunit;
using i8 = System.SByte;
using i16 = System.Int16;
using i32 = System.Int32;
using i64 = System.Int64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;
using f32 = System.Single;
using f64 = System.Double;

namespace NetStack.Serialization
{
    public partial class BitBufferTests
    {
        [Fact]
        public void RandomManyTimes()
        {
            var writer = new BitBufferWriter<SevenBit>();
            var reader = new BitBufferReader<SevenBitRe>();
            var random = new Random(42);
            for (var i = 0; i < i16.MaxValue; i++)
            {
                for (var j = 0; j < BitBufferLimits.MtuIeee802; j++)
                {
                    if (random.Next() % 11 == 0)
                        writer.i64(long.MaxValue);
                    if (random.Next() % 7 == 0)
                        writer.i32(int.MaxValue);
                    if (random.Next() % 5 == 0)
                        writer.i16(i16.MaxValue);
                    if (random.Next() % 3 == 0)
                        writer.b(true);                        
                }


                var result = writer.ToArray();
                reader.FromArray(result);
                writer.Clear();
            }
        }

        [Fact]
        public void ToArrayFromFrom()
        {
            var writer = new BitBufferWriter<SevenBit>(100);
            writer.i64(i64.MaxValue);
            writer.i32(i32.MaxValue);
            writer.i16(i16.MaxValue);
            var result = writer.ToArray();
            var reader = new BitBufferReader<SevenBitRe>();
            reader.FromArray(result);
            Assert.Equal(i64.MaxValue, reader.i64());
            Assert.Equal(i32.MaxValue, reader.i32());
            Assert.Equal(i16.MaxValue, reader.i16());
            reader.FromArray(result);
            Assert.Equal(i64.MaxValue, reader.i64());
            Assert.Equal(i32.MaxValue, reader.i32());
            Assert.Equal(i16.MaxValue, reader.i16());
        }

        [Fact]
        public void ToSpanFromFrom()
        {
            var writer = new BitBufferWriter<SevenBit>(100);
            writer.i64(i64.MaxValue);
            writer.i32(i32.MaxValue);
            writer.i16(i16.MaxValue);
            Span<byte> span = new byte[writer.LengthWritten];
            ReadOnlySpan<byte> read = span;
            writer.ToSpan(span);
            var reader = new BitBufferReader<SevenBitRe>();            
            reader.FromSpan(read);
            Assert.Equal(i64.MaxValue, reader.i64());
            Assert.Equal(i32.MaxValue, reader.i32());
            Assert.Equal(i16.MaxValue, reader.i16());
            reader.FromSpan(read);
            Assert.Equal(i64.MaxValue, reader.i64());
            Assert.Equal(i32.MaxValue, reader.i32());
            Assert.Equal(i16.MaxValue, reader.i16());
        }
    }
}
