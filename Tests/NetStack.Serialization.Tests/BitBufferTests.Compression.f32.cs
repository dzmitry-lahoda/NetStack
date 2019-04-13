using System;
using System.Numerics;
using Xunit;

namespace NetStack.Serialization
{
    public partial class BitBufferTests
    {
        [Fact]
        public void AngleHalf()
        {
            var buffer = new BitBufferWriter<SevenBitEncoding>();
            var angle = 359.1f;
            buffer.AddHalfFloat(angle);
            Assert.Equal(16, buffer.BitsWritten);
            var data = buffer.ToArray();
            var reader = new BitBufferReader<SevenBitDecoding>();
            reader.FromArray(data);
            var result = reader.ReadHalfFloat();
            Assert.Equal(359, result);
        }

        [Fact]
        public void AngleLimitsAndTwoPrecision()
        {
            var buffer = new BitBufferWriter<SevenBitEncoding>();
            var angle = 359.01f;
            buffer.f32(angle, 0f, 360f, 0.01f);
            Assert.Equal(16, buffer.BitsWritten);
            var data = buffer.ToArray();
            var reader = new BitBufferReader<SevenBitDecoding>();
            reader.FromArray(data);
            var result = reader.f32(0f, 360f, 0.01f);
            Assert.Equal(359.01f, result, 2);
        }

        [Fact]
        public void AngleZero()
        {
            var buffer = new BitBufferWriter<SevenBitEncoding>();
            var angle = 0f;
            buffer.f32(angle);
            Assert.Equal(32, buffer.BitsWritten);
            var data = buffer.ToArray();
            var reader = new BitBufferReader<SevenBitDecoding>();
            reader.FromArray(data);
            var result = reader.f32();
            Assert.Equal(0f, result, 1);
        }

        [Fact]
        public void AngleQuantization()
        {
            var buffer = new BitBufferWriter<SevenBitEncoding>();
            var angle = 35901u;
            buffer.u32(angle, 0, 36000);
            Assert.Equal(16, buffer.BitsWritten);
            var data = buffer.ToArray();
            var reader = new BitBufferReader<SevenBitDecoding>();
            reader.FromArray(data);
            var result = reader.f32(0f, 360f, 0.01f);
            Assert.Equal(359.01f, result, 2);
        }
    }
}
