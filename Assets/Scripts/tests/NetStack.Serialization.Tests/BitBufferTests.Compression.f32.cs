using System;
using System.Numerics;
using NUnit.Framework;

namespace NetStack.Serialization
{
    public partial class BitBufferTests
    {
        [Test]
        public void AngleHalf()
        {
            var buffer = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            var angle = 359.1f;
            buffer.f32Half(angle);
            Assert.AreEqual(16, buffer.BitsWritten);
            var data = buffer.ToArray();
            var reader = new BitBufferReader<SevenBitDecoding<u32ArrayMemory>>();
            reader.CopyFrom(data);
            var result = reader.f32Half();
            Assert.AreEqual(359, result);
        }

        [Test]
        public void AngleLimitsAndTwoPrecision()
        {
            var buffer = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            var angle = 359.01f;
            buffer.f32(angle, 0f, 360f, 0.01f);
            Assert.AreEqual(16, buffer.BitsWritten);
            var data = buffer.ToArray();
            var reader = new BitBufferReader<SevenBitDecoding<u32ArrayMemory>>();
            reader.CopyFrom(data);
            var result = reader.f32(0f, 360f, 0.01f);
            Assert.AreEqual(359.01f, result, 2);
        }

        [Test]
        public void AngleZero()
        {
            var buffer = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            var angle = 0f;
            buffer.f32(angle);
            Assert.AreEqual(32, buffer.BitsWritten);
            var data = buffer.ToArray();
            var reader = new BitBufferReader<SevenBitDecoding<u32ArrayMemory>>();
            reader.CopyFrom(data);
            var result = reader.f32();
            Assert.AreEqual(0f, result, 1);
        }

        [Test]
        public void AngleQuantization()
        {
            var buffer = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            var angle = 35901u;
            buffer.u32(angle, 0, 36000);
            Assert.AreEqual(16, buffer.BitsWritten);
            var data = buffer.ToArray();
            var reader = new BitBufferReader<SevenBitDecoding<u32ArrayMemory>>();
            reader.CopyFrom(data);
            var result = reader.f32(0f, 360f, 0.01f);
            Assert.AreEqual(359.01f, result, 2);
        }
    }
}
