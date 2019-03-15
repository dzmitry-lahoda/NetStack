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
            //buffer.Finish();
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
            Assert.Equal(3, buffer.Length);
        }

        [Fact]
        public void ByteMin3()
        {
            var buffer = new BitBuffer();
            buffer.AddByte(byte.MinValue);
            buffer.AddByte(byte.MinValue);
            buffer.AddByte(byte.MinValue);
            Assert.Equal(3, buffer.Length);
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
            Assert.Equal(8, buffer.Length);
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
            //buffer.Finish();         
            Assert.Equal(8, buffer.Length);
        }   


        [Fact]
        public void ByteShortMin4()
        {
            var buffer = new BitBuffer();
            buffer.AddShort(short.MinValue);
            buffer.AddShort(short.MinValue);
            buffer.AddShort(short.MinValue);
            buffer.AddShort(short.MinValue);    
            //buffer.Finish();                  
            Assert.Equal(12, buffer.Length);
        }                     

        [Fact]
        public void UIntMax()
        {
            var buffer = new BitBuffer();
            buffer.AddUInt(uint.MaxValue);
            Assert.Equal(5, buffer.Length);
        }

        [Fact]
        public void UIntMin()
        {
            var buffer = new BitBuffer();
            buffer.AddUInt(uint.MinValue);
            Assert.Equal(1, buffer.Length);
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
