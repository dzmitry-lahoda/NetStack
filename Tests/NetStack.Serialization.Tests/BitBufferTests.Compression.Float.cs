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
            var buffer = new BitBufferWrite();
            var angle = 359.1f;
            buffer.AddHalfFloat(angle);
            Assert.Equal(16, buffer.BitsWritten);
            var data = buffer.ToArray();
            var reader = new BitBufferReader();
            reader.FromArray(data);
            var result = reader.ReadHalfFloat();
            Assert.Equal(359, result);
        }

        [Fact]
        public void AngleLimitsAndTwoPrecision()
        {
            var buffer = new BitBufferWrite();
            var angle = 359.01f;
            buffer.AddFloat(angle, 0f, 360f, 0.01f);
            Assert.Equal(16, buffer.BitsWritten);
            var data = buffer.ToArray();
            var reader = new BitBufferReader();
            reader.FromArray(data);
            var result = reader.ReadFloat(0f, 360f, 0.01f);
            Assert.Equal(359.01f, result, 2);
        }

        [Fact]
        public void AngleZero()
        {
            var buffer = new BitBufferWrite();
            var angle = 0f;
            buffer.AddFloat(angle);
            Assert.Equal(32, buffer.BitsWritten);
            var data = buffer.ToArray();
            var reader = new BitBufferReader();
            reader.FromArray(data);
            var result = reader.ReadFloat();
            Assert.Equal(0f, result, 1);
        }

        [Fact]
        public void AngleQuantization()
        {
            var buffer = new BitBufferWrite();
            var angle = 35901u;
            buffer.AddUInt(angle, 0, 36000);
            Assert.Equal(16, buffer.BitsWritten);
            var data = buffer.ToArray();
            var reader = new BitBufferReader();
            reader.FromArray(data);
            var result = reader.ReadFloat(0f, 360f, 0.01f);
            Assert.Equal(359.01f, result, 2);
        }
    }
}
