using System;
using System.Numerics;
using NUnit.Framework;

namespace NetStack.Serialization
{
    public partial class BitBufferTests
    {
        [Test]
        public void Capacity()
        {
            var writer = new BitBufferWriter<SevenBitEncoding>(100);
            Assert.AreEqual(0, writer.LengthWritten);
            var received = new byte[2000];
            var reader = new BitBufferReader<SevenBitDecoding>();
            reader.CopyFrom(received);
            Assert.AreEqual(0, writer.LengthWritten);
        }

        [Test]
        public void ResetSet()
        {
            var writer = new BitBufferWriter<SevenBitEncoding>(8);
        
            writer.i32(666);
            var bitsWritten = writer.BitsWritten;
            var bitsAvailable = writer.BitsAvailable;
            writer.Reset();
            Assert.AreEqual(0, writer.BitsWritten);
            
            writer.i32(666);
            Assert.AreEqual(bitsWritten, writer.BitsWritten);
            Assert.AreEqual(bitsAvailable, writer.BitsAvailable);
            writer.i32(-273);
            writer.b(true);
            writer.i64(1234567890);
            var reader = new BitBufferReader<SevenBitDecoding>();
            reader.CopyFrom(writer.ToArray());
            Assert.AreEqual(666, reader.i32());
            var bitsRead = reader.BitsRead;
            reader.i32();
            reader.SetPosition(bitsRead);
            Assert.AreEqual(-273, reader.i32());
            var bitPosition = reader.BitsRead;
            reader.b();
            reader.SetPosition(bitPosition);
            Assert.True(reader.b());            
            
        }        

        [Test]
        public void BitsRead()
        {
            var writer = new BitBufferWriter<SevenBitEncoding>();

            Assert.AreEqual(0, writer.BitsWritten);
            writer.b(true);
            Assert.AreEqual(1, writer.BitsWritten);
            writer.u8(123);
            Assert.AreEqual(9, writer.BitsWritten);
            writer.i16(12345);
            Assert.AreEqual(33, writer.BitsWritten);
            writer.i32(1234567890);
            Assert.AreEqual(73, writer.BitsWritten);
            var data = writer.ToArray();
            var reader = new BitBufferReader<SevenBitDecoding>();
            reader.CopyFrom(data);

            Assert.AreEqual(0, reader.BitsRead);
            reader.b();
            Assert.AreEqual(1, reader.BitsRead);
            reader.u8();
            Assert.AreEqual(9, reader.BitsRead);
            reader.i16();
            Assert.AreEqual(33, reader.BitsRead);
            reader.i32();
            Assert.AreEqual(73, reader.BitsRead);
        }
    }
}
