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
    partial class BitBufferTests
    {
        [Test]
        public void i16ReadWrite()
        {
            var writer = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            writer.i16(i16.MinValue);
            writer.i16(i16.MinValue / 2);
            writer.i16(0);
            i16 half = i16.MaxValue / 2;
            writer.i16(half);
            writer.i16(i16.MaxValue);
            writer.Align();
            var allocated = new byte[i16.MaxValue];
            writer.ToSpan(allocated);
            var reader = new BitBufferReader<SevenBitDecoding>(allocated.Length);
            reader.CopyFrom(allocated);
            Assert.AreEqual(i16.MinValue, reader.i16());
            Assert.AreEqual(i16.MinValue / 2, reader.i16());
            Assert.AreEqual(0, reader.i16());
            Assert.AreEqual(half, reader.i16Peek());
            Assert.AreEqual(half, reader.i16());
            Assert.AreEqual(i16.MaxValue, reader.i16());
        }

        [Test]
        public void i16ReadWriteLimits()
        {
            var writer = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            writer.i16(-1, -2, 2);
            writer.i16(-1, 4);
            var allocated = new byte[i16.MaxValue];
            writer.ToSpan(allocated);
            var reader = new BitBufferReader<SevenBitDecoding>(allocated.Length);
            reader.CopyFrom(allocated);
            Assert.AreEqual(-1, reader.i16(-2, 2));
            Assert.AreEqual(-1, reader.i16(4));
        }
    }
}
