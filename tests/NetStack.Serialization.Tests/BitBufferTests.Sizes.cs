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
            var buffer = new BitBuffer(100);
            Assert.Equal(0, buffer.Length);
            var received = new byte[2000];
            buffer.FromArray(received);
            Assert.Equal(0, buffer.Length);
        }

       [Fact]
        public void BitsRead()
        {
            var write = new BitBuffer();
            
            Assert.Equal(0, write.BitsPassed2);
            write.AddBool(true);
            Assert.Equal(1, write.BitsPassed2);
            write.AddByte(123);
            Assert.Equal(9, write.BitsPassed2);
            write.AddShort(12345);
            Assert.Equal(33, write.BitsPassed2);
            write.AddInt(1234567890);
            Assert.Equal(73, write.BitsPassed2);
            var data = write.ToArray();
            var reader = new BitBuffer();
            reader.FromArray(data);
            
            Assert.Equal(0, reader.BitsPassed);
            reader.ReadBool();
            Assert.Equal(1, reader.BitsPassed);
            reader.ReadByte();
            Assert.Equal(9, reader.BitsPassed);
            reader.ReadShort();
            Assert.Equal(33, reader.BitsPassed);
            reader.ReadInt();
            Assert.Equal(73, reader.BitsPassed);
        }        
    }
}
