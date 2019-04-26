using System;
using System.Numerics;
using NUnit.Framework;
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
        [Test]
        public void RandomManyTimes()
        {
            var writer = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            var reader = new BitBufferReader<SevenBitDecoding>();
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
                reader.CopyFrom(result);
                writer.Reset();
            }
        }

        [Test]
        public void ToArrayFromFrom()
        {
            var writer = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>(100);
            writer.i64(i64.MaxValue);
            writer.i32(i32.MaxValue);
            writer.i16(i16.MaxValue);
            var result = writer.ToArray();
            var reader = new BitBufferReader<SevenBitDecoding>();
            reader.CopyFrom(result);
            Assert.AreEqual(i64.MaxValue, reader.i64());
            Assert.AreEqual(i32.MaxValue, reader.i32());
            Assert.AreEqual(i16.MaxValue, reader.i16());
            reader.CopyFrom(result);
            Assert.AreEqual(i64.MaxValue, reader.i64());
            Assert.AreEqual(i32.MaxValue, reader.i32());
            Assert.AreEqual(i16.MaxValue, reader.i16());
        }

        [Test]
        public void ToSpanFromFrom()
        {
            var writer = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>(100);
            writer.i64(i64.MaxValue);
            writer.i32(i32.MaxValue);
            writer.i16(i16.MaxValue);
            Span<byte> span = new byte[writer.LengthWritten];
            ReadOnlySpan<byte> read = span;
            writer.ToSpan(span);
            var reader = new BitBufferReader<SevenBitDecoding>();            
            reader.CopyFrom(read);
            Assert.AreEqual(i64.MaxValue, reader.i64());
            Assert.AreEqual(i32.MaxValue, reader.i32());
            Assert.AreEqual(i16.MaxValue, reader.i16());
            reader.CopyFrom(read);
            Assert.AreEqual(i64.MaxValue, reader.i64());
            Assert.AreEqual(i32.MaxValue, reader.i32());
            Assert.AreEqual(i16.MaxValue, reader.i16());
        }

        [Test]
        public void ToFromArrayPosition()
        {
            var writer = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            writer.u8(13);
            writer.i64(i64.MaxValue);
            writer.Align();
            var allocated = new byte[ushort.MaxValue];
            writer.ToSpan(new Span<u8>(allocated, 10, 100));
            var reader = new BitBufferReader<SevenBitDecoding>(allocated.Length);
            reader.CopyFrom(new ReadOnlySpan<u8>(allocated, 10, 100));
            Assert.AreEqual(13, reader.u8());
            Assert.AreEqual(long.MaxValue, reader.i64());
        }  

        [Test]
        public void RawToEncoded()
        {
            var rawWriter = new BitBufferWriter<RawEncoding<u32ArrayMemory>>();
            rawWriter.i32(i32.MaxValue - 13);
            rawWriter.u32(u32.MaxValue - 666);
            var writer = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>(rawWriter);
            writer.i32(i32.MaxValue - 13);
            writer.u32(u32.MaxValue - 666);
            var data = writer.ToArray();
            var rawReader = new BitBufferReader<RawDecoding>();
            rawReader.CopyFrom(data);
            Assert.AreEqual(i32.MaxValue - 13, rawReader.i32());
            Assert.AreEqual(u32.MaxValue - 666, rawReader.u32());
            var reader = new BitBufferReader<SevenBitDecoding>(rawReader);
            Assert.AreEqual(i32.MaxValue - 13, reader.i32());
            Assert.AreEqual(u32.MaxValue - 666, reader.u32());            
        }                 
    }
}
