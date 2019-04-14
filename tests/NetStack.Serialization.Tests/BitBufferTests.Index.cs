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
            var writer = new BitBufferWriter<SevenBitEncoding>(100);
            Assert.Equal(0, writer.LengthWritten);
            var received = new byte[2000];
            var reader = new BitBufferReader<SevenBitDecoding>();
            reader.CopyFrom(received);
            Assert.Equal(0, writer.LengthWritten);
        }

        [Fact]
        public void ResetSet()
        {
            var writer = new BitBufferWriter<SevenBitEncoding>(8);
        
            writer.i32(666);
            var bitsWritten = writer.BitsWritten;
            var bitsAvailable = writer.BitsAvailable;
            writer.Reset();
            Assert.Equal(0, writer.BitsWritten);
            
            writer.i32(666);
            Assert.Equal(bitsWritten, writer.BitsWritten);
            Assert.Equal(bitsAvailable, writer.BitsAvailable);
            writer.i32(-273);
            writer.b(true);
            writer.i64(1234567890);
            var reader = new BitBufferReader<SevenBitDecoding>();
            reader.CopyFrom(writer.ToArray());
            Assert.Equal(666, reader.i32());
            var bitsRead = reader.BitsRead;
            reader.i32();
            reader.SetPosition(bitsRead);
            Assert.Equal(-273, reader.i32());
            var bitPosition = reader.BitsRead;
            reader.b();
            reader.SetPosition(bitPosition);
            Assert.True(reader.b());            
            
        }        

        [Fact]
        public void BitsRead()
        {
            var writer = new BitBufferWriter<SevenBitEncoding>();

            Assert.Equal(0, writer.BitsWritten);
            writer.b(true);
            Assert.Equal(1, writer.BitsWritten);
            writer.u8(123);
            Assert.Equal(9, writer.BitsWritten);
            writer.i16(12345);
            Assert.Equal(33, writer.BitsWritten);
            writer.i32(1234567890);
            Assert.Equal(73, writer.BitsWritten);
            var data = writer.ToArray();
            var reader = new BitBufferReader<SevenBitDecoding>();
            reader.CopyFrom(data);

            Assert.Equal(0, reader.BitsRead);
            reader.b();
            Assert.Equal(1, reader.BitsRead);
            reader.u8();
            Assert.Equal(9, reader.BitsRead);
            reader.i16();
            Assert.Equal(33, reader.BitsRead);
            reader.i32();
            Assert.Equal(73, reader.BitsRead);
        }
    }
}
