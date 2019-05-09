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
        public void FibonacciVsSevenBit0()
        {
            var fib = new BitBufferWriter<FibonacciEncoding<u32ArrayMemory>>();
            u16 value = 0;
            fib.u16(value);
            Assert.AreEqual(2, fib.BitsWritten);

            var sb = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            sb.u16(value);
            Assert.AreEqual(8, sb.BitsWritten);
        }

        [Test]
        public void FibonacciVsSevenBit1()
        {
            var fib = new BitBufferWriter<FibonacciEncoding<u32ArrayMemory>>();
            u16 value = 1;
            fib.u16(value);
            Assert.AreEqual(3, fib.BitsWritten);

            var sb = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            sb.u16(value);
            Assert.AreEqual(8, sb.BitsWritten);
        }

        [Test]
        public void FibonacciVsSevenBit2()
        {
            var fib = new BitBufferWriter<FibonacciEncoding<u32ArrayMemory>>();
            u16 value = 2;
            fib.u16(value);
            Assert.AreEqual(4, fib.BitsWritten);

            var sb = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            sb.u16(value);
            Assert.AreEqual(8, sb.BitsWritten);
        }

        [Test]
        public void FibonacciVsSevenBit4()
        {
            var fib = new BitBufferWriter<FibonacciEncoding<u32ArrayMemory>>();
            u16 value = 4;
            fib.u16(value);
            Assert.AreEqual(5, fib.BitsWritten);

            var sb = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            sb.u16(value);
            Assert.AreEqual(8, sb.BitsWritten);
        }

        [Test]
        public void FibonacciVsSevenBit8()
        {
            var fib = new BitBufferWriter<FibonacciEncoding<u32ArrayMemory>>();
            u16 value = 8;
            fib.u16(value);
            Assert.AreEqual(6, fib.BitsWritten);

            var sb = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            sb.u16(value);
            Assert.AreEqual(8, sb.BitsWritten);
        }

        [Test]
        public void FibonacciVsSevenBit16()
        {
            var fib = new BitBufferWriter<FibonacciEncoding<u32ArrayMemory>>();
            u16 value = 16;
            fib.u16(value);
            Assert.AreEqual(7, fib.BitsWritten);

            var sb = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            sb.u16(value);
            Assert.AreEqual(8, sb.BitsWritten);
        }

        [Test]
        public void FibonacciVsSevenBit32()
        {
            var fib = new BitBufferWriter<FibonacciEncoding<u32ArrayMemory>>();
            u16 value = 32;
            fib.u16(value);
            Assert.AreEqual(8, fib.BitsWritten);

            var sb = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            sb.u16(value);
            Assert.AreEqual(8, sb.BitsWritten);
        }

        [Test]
        public void FibonacciVsSevenBit64()
        {
            var fib = new BitBufferWriter<FibonacciEncoding<u32ArrayMemory>>();
            u16 value = 64;
            fib.u16(value);
            Assert.AreEqual(10, fib.BitsWritten);

            var sb = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            sb.u16(value);
            Assert.AreEqual(8, sb.BitsWritten);
        }

        [Test]
        public void FibonacciVsSevenBit128()
        {
            var fib = new BitBufferWriter<FibonacciEncoding<u32ArrayMemory>>();
            u16 value = 128;
            fib.u16(value);
            Assert.AreEqual(11, fib.BitsWritten);

            var sb = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            sb.u16(value);
            Assert.AreEqual(16, sb.BitsWritten);
        }

        [Test]
        public void FibonacciVsSevenBit256()
        {
            var fib = new BitBufferWriter<FibonacciEncoding<u32ArrayMemory>>();
            u16 value = 256;
            fib.u16(value);
            Assert.AreEqual(13, fib.BitsWritten);

            var sb = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            sb.u16(value);
            Assert.AreEqual(16, sb.BitsWritten);
        }

        [Test]
        public void FibonacciVsSevenBitFrom0To255()
        {
            var fib = new BitBufferWriter<FibonacciEncoding<u32ArrayMemory>>();
            for (var i = 0; i <= byte.MaxValue; i++)
                fib.u16((u16)i);

            Assert.AreEqual(2732, fib.BitsWritten);

            var sb = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            for (var i = 0; i <= byte.MaxValue; i++)
                sb.u16((u16)i);
            Assert.AreEqual(3072, sb.BitsWritten);
        }

        [Test]
        public void FibonacciVsSevenBitFrom256To32766()
        {
            var fib = new BitBufferWriter<FibonacciEncoding<u32ArrayMemory>>(u16.MaxValue);
            for (var i = 256; i <= i16.MaxValue - 1; i++)
                fib.u16((u16)i);

            Assert.AreEqual(675908, fib.BitsWritten);

            var sb = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>(u16.MaxValue);
            for (var i = 256; i <= i16.MaxValue - 1; i++)
                sb.u16((u16)i);
            Assert.AreEqual(651240, sb.BitsWritten);
        }

    }
}
