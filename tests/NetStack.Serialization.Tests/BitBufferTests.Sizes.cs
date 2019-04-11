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
            var buffer = new BitBufferWrite(100);
            Assert.Equal(0, buffer.LengthWritten);
            var received = new byte[2000];
            var reader = new BitBufferRead();
            reader.FromArray(received);
            Assert.Equal(0, buffer.LengthWritten);
        }

       [Fact]
        public void BitsRead()
        {
            var write = new BitBufferWrite();
            
            Assert.Equal(0, write.BitsWritten);
            write.AddBool(true);
            Assert.Equal(1, write.BitsWritten);
            write.u8(123);
            Assert.Equal(9, write.BitsWritten);
            write.i16(12345);
            Assert.Equal(33, write.BitsWritten);
            write.i32(1234567890);
            Assert.Equal(73, write.BitsWritten);
            var data = write.ToArray();
            var reader = new BitBufferRead();
            reader.FromArray(data);
            
            Assert.Equal(0, reader.BitsRead);
            reader.@bool();
            Assert.Equal(1, reader.BitsRead);
            reader.u8();
            Assert.Equal(9, reader.BitsRead);
            reader.ReadShort();
            Assert.Equal(33, reader.BitsRead);
            reader.i32();
            Assert.Equal(73, reader.BitsRead);
        }        
    }
}
