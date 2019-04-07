

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
            var buffer = new BitBuffer();
            buffer.AddByte(byte.MaxValue);
            buffer.Finish();
            var allocated = new byte[ushort.MaxValue];
            buffer.ToArray(allocated);
            var reader = new BitBuffer(allocated.Length);
            reader.FromArray(allocated);
            Assert.Equal(byte.MaxValue, reader.ReadByte());
        }

        [Fact]
        public void ByteMaxValueWritePeek()
        {
            var buffer = new BitBuffer();
            buffer.AddByte(byte.MaxValue);
            var data = buffer.ToArray();
            var reader = new BitBuffer();
            reader.FromArray(data);
            Assert.Equal(byte.MaxValue, reader.PeekByte());
        }
        
        public void ByteMaxValueWritePeek1024()
        {
            var buffer = new BitBuffer();
            buffer.AddByte(byte.MaxValue);
            var data = buffer.ToArray();
            var reader = new BitBuffer();
            reader.FromArray(data);
            for (int i = 0; i < 1024; i++)
            {
                Assert.Equal(byte.MaxValue, reader.PeekByte());    
            }
        }

        [Fact]
        public void ByteWritePeekRead256()
        {
            var buffer = new BitBuffer(1000);
            for (int i = 0; i < 513; i++)
            {
                //buffer.AddByte(i % 2 == 0 ? byte.MaxValue : (byte)0);
                buffer.AddByte(byte.MaxValue) ;
            }

            var data = buffer.ToArray();
            var reader = new BitBuffer();
            reader.FromArray(data);
            for (int i = 0; i < 513; i++)
            {
                    //Assert.Equal(i % 2 == 0 ? byte.MaxValue : (byte)0, reader.PeekByte());
                    //Assert.Equal(i % 2 == 0 ? byte.MaxValue : (byte)0, reader.ReadByte());
                    var peek = reader.PeekByte();
                    //Assert.True(byte.MaxValue == peek, $"Peek {peek} at {i} is wrong");
                    var read =  reader.ReadByte();
                    //Assert.True(byte.MaxValue == read, $"Read {read} at {i} is wrong");
            }
        }
    }
}