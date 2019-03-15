using System;
using Xunit;

namespace NetStack.Serialization
{
    public class BitBufferTests
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

            Assert.Equal(1, buffer.Length);
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
            Assert.Equal(2, buffer.Length);
        }

        [Fact]
        public void ByteMax3()
        {
            var buffer = new BitBuffer();
            buffer.AddByte(byte.MaxValue);
            buffer.AddByte(byte.MaxValue);
            buffer.AddByte(byte.MaxValue);
            Assert.Equal(4, buffer.Length);
        }

        [Fact]
        public void ByteMin3()
        {
            var buffer = new BitBuffer();
            buffer.AddByte(byte.MinValue);
            buffer.AddByte(byte.MinValue);
            buffer.AddByte(byte.MinValue);
            Assert.Equal(4, buffer.Length);
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
            Assert.Equal(9, buffer.Length);
        }         

        [Fact]
        public void ByteHalf()
        {
            var buffer = new BitBuffer();
            buffer.AddByte(byte.MaxValue/2);
            buffer.AddByte(byte.MaxValue/2);
            buffer.AddByte(byte.MaxValue/2);
            buffer.AddByte(byte.MaxValue/2);
            buffer.AddByte(byte.MaxValue/2);
            buffer.AddByte(byte.MaxValue/2);
            buffer.AddByte(byte.MaxValue/2);
            buffer.AddByte(byte.MaxValue/2);                      
            Assert.Equal(9, buffer.Length);
        }   


        [Fact]
        public void ByteShortMin4()
        {
            var buffer = new BitBuffer();
            buffer.AddShort(short.MinValue);
            buffer.AddShort(short.MinValue);
            buffer.AddShort(short.MinValue);
            buffer.AddShort(short.MinValue);                      
            Assert.Equal(8, buffer.Length);
        }                     

        [Fact]
        public void UIntMax()
        {
            var buffer = new BitBuffer();
            buffer.AddUInt(uint.MaxValue);
            Assert.Equal(6, buffer.Length);
        }

        [Fact]
        public void UIntMin()
        {
            var buffer = new BitBuffer();
            buffer.AddUInt(uint.MinValue);
            Assert.Equal(2, buffer.Length);
        }

        [Fact]
        public void BoolReadWrite()
        {
            var buffer = new BitBuffer();
            buffer.AddBool(true);
            buffer.Finish();
            // buffer.ResetReadPos();
            // Assert.True(buffer.ReadBool());
        }
    }
}
