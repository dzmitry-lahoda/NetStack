

using System;
using System.Numerics;
using Xunit;
using i8 = System.SByte;
using i16 = System.Int16;
using i32 = System.Int32;
using i64 = System.Int64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;
using f32 = System.Single;
using f64 = System.Double;


namespace NetStack.Serialization
{
    partial class BitBufferTests
    {
        [Fact]
        public void u8WriteRead()
        {
            var writer = new BitBufferWriter<SevenBit>();
            writer.u8(u8.MaxValue);
            writer.Finish();
            var allocated = new u8[ushort.MaxValue];
            writer.ToArray(allocated);
            var reader = new BitBufferReader(allocated.Length);
            reader.FromArray(allocated);
            Assert.Equal(u8.MaxValue, reader.u8());
        }

        [Fact]
        public void u8MinMaxRequired()
        {
            var buffer = new BitBufferWriter<SevenBit>();
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
        public void u8MaxValueWritePeek()
        {
            var writer = new BitBufferWriter<SevenBit>();
            writer.u8(u8.MaxValue);
            var data = writer.ToArray();
            var reader = new BitBufferReader();
            reader.FromArray(data);
            Assert.Equal(u8.MaxValue, reader.u8Peek());
        }
        
        public void u8MaxValueWritePeek1024()
        {
            var writer = new BitBufferWriter<SevenBit>();
            writer.u8(u8.MaxValue);
            var data = writer.ToArray();
            var reader = new BitBufferReader();
            reader.FromArray(data);
            for (int i = 0; i < 1024; i++)
            {
                Assert.Equal(u8.MaxValue, reader.u8Peek());    
            }
        }

        [Fact]
        public void u8WritePeekRead256()
        {
            var writer = new BitBufferWriter<SevenBit>(1000);
            for (int i = 0; i < 513; i++)
            {
                //buffer.Addu8(i % 2 == 0 ? u8.MaxValue : (u8)0);
                writer.u8(u8.MaxValue) ;
            }

            var data = writer.ToArray();
            var reader = new BitBufferReader();
            reader.FromArray(data);
            for (int i = 0; i < 513; i++)
            {
                    //Assert.Equal(i % 2 == 0 ? u8.MaxValue : (u8)0, reader.Peeku8());
                    //Assert.Equal(i % 2 == 0 ? u8.MaxValue : (u8)0, reader.Readu8());
                    var peek = reader.u8Peek();
                    //Assert.True(u8.MaxValue == peek, $"Peek {peek} at {i} is wrong");
                    var read =  reader.u8();
                    //Assert.True(u8.MaxValue == read, $"Read {read} at {i} is wrong");
            }
        }

#if DEBUG || NETSTACK_VALIDATE
        [Fact]
        public void u8WriteOutOfRange()
        {
            var writer = new BitBufferWriter<SevenBit>();
            Assert.Throws<ArgumentOutOfRangeException>(()=> writer.u8(125, 0, 123));
            Assert.Throws<ArgumentOutOfRangeException>(()=> writer.u8(1, 2, 123));
            Assert.Throws<ArgumentException>(()=> writer.u8(123, 2));
            Assert.Throws<ArgumentException>(()=> writer.u8(44, 55, 33));
        }    

        [Fact]
        public void u8ReadOutOfRange()
        {
            var reader = new BitBufferReader();
            reader.FromArray(new u8[666]);
            Assert.Throws<ArgumentException>(()=> reader.u8(255, 123));
            Assert.Throws<ArgumentOutOfRangeException>(()=> reader.u8(-1));
            Assert.Throws<ArgumentOutOfRangeException>(()=> reader.u8(33));
        }               

        [Fact]
        public void u8VerySmallReader()
        {
            var smallReader = new BitBufferReader(1);
            smallReader.FromArray(new u8[4]);
            smallReader.u8(u8.MinValue, u8.MaxValue);
            Assert.True(smallReader.BitsRead > 0);
            smallReader.u8(u8.MinValue, u8.MaxValue);
            smallReader.u8(u8.MinValue, u8.MaxValue);
            smallReader.u8(u8.MinValue, u8.MaxValue);
            smallReader.u8(u8.MinValue, u8.MaxValue);
            smallReader.u8(u8.MinValue, u8.MaxValue);
            smallReader.u8(u8.MinValue, u8.MaxValue);
            smallReader.u8(u8.MinValue, u8.MaxValue);
            Assert.Throws<ArgumentOutOfRangeException>(()=> smallReader.u8(u8.MinValue, u8.MaxValue));
        }
#endif          
    }
}