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
            var buffer = new BitBuffer();
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
            var buffer = new BitBuffer();
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
            var buffer = new BitBuffer();
            buffer.AddByte(byte.MaxValue);
            buffer.AddByte(byte.MaxValue);
            buffer.AddByte(byte.MaxValue);
            Assert.Equal(3, buffer.LengthWritten);
        }

        [Fact]
        public void ByteMin3()
        {
            var buffer = new BitBuffer();
            buffer.AddByte(byte.MinValue);
            buffer.AddByte(byte.MinValue);
            buffer.AddByte(byte.MinValue);
            Assert.Equal(3, buffer.LengthWritten);
        }

        [Fact]
        public void ByteMin8()
        {
            var buffer = new BitBuffer();
            buffer.AddByte(byte.MinValue);
            buffer.AddByte(byte.MinValue);
            buffer.AddByte(byte.MinValue);
            buffer.AddByte(byte.MinValue);
            buffer.AddByte(byte.MinValue);
            buffer.AddByte(byte.MinValue);
            buffer.AddByte(byte.MinValue);
            buffer.AddByte(byte.MinValue);
            Assert.Equal(8, buffer.LengthWritten);
        }

        [Fact]
        public void ByteRangeMin8()
        {
            var buffer = new BitBuffer();
            buffer.AddByte(byte.MinValue, 0, (byte)sbyte.MaxValue);
            buffer.AddByte(byte.MinValue, 0, (byte)sbyte.MaxValue);
            buffer.AddByte(byte.MinValue, 0, (byte)sbyte.MaxValue);
            buffer.AddByte(byte.MinValue, 0, (byte)sbyte.MaxValue);
            buffer.AddByte(byte.MinValue, 0, (byte)sbyte.MaxValue);
            buffer.AddByte(byte.MinValue, 0, (byte)sbyte.MaxValue);
            buffer.AddByte(byte.MinValue, 0, (byte)sbyte.MaxValue);
            buffer.AddByte(byte.MinValue, 0, (byte)sbyte.MaxValue);
            Assert.Equal(7, buffer.LengthWritten);
        }

        [Fact]
        public void ByteRangeMin8Max10()
        {
            var buffer = new BitBuffer();
            buffer.AddByte(byte.MinValue, 0, 10);
            buffer.AddByte(byte.MinValue, 0, 10);
            buffer.AddByte(byte.MinValue, 0, 10);
            buffer.AddByte(byte.MinValue, 0, 10);
            buffer.AddByte(byte.MinValue, 0, 10);
            buffer.AddByte(byte.MinValue, 0, 10);
            buffer.AddByte(byte.MinValue, 0, 10);
            buffer.AddByte(byte.MinValue, 0, 10);
            Assert.Equal(4, buffer.LengthWritten);
        }        

        [Fact]
        public void ByteHalf()
        {
            var buffer = new BitBuffer();
            buffer.AddByte(byte.MaxValue / 2);
            buffer.AddByte(byte.MaxValue / 2);
            buffer.AddByte(byte.MaxValue / 2);
            buffer.AddByte(byte.MaxValue / 2);
            buffer.AddByte(byte.MaxValue / 2);
            buffer.AddByte(byte.MaxValue / 2);
            buffer.AddByte(byte.MaxValue / 2);
            buffer.AddByte(byte.MaxValue / 2);
            Assert.Equal(8, buffer.LengthWritten);
        }
        

        [Fact]
        public void ByteShortMin4()
        {
            var buffer = new BitBuffer();
            buffer.AddShort(short.MinValue);
            buffer.AddShort(short.MinValue);
            buffer.AddShort(short.MinValue);
            buffer.AddShort(short.MinValue);
            Assert.Equal(12, buffer.LengthWritten);
        }

        [Fact]
        public void ByteShort0()
        {
            var buffer = new BitBuffer();
            buffer.AddShort(0);
            buffer.AddShort(0);
            buffer.AddShort(0);
            buffer.AddShort(0);
            Assert.Equal(4, buffer.LengthWritten);
        }

        [Fact]
        public void ShortMinMax4()
        {
            var buffer = new BitBuffer();
            buffer.AddShort(short.MaxValue, short.MinValue, short.MaxValue);
            buffer.AddShort(short.MaxValue, short.MinValue, short.MaxValue);
            buffer.AddShort(short.MaxValue, short.MinValue, short.MaxValue);
            buffer.AddShort(short.MaxValue, short.MinValue, short.MaxValue);
            Assert.Equal(8, buffer.LengthWritten);
        }

        [Fact]
        public void UIntMax()
        {
            var buffer = new BitBuffer();
            buffer.AddUInt(uint.MaxValue);
            Assert.Equal(5, buffer.LengthWritten);
        }

        [Fact]
        public void UIntMin()
        {
            var buffer = new BitBuffer();
            buffer.AddUInt(uint.MinValue);
            Assert.Equal(1, buffer.LengthWritten);
        }

        [Fact]
        public void IntMaxValue4()
        {
            var buffer = new BitBuffer();
            buffer.AddInt(int.MaxValue);
            buffer.AddInt(int.MaxValue);
            buffer.AddInt(int.MaxValue);
            buffer.AddInt(int.MaxValue);
            Assert.Equal(20, buffer.LengthWritten);
        }

        [Fact]
        public void IntZeroMaxValue4()
        {
            var buffer = new BitBuffer();
            buffer.AddInt(int.MaxValue, 0, int.MaxValue);
            buffer.AddInt(int.MaxValue, 0, int.MaxValue);
            buffer.AddInt(int.MaxValue, 0, int.MaxValue);
            buffer.AddInt(int.MaxValue, 0, int.MaxValue);
            Assert.Equal(16, buffer.LengthWritten);
        }

        [Fact]
        public void ShortMax4Divided()
        {
            var buffer = new BitBuffer();
            for (var i=0; i< 8;i++)
                buffer.AddShort(short.MaxValue/3);
                                  
            Assert.Equal(24, buffer.LengthWritten);
        }

        [Fact]
        public void IntOfShortMax4Divided()
        {
            var buffer = new BitBuffer();
            for (var i=0; i< 8;i++)
                buffer.AddInt(short.MaxValue/3);
            Assert.Equal(24, buffer.LengthWritten);
        }

        [Fact]
        public void ShortZeroMax4()
        {
            var buffer = new BitBuffer();
            buffer.AddShort(short.MaxValue, 0, short.MaxValue);
            buffer.AddShort(short.MaxValue, 0, short.MaxValue);
            buffer.AddShort(short.MaxValue, 0, short.MaxValue);
            buffer.AddShort(short.MaxValue, 0, short.MaxValue);
            Assert.Equal(8, buffer.LengthWritten);
        }

        [Fact]
        public void SByte0()
        {
            var buffer = new BitBuffer();
            buffer.AddSByte(0);
            buffer.AddSByte(0);
            buffer.AddSByte(0);
            buffer.AddSByte(0);
            Assert.Equal(4, buffer.LengthWritten);
        }

        [Fact]
        public void SbyteMax4()
        {
            var buffer = new BitBuffer();
            buffer.AddSByte(sbyte.MaxValue);
            buffer.AddSByte(sbyte.MaxValue);
            buffer.AddSByte(sbyte.MaxValue);
            buffer.AddSByte(sbyte.MaxValue);
            buffer.AddSByte(sbyte.MaxValue);
            buffer.AddSByte(sbyte.MaxValue);
            buffer.AddSByte(sbyte.MaxValue);
            buffer.AddSByte(sbyte.MaxValue);
            Assert.Equal(8, buffer.LengthWritten);
        }

        [Fact]
        public void SByteZeroMax4()
        {
            var buffer = new BitBuffer();
            buffer.AddSByte(sbyte.MaxValue, 0, sbyte.MaxValue);
            buffer.AddSByte(sbyte.MaxValue, 0, sbyte.MaxValue);
            buffer.AddSByte(sbyte.MaxValue, 0, sbyte.MaxValue);
            buffer.AddSByte(sbyte.MaxValue, 0, sbyte.MaxValue);
            buffer.AddSByte(sbyte.MaxValue, 0, sbyte.MaxValue);
            buffer.AddSByte(sbyte.MaxValue, 0, sbyte.MaxValue);
            buffer.AddSByte(sbyte.MaxValue, 0, sbyte.MaxValue);
            buffer.AddSByte(sbyte.MaxValue, 0, sbyte.MaxValue);
            Assert.Equal(7, buffer.LengthWritten);
        }
    }
}
