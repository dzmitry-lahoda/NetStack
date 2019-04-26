using System;
using NUnit.Framework;

namespace NetStack.Serialization
{
    public class HalfPrecisionTests
    {
        [Test]
        public void f123dot456()
        {
            float f = 123.456f;
            ushort compressed = HalfPrecision.Compress(f);
            float decompressed = HalfPrecision.Decompress(compressed);
            Assert.AreEqual(f, decompressed, 0.1);
            Assert.AreNotEqual(f, decompressed);
        }

        [Test]
        public void f123456()
        {
            float f = 123456f;
            ushort compressed = HalfPrecision.Compress(f);
            float decompressed = HalfPrecision.Decompress(compressed);
            Assert.True(decompressed <= ushort.MaxValue + 1);
            Assert.True(ushort.MaxValue <= decompressed);              
        }     

        [Test]
        public void f12dot3456()
        {
            float f = 12.3456f;
            ushort compressed = HalfPrecision.Compress(f);
            float decompressed = HalfPrecision.Decompress(compressed);
            Assert.AreEqual(f, decompressed, 1);
            Assert.AreNotEqual(f, decompressed);
        }             
    }
}
