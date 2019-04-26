using System;
using System.Numerics;
using NUnit.Framework;

namespace NetStack.Serialization
{
    public partial class BitBufferTests
    {
        [Test]
        public void BoolIsBit7()
        {
            var buffer = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            buffer.b(true);
            buffer.b(true);
            buffer.b(true);
            buffer.b(true);
            buffer.b(true);
            buffer.b(true);
            buffer.b(true);
            Assert.AreEqual(1, buffer.LengthWritten);
        }

        [Test]
        public void BoolIsBit9()
        {
            var buffer = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            buffer.b(true);
            buffer.b(true);
            buffer.b(true);
            buffer.b(true);
            buffer.b(true);
            buffer.b(true);
            buffer.b(true);
            buffer.b(true);

            buffer.b(true);
            Assert.AreEqual(2, buffer.LengthWritten);
        }

        [Test]
        public void ByteMax3()
        {
            var buffer = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            buffer.u8(byte.MaxValue);
            buffer.u8(byte.MaxValue);
            buffer.u8(byte.MaxValue);
            Assert.AreEqual(3, buffer.LengthWritten);
        }

        [Test]
        public void ByteMin3()
        {
            var buffer = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            buffer.u8(byte.MinValue);
            buffer.u8(byte.MinValue);
            buffer.u8(byte.MinValue);
            Assert.AreEqual(3, buffer.LengthWritten);
        }

        [Test]
        public void ByteMin8()
        {
            var buffer = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            buffer.u8(byte.MinValue);
            buffer.u8(byte.MinValue);
            buffer.u8(byte.MinValue);
            buffer.u8(byte.MinValue);
            buffer.u8(byte.MinValue);
            buffer.u8(byte.MinValue);
            buffer.u8(byte.MinValue);
            buffer.u8(byte.MinValue);
            Assert.AreEqual(8, buffer.LengthWritten);
        }

        [Test]
        public void ByteRangeMin8()
        {
            var buffer = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            buffer.u8(byte.MinValue, 0, (byte)sbyte.MaxValue);
            buffer.u8(byte.MinValue, 0, (byte)sbyte.MaxValue);
            buffer.u8(byte.MinValue, 0, (byte)sbyte.MaxValue);
            buffer.u8(byte.MinValue, 0, (byte)sbyte.MaxValue);
            buffer.u8(byte.MinValue, 0, (byte)sbyte.MaxValue);
            buffer.u8(byte.MinValue, 0, (byte)sbyte.MaxValue);
            buffer.u8(byte.MinValue, 0, (byte)sbyte.MaxValue);
            buffer.u8(byte.MinValue, 0, (byte)sbyte.MaxValue);
            Assert.AreEqual(7, buffer.LengthWritten);
        }

        [Test]
        public void ByteRangeMin8Max10()
        {
            var buffer = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            buffer.u8(byte.MinValue, 0, 10);
            buffer.u8(byte.MinValue, 0, 10);
            buffer.u8(byte.MinValue, 0, 10);
            buffer.u8(byte.MinValue, 0, 10);
            buffer.u8(byte.MinValue, 0, 10);
            buffer.u8(byte.MinValue, 0, 10);
            buffer.u8(byte.MinValue, 0, 10);
            buffer.u8(byte.MinValue, 0, 10);
            Assert.AreEqual(4, buffer.LengthWritten);
        }        

        [Test]
        public void ByteHalf()
        {
            var buffer = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            buffer.u8(byte.MaxValue / 2);
            buffer.u8(byte.MaxValue / 2);
            buffer.u8(byte.MaxValue / 2);
            buffer.u8(byte.MaxValue / 2);
            buffer.u8(byte.MaxValue / 2);
            buffer.u8(byte.MaxValue / 2);
            buffer.u8(byte.MaxValue / 2);
            buffer.u8(byte.MaxValue / 2);
            Assert.AreEqual(8, buffer.LengthWritten);
        }
        

        [Test]
        public void ByteShortMin4()
        {
            var buffer = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            buffer.i16(short.MinValue);
            buffer.i16(short.MinValue);
            buffer.i16(short.MinValue);
            buffer.i16(short.MinValue);
            Assert.AreEqual(12, buffer.LengthWritten);
        }

        [Test]
        public void ByteShort0()
        {
            var buffer = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            buffer.i16(0);
            buffer.i16(0);
            buffer.i16(0);
            buffer.i16(0);
            Assert.AreEqual(4, buffer.LengthWritten);
        }

        [Test]
        public void ShortMinMax4()
        {
            var buffer = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            buffer.i16(short.MaxValue, short.MinValue, short.MaxValue);
            buffer.i16(short.MaxValue, short.MinValue, short.MaxValue);
            buffer.i16(short.MaxValue, short.MinValue, short.MaxValue);
            buffer.i16(short.MaxValue, short.MinValue, short.MaxValue);
            Assert.AreEqual(8, buffer.LengthWritten);
        }

        [Test]
        public void UIntMax()
        {
            var buffer = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            buffer.u32(uint.MaxValue);
            Assert.AreEqual(5, buffer.LengthWritten);
        }

        [Test]
        public void UIntMin()
        {
            var writer = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            writer.u32(uint.MinValue);
            Assert.AreEqual(1, writer.LengthWritten);
        }

        [Test]
        public void IntMaxValue4()
        {
            var writer = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            writer.i32(int.MaxValue);
            writer.i32(int.MaxValue);
            writer.i32(int.MaxValue);
            writer.i32(int.MaxValue);
            Assert.AreEqual(20, writer.LengthWritten);
        }

        [Test]
        public void IntZeroMaxValue4()
        {
            var writer = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            writer.i32(int.MaxValue, 0, int.MaxValue);
            writer.i32(int.MaxValue, 0, int.MaxValue);
            writer.i32(int.MaxValue, 0, int.MaxValue);
            writer.i32(int.MaxValue, 0, int.MaxValue);
            Assert.AreEqual(16, writer.LengthWritten);
        }

        [Test]
        public void ShortMax4Divided()
        {
            var writer = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            for (var i=0; i< 8;i++)
                writer.i16(short.MaxValue/3);
                                  
            Assert.AreEqual(24, writer.LengthWritten);
        }

        [Test]
        public void IntOfShortMax4Divided()
        {
            var writer = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            for (var i=0; i< 8;i++)
                writer.i32(short.MaxValue/3);
            Assert.AreEqual(24, writer.LengthWritten);
        }

        [Test]
        public void ShortZeroMax4()
        {
            var writer = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            writer.i16(short.MaxValue, 0, short.MaxValue);
            writer.i16(short.MaxValue, 0, short.MaxValue);
            writer.i16(short.MaxValue, 0, short.MaxValue);
            writer.i16(short.MaxValue, 0, short.MaxValue);
            Assert.AreEqual(8, writer.LengthWritten);
        }

        [Test]
        public void SByte0()
        {
            var writer = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            writer.i8(0);
            writer.i8(0);
            writer.i8(0);
            writer.i8(0);
            Assert.AreEqual(4, writer.LengthWritten);
        }

        [Test]
        public void SbyteMax4()
        {
            var writer = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            writer.i8(sbyte.MaxValue);
            writer.i8(sbyte.MaxValue);
            writer.i8(sbyte.MaxValue);
            writer.i8(sbyte.MaxValue);
            writer.i8(sbyte.MaxValue);
            writer.i8(sbyte.MaxValue);
            writer.i8(sbyte.MaxValue);
            writer.i8(sbyte.MaxValue);
            Assert.AreEqual(8, writer.LengthWritten);
        }

        [Test]
        public void SByteZeroMax4()
        {
            var writer = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            writer.i8(sbyte.MaxValue, 0, sbyte.MaxValue);
            writer.i8(sbyte.MaxValue, 0, sbyte.MaxValue);
            writer.i8(sbyte.MaxValue, 0, sbyte.MaxValue);
            writer.i8(sbyte.MaxValue, 0, sbyte.MaxValue);
            writer.i8(sbyte.MaxValue, 0, sbyte.MaxValue);
            writer.i8(sbyte.MaxValue, 0, sbyte.MaxValue);
            writer.i8(sbyte.MaxValue, 0, sbyte.MaxValue);
            writer.i8(sbyte.MaxValue, 0, sbyte.MaxValue);
            Assert.AreEqual(7, writer.LengthWritten);
        }
    }
}
