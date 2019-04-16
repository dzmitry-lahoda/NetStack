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
        public void u16ReadWrite()
        {
            var writer = new BitBufferWriter<SevenBitEncoding>();
            writer.u16(u16.MinValue);
            writer.u16(u16.MaxValue / 2);
            writer.u16(u16.MaxValue);
            var data = writer.ToArray();
            writer.ToArray();
            var reader = new BitBufferReader<SevenBitDecoding>();
            reader.CopyFrom(data);
            Assert.AreEqual(u16.MinValue, reader.u16());
            var half = u16.MaxValue / 2;
            Assert.AreEqual(half, reader.u16());
            Assert.AreEqual(u16.MaxValue, reader.u16());
        }     

        [Test]
        public void u16ReadWriteLimits()
        {
            var writer = new BitBufferWriter<SevenBitEncoding>();
            writer.u16(2, 0, 10);
            writer.u16(1, 3);
            var data =writer.ToArray();
            var reader = new BitBufferReader<SevenBitDecoding>();
            reader.CopyFrom(data);
            Assert.AreEqual(2, reader.i16(0, 10));
            Assert.AreEqual(1, reader.u16(3));
        }           
    }
}
