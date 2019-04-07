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
            var buffer = new BitBuffer();
            var angle = 359.1f;
            buffer.AddHalfFloat(angle);     
            Assert.Equal(16, buffer.BitsPassed2);       
            var data = buffer.ToArray();
            buffer.FromArray(data);
            var result = buffer.ReadHalfFloat();
            Assert.Equal(359, result);            
        }

        [Fact]
        public void AngleLimitsAndTwoPrecision()
        {
            var buffer = new BitBuffer();
            var angle = 359.01f;
            buffer.AddFloat(angle, 0f, 360f,0.01f);
            Assert.Equal(16, buffer.BitsPassed2);
            var data = buffer.ToArray();
            buffer.FromArray(data);
            var result = buffer.ReadFloat(0f, 360f,0.01f);
            Assert.Equal(359.01f, result, 2);
        }

        [Fact]
        public void AngleZero()
        {
            var buffer = new BitBuffer();
            var angle = 0f;
            buffer.AddFloat(angle);
            Assert.Equal(32, buffer.BitsPassed2);
            var data = buffer.ToArray();
            buffer.FromArray(data);
            var result = buffer.ReadFloat();
            Assert.Equal(0f, result, 1);
        }        

        [Fact]
        public void AngleQuantization()
        {
            var buffer = new BitBuffer();
            var angle = 35901u;
            buffer.AddUInt(angle, 0, 36000);
            Assert.Equal(16, buffer.BitsPassed2);
            var data = buffer.ToArray();
            buffer.FromArray(data);
            var result = buffer.ReadFloat(0f, 360f,0.01f);
            Assert.Equal(359.01f, result,  2);
        }        
    }
}
