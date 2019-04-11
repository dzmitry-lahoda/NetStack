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
            var buffer = new BitBufferWrite();
            buffer.AddBool(true);
            buffer.AddBool(true);
            buffer.AddBool(true);
            buffer.AddBool(true);
            buffer.AddBool(true);
            buffer.AddBool(true);
            buffer.AddBool(true);
            Assert.Equal(1, buffer.LengthWritten);
        }

        [Fact]
        public void BoolIsBit9()
        {
            var buffer = new BitBufferWrite();
            buffer.AddBool(true);
            buffer.AddBool(true);
            buffer.AddBool(true);
            buffer.AddBool(true);
            buffer.AddBool(true);
            buffer.AddBool(true);
            buffer.AddBool(true);
            buffer.AddBool(true);

            buffer.AddBool(true);
            Assert.Equal(2, buffer.LengthWritten);
        }

        [Fact]
        public void ByteMax3()
        {
            var buffer = new BitBufferWrite();
            buffer.u8(byte.MaxValue);
            buffer.u8(byte.MaxValue);
            buffer.u8(byte.MaxValue);
            Assert.Equal(3, buffer.LengthWritten);
        }

        [Fact]
        public void ByteMin3()
        {
            var buffer = new BitBufferWrite();
            buffer.u8(byte.MinValue);
            buffer.u8(byte.MinValue);
            buffer.u8(byte.MinValue);
            Assert.Equal(3, buffer.LengthWritten);
        }

        [Fact]
        public void ByteMin8()
        {
            var buffer = new BitBufferWrite();
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
            var buffer = new BitBufferWrite();
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
            var buffer = new BitBufferWrite();
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
            var buffer = new BitBufferWrite();
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
            var buffer = new BitBufferWrite();
            buffer.i16(short.MinValue);
            buffer.i16(short.MinValue);
            buffer.i16(short.MinValue);
            buffer.i16(short.MinValue);
            Assert.Equal(12, buffer.LengthWritten);
        }

        [Fact]
        public void ByteShort0()
        {
            var buffer = new BitBufferWrite();
            buffer.i16(0);
            buffer.i16(0);
            buffer.i16(0);
            buffer.i16(0);
            Assert.Equal(4, buffer.LengthWritten);
        }

        [Fact]
        public void ShortMinMax4()
        {
            var buffer = new BitBufferWrite();
            buffer.i16(short.MaxValue, short.MinValue, short.MaxValue);
            buffer.i16(short.MaxValue, short.MinValue, short.MaxValue);
            buffer.i16(short.MaxValue, short.MinValue, short.MaxValue);
            buffer.i16(short.MaxValue, short.MinValue, short.MaxValue);
            Assert.Equal(8, buffer.LengthWritten);
        }

        [Fact]
        public void UIntMax()
        {
            var buffer = new BitBufferWrite();
            buffer.u32(uint.MaxValue);
            Assert.Equal(5, buffer.LengthWritten);
        }

        [Fact]
        public void UIntMin()
        {
            var buffer = new BitBufferWrite();
            buffer.u32(uint.MinValue);
            Assert.Equal(1, buffer.LengthWritten);
        }

        [Fact]
        public void IntMaxValue4()
        {
            var buffer = new BitBufferWrite();
            buffer.AddInt(int.MaxValue);
            buffer.AddInt(int.MaxValue);
            buffer.AddInt(int.MaxValue);
            buffer.AddInt(int.MaxValue);
            Assert.Equal(20, buffer.LengthWritten);
        }

        [Fact]
        public void IntZeroMaxValue4()
        {
            var buffer = new BitBufferWrite();
            buffer.i32(int.MaxValue, 0, int.MaxValue);
            buffer.i32(int.MaxValue, 0, int.MaxValue);
            buffer.i32(int.MaxValue, 0, int.MaxValue);
            buffer.i32(int.MaxValue, 0, int.MaxValue);
            Assert.Equal(16, buffer.LengthWritten);
        }

        [Fact]
        public void ShortMax4Divided()
        {
            var buffer = new BitBufferWrite();
            for (var i=0; i< 8;i++)
                buffer.i16(short.MaxValue/3);
                                  
            Assert.Equal(24, buffer.LengthWritten);
        }

        [Fact]
        public void IntOfShortMax4Divided()
        {
            var buffer = new BitBufferWrite();
            for (var i=0; i< 8;i++)
                buffer.AddInt(short.MaxValue/3);
            Assert.Equal(24, buffer.LengthWritten);
        }

        [Fact]
        public void ShortZeroMax4()
        {
            var buffer = new BitBufferWrite();
            buffer.i16(short.MaxValue, 0, short.MaxValue);
            buffer.i16(short.MaxValue, 0, short.MaxValue);
            buffer.i16(short.MaxValue, 0, short.MaxValue);
            buffer.i16(short.MaxValue, 0, short.MaxValue);
            Assert.Equal(8, buffer.LengthWritten);
        }

        [Fact]
        public void SByte0()
        {
            var buffer = new BitBufferWrite();
            buffer.i8(0);
            buffer.i8(0);
            buffer.i8(0);
            buffer.i8(0);
            Assert.Equal(4, buffer.LengthWritten);
        }

        [Fact]
        public void SbyteMax4()
        {
            var buffer = new BitBufferWrite();
            buffer.i8(sbyte.MaxValue);
            buffer.i8(sbyte.MaxValue);
            buffer.i8(sbyte.MaxValue);
            buffer.i8(sbyte.MaxValue);
            buffer.i8(sbyte.MaxValue);
            buffer.i8(sbyte.MaxValue);
            buffer.i8(sbyte.MaxValue);
            buffer.i8(sbyte.MaxValue);
            Assert.Equal(8, buffer.LengthWritten);
        }

        [Fact]
        public void SByteZeroMax4()
        {
            var buffer = new BitBufferWrite();
            buffer.i8(sbyte.MaxValue, 0, sbyte.MaxValue);
            buffer.i8(sbyte.MaxValue, 0, sbyte.MaxValue);
            buffer.i8(sbyte.MaxValue, 0, sbyte.MaxValue);
            buffer.i8(sbyte.MaxValue, 0, sbyte.MaxValue);
            buffer.i8(sbyte.MaxValue, 0, sbyte.MaxValue);
            buffer.i8(sbyte.MaxValue, 0, sbyte.MaxValue);
            buffer.i8(sbyte.MaxValue, 0, sbyte.MaxValue);
            buffer.i8(sbyte.MaxValue, 0, sbyte.MaxValue);
            Assert.Equal(7, buffer.LengthWritten);
        }
    }
}
