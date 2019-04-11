

using System;
using System.Numerics;
using Xunit;

namespace NetStack.Serialization
{
    partial class BitBufferTests
    {
        [Fact]
        public void ByteWriteRead()
        {
            var writer = new BitBufferWrite();
            writer.u8(byte.MaxValue);
            writer.Finish();
            var allocated = new byte[ushort.MaxValue];
            writer.ToArray(allocated);
            var reader = new BitBufferReader(allocated.Length);
            reader.FromArray(allocated);
            Assert.Equal(byte.MaxValue, reader.u8());
        }

        [Fact]
        public void ByteMinMaxRequired()
        {
            var buffer = new BitBufferWrite();
            buffer.u8(123, 0, 201);
            buffer.u8(1);
            buffer.u8(42, 1, 43);
            buffer.u8(1, 0, 10);
            buffer.u8(2, 3);
            buffer.u8(0);
            var bitsWritten = buffer.BitsWritten;
            var data = buffer.ToArray();
            var reader = new BitBufferReader();
            reader.FromArray(data);
            Assert.Equal(123, reader.u8(0, 201));
            Assert.Equal(1, reader.u8());
            Assert.Equal(42, reader.u8(1, 43));
            Assert.Equal(1, reader.u8(0, 10));
            Assert.Equal(2, reader.u8(3));
            Assert.Equal(0, reader.u8());
            Assert.Equal(bitsWritten, reader.BitsRead);
        }     

        [Fact]
        public void ByteMaxValueWritePeek()
        {
            var writer = new BitBufferWrite();
            writer.u8(byte.MaxValue);
            var data = writer.ToArray();
            var reader = new BitBufferReader();
            reader.FromArray(data);
            Assert.Equal(byte.MaxValue, reader.u8Peek());
        }
        
        public void ByteMaxValueWritePeek1024()
        {
            var writer = new BitBufferWrite();
            writer.u8(byte.MaxValue);
            var data = writer.ToArray();
            var reader = new BitBufferReader();
            reader.FromArray(data);
            for (int i = 0; i < 1024; i++)
            {
                Assert.Equal(byte.MaxValue, reader.u8Peek());    
            }
        }

        [Fact]
        public void ByteWritePeekRead256()
        {
            var writer = new BitBufferWrite(1000);
            for (int i = 0; i < 513; i++)
            {
                //buffer.AddByte(i % 2 == 0 ? byte.MaxValue : (byte)0);
                writer.u8(byte.MaxValue) ;
            }

            var data = writer.ToArray();
            var reader = new BitBufferReader();
            reader.FromArray(data);
            for (int i = 0; i < 513; i++)
            {
                    //Assert.Equal(i % 2 == 0 ? byte.MaxValue : (byte)0, reader.PeekByte());
                    //Assert.Equal(i % 2 == 0 ? byte.MaxValue : (byte)0, reader.ReadByte());
                    var peek = reader.u8Peek();
                    //Assert.True(byte.MaxValue == peek, $"Peek {peek} at {i} is wrong");
                    var read =  reader.u8();
                    //Assert.True(byte.MaxValue == read, $"Read {read} at {i} is wrong");
            }
        }
    }
}