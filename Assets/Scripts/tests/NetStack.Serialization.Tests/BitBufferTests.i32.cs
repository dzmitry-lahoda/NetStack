using System;
using System.Numerics;
using NUnit.Framework;
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
        [Test]
        public void IntReadWrite()
        {
            var writer = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            writer.i32(int.MinValue);
            writer.i32(0);
            writer.i32(int.MaxValue);
            writer.Align();
            var allocated = new byte[ushort.MaxValue];
            writer.ToSpan(allocated);
            var reader = new BitBufferReader<SevenBitDecoding<u32ArrayMemory>>(allocated.Length);
            reader.CopyFrom(allocated);
            Assert.AreEqual(int.MinValue, reader.i32());
            Assert.AreEqual(0, reader.i32());
            Assert.AreEqual(int.MaxValue, reader.i32());
        }

        [Test]
        public void IntMinMaxRequired()
        {
            var writer = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            writer.i32(12345, 0, 123456);
            writer.i32(1);
            writer.i32(42, -1, 43);
            writer.i32(1, 0, 10);
            writer.i32(2, 3);
            writer.i32(0);
            var bitsWritten = writer.BitsWritten;
            var data = writer.ToArray();
            var reader = new BitBufferReader<SevenBitDecoding<u32ArrayMemory>>();
            reader.CopyFrom(data);
            Assert.AreEqual(12345, reader.i32(0, 123456));
            Assert.AreEqual(1, reader.i32());
            Assert.AreEqual(42, reader.i32Peek(-1, 43));
            Assert.AreEqual(42, reader.i32(-1, 43));
            Assert.AreEqual(1, reader.i32(0, 10));
            Assert.AreEqual(2, reader.i32(3));
            Assert.AreEqual(0, reader.i32());
            Assert.AreEqual(bitsWritten, reader.BitsRead);
        }
#if !NO_EXCEPTIONS
        [Test]
        public void i32WriteOutOfRange()
        {
            var writer = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            Assert.Throws<ArgumentOutOfRangeException>(()=> writer.i32(12345, 0, 123));
            Assert.Throws<ArgumentOutOfRangeException>(()=> writer.i32(-12345, 0, 123));
            Assert.Throws<ArgumentException>(()=> writer.i32(-12345, 2));
            Assert.Throws<ArgumentException>(()=> writer.i32(444, 666, 123));
        }    

        [Test]
        public void i32ReadOutOfRange()
        {
            var reader = new BitBufferReader<SevenBitDecoding<u32ArrayMemory>>();
            reader.CopyFrom(new u8[666]);
            Assert.Throws<ArgumentException>(()=> reader.i32(666, 123));
            Assert.Throws<ArgumentOutOfRangeException>(()=> reader.i32(-1));
            Assert.Throws<ArgumentOutOfRangeException>(()=> reader.i32(33));
        }               

        [Test]
        public void int32VerySmallReader()
        {
            var smallReader = new BitBufferReader<SevenBitDecoding<u32ArrayMemory>>(1);
            smallReader.CopyFrom(new u8[4]);
            smallReader.i32(i32.MinValue, i32.MaxValue);
            Assert.True(smallReader.BitsRead > 0);
            smallReader.i32(i32.MinValue, i32.MaxValue);
            Assert.Throws<ArgumentOutOfRangeException>(()=> smallReader.i32(i32.MinValue, i32.MaxValue));
        }
#endif        
    }
}
