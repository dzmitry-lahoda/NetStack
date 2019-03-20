using System;
using System.Numerics;
using Xunit;

namespace NetStack.Serialization
{
    public partial class BitBufferTests
    {
        [Fact]
        public void Capacity()
        {
            var buffer = new BitBuffer(100);
            Assert.Equal(0, buffer.Length);
            var received = new byte[2000];
            buffer.FromArray(received);
            Assert.Equal(1999, buffer.Length);
        }
    }
}
