using System;
using Xunit;

namespace NetStack.Compression
{
    public class HalfPrecisionTests
    {
        [Fact]
        public void f123dot456()
        {
            float f = 123.456f;
            ushort compressed = HalfPrecision.Compress(f);
            float decompressed = HalfPrecision.Decompress(compressed);
            Assert.Equal(f, decompressed, 0);
            Assert.NotEqual(f, decompressed, 1);
        }

        [Fact]
        public void f123456()
        {
            float f = 123456f;
            ushort compressed = HalfPrecision.Compress(f);
            float decompressed = HalfPrecision.Decompress(compressed);
            Assert.True(decompressed <= ushort.MaxValue + 1);
            Assert.True(ushort.MaxValue <= decompressed);              
        }     

        [Fact]
        public void f12dot3456()
        {
            float f = 12.3456f;
            ushort compressed = HalfPrecision.Compress(f);
            float decompressed = HalfPrecision.Decompress(compressed);
            Assert.Equal(f, decompressed, 1);
            Assert.NotEqual(f, decompressed, 2);
        }             
    }
}
