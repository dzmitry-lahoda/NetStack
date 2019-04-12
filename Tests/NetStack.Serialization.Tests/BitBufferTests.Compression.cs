using System;
using System.Numerics;
using Xunit;

namespace NetStack.Serialization
{
    public partial class BitBufferTests
    {
        [Fact]
        public void BoolIsBit7()
        {
            var buffer = new BitBufferWriter();
            buffer.b(true);
            buffer.b(true);
            buffer.b(true);
            buffer.b(true);
            buffer.b(true);
            buffer.b(true);
            buffer.b(true);
            Assert.Equal(1, buffer.LengthWritten);
        }

        [Fact]
        public void BoolIsBit9()
        {
            var buffer = new BitBufferWriter();
            buffer.b(true);
            buffer.b(true);
            buffer.b(true);
            buffer.b(true);
            buffer.b(true);
            buffer.b(true);
            buffer.b(true);
            buffer.b(true);

            buffer.b(true);
            Assert.Equal(2, buffer.LengthWritten);
        }

        [Fact]
        public void ByteMax3()
        {
            var buffer = new BitBufferWriter();
            buffer.u8(byte.MaxValue);
            buffer.u8(byte.MaxValue);
            buffer.u8(byte.MaxValue);
            Assert.Equal(3, buffer.LengthWritten);
        }

        [Fact]
        public void ByteMin3()
        {
            var buffer = new BitBufferWriter();
            buffer.u8(byte.MinValue);
            buffer.u8(byte.MinValue);
            buffer.u8(byte.MinValue);
            Assert.Equal(3, buffer.LengthWritten);
        }

        [Fact]
        public void ByteMin8()
        {
            var buffer = new BitBufferWriter();
            buffer.u8(byte.MinValue);
            buffer.u8(byte.MinValue);
            buffer.u8(byte.MinValue);
            buffer.u8(byte.MinValue);
            buffer.u8(byte.MinValue);
            buffer.u8(byte.MinValue);
            buffer.u8(byte.MinValue);
            buffer.u8(byte.MinValue);
            Assert.Equal(8, buffer.LengthWritten);
        }

        [Fact]
        public void ByteRangeMin8()
        {
            var buffer = new BitBufferWriter();
            buffer.u8(byte.MinValue, 0, (byte)sbyte.MaxValue);
            buffer.u8(byte.MinValue, 0, (byte)sbyte.MaxValue);
            buffer.u8(byte.MinValue, 0, (byte)sbyte.MaxValue);
            buffer.u8(byte.MinValue, 0, (byte)sbyte.MaxValue);
            buffer.u8(byte.MinValue, 0, (byte)sbyte.MaxValue);
            buffer.u8(byte.MinValue, 0, (byte)sbyte.MaxValue);
            buffer.u8(byte.MinValue, 0, (byte)sbyte.MaxValue);
            buffer.u8(byte.MinValue, 0, (byte)sbyte.MaxValue);
            Assert.Equal(7, buffer.LengthWritten);
        }

        [Fact]
        public void ByteRangeMin8Max10()
        {
            var buffer = new BitBufferWriter();
            buffer.u8(byte.MinValue, 0, 10);
            buffer.u8(byte.MinValue, 0, 10);
            buffer.u8(byte.MinValue, 0, 10);
            buffer.u8(byte.MinValue, 0, 10);
            buffer.u8(byte.MinValue, 0, 10);
            buffer.u8(byte.MinValue, 0, 10);
            buffer.u8(byte.MinValue, 0, 10);
            buffer.u8(byte.MinValue, 0, 10);
            Assert.Equal(4, buffer.LengthWritten);
        }        

        [Fact]
        public void ByteHalf()
        {
            var buffer = new BitBufferWriter();
            buffer.u8(byte.MaxValue / 2);
            buffer.u8(byte.MaxValue / 2);
            buffer.u8(byte.MaxValue / 2);
            buffer.u8(byte.MaxValue / 2);
            buffer.u8(byte.MaxValue / 2);
            buffer.u8(byte.MaxValue / 2);
            buffer.u8(byte.MaxValue / 2);
            buffer.u8(byte.MaxValue / 2);
            Assert.Equal(8, buffer.LengthWritten);
        }
        

        [Fact]
        public void ByteShortMin4()
        {
            var buffer = new BitBufferWriter();
            buffer.i16(short.MinValue);
            buffer.i16(short.MinValue);
            buffer.i16(short.MinValue);
            buffer.i16(short.MinValue);
            Assert.Equal(12, buffer.LengthWritten);
        }

        [Fact]
        public void ByteShort0()
        {
            var buffer = new BitBufferWriter();
            buffer.i16(0);
            buffer.i16(0);
            buffer.i16(0);
            buffer.i16(0);
            Assert.Equal(4, buffer.LengthWritten);
        }

        [Fact]
        public void ShortMinMax4()
        {
            var buffer = new BitBufferWriter();
            buffer.i16(short.MaxValue, short.MinValue, short.MaxValue);
            buffer.i16(short.MaxValue, short.MinValue, short.MaxValue);
            buffer.i16(short.MaxValue, short.MinValue, short.MaxValue);
            buffer.i16(short.MaxValue, short.MinValue, short.MaxValue);
            Assert.Equal(8, buffer.LengthWritten);
        }

        [Fact]
        public void UIntMax()
        {
            var buffer = new BitBufferWriter();
            buffer.u32(uint.MaxValue);
            Assert.Equal(5, buffer.LengthWritten);
        }

        [Fact]
        public void UIntMin()
        {
            var writer = new BitBufferWriter();
            writer.u32(uint.MinValue);
            Assert.Equal(1, writer.LengthWritten);
        }

        [Fact]
        public void IntMaxValue4()
        {
            var writer = new BitBufferWriter();
            writer.i32(int.MaxValue);
            writer.i32(int.MaxValue);
            writer.i32(int.MaxValue);
            writer.i32(int.MaxValue);
            Assert.Equal(20, writer.LengthWritten);
        }

        [Fact]
        public void IntZeroMaxValue4()
        {
            var writer = new BitBufferWriter();
            writer.i32(int.MaxValue, 0, int.MaxValue);
            writer.i32(int.MaxValue, 0, int.MaxValue);
            writer.i32(int.MaxValue, 0, int.MaxValue);
            writer.i32(int.MaxValue, 0, int.MaxValue);
            Assert.Equal(16, writer.LengthWritten);
        }

        [Fact]
        public void ShortMax4Divided()
        {
            var writer = new BitBufferWriter();
            for (var i=0; i< 8;i++)
                writer.i16(short.MaxValue/3);
                                  
            Assert.Equal(24, writer.LengthWritten);
        }

        [Fact]
        public void IntOfShortMax4Divided()
        {
            var writer = new BitBufferWriter();
            for (var i=0; i< 8;i++)
                writer.i32(short.MaxValue/3);
            Assert.Equal(24, writer.LengthWritten);
        }

        [Fact]
        public void ShortZeroMax4()
        {
            var writer = new BitBufferWriter();
            writer.i16(short.MaxValue, 0, short.MaxValue);
            writer.i16(short.MaxValue, 0, short.MaxValue);
            writer.i16(short.MaxValue, 0, short.MaxValue);
            writer.i16(short.MaxValue, 0, short.MaxValue);
            Assert.Equal(8, writer.LengthWritten);
        }

        [Fact]
        public void SByte0()
        {
            var writer = new BitBufferWriter();
            writer.i8(0);
            writer.i8(0);
            writer.i8(0);
            writer.i8(0);
            Assert.Equal(4, writer.LengthWritten);
        }

        [Fact]
        public void SbyteMax4()
        {
            var writer = new BitBufferWriter();
            writer.i8(sbyte.MaxValue);
            writer.i8(sbyte.MaxValue);
            writer.i8(sbyte.MaxValue);
            writer.i8(sbyte.MaxValue);
            writer.i8(sbyte.MaxValue);
            writer.i8(sbyte.MaxValue);
            writer.i8(sbyte.MaxValue);
            writer.i8(sbyte.MaxValue);
            Assert.Equal(8, writer.LengthWritten);
        }

        [Fact]
        public void SByteZeroMax4()
        {
            var writer = new BitBufferWriter();
            writer.i8(sbyte.MaxValue, 0, sbyte.MaxValue);
            writer.i8(sbyte.MaxValue, 0, sbyte.MaxValue);
            writer.i8(sbyte.MaxValue, 0, sbyte.MaxValue);
            writer.i8(sbyte.MaxValue, 0, sbyte.MaxValue);
            writer.i8(sbyte.MaxValue, 0, sbyte.MaxValue);
            writer.i8(sbyte.MaxValue, 0, sbyte.MaxValue);
            writer.i8(sbyte.MaxValue, 0, sbyte.MaxValue);
            writer.i8(sbyte.MaxValue, 0, sbyte.MaxValue);
            Assert.Equal(7, writer.LengthWritten);
        }
    }
}
