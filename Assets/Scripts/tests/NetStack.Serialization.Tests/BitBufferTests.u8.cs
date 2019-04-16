

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
        public void u8WriteRead()
        {
            var writer = new BitBufferWriter<SevenBitEncoding>();
            writer.u8(u8.MaxValue);
            writer.Align();
            var allocated = new u8[ushort.MaxValue];
            writer.ToSpan(allocated);
            var reader = new BitBufferReader<SevenBitDecoding>(allocated.Length);
            reader.CopyFrom(allocated);
            Assert.AreEqual(u8.MaxValue, reader.u8());
        }

        [Test]
        public void u8MinMaxRequired()
        {
            var buffer = new BitBufferWriter<SevenBitEncoding>();
            buffer.u8(123, 0, 201);
            buffer.u8(1);
            buffer.u8(42, 1, 43);
            buffer.u8(1, 0, 10);
            buffer.u8(2, 3);
            buffer.u8(0);
            var bitsWritten = buffer.BitsWritten;
            var data = buffer.ToArray();
            var reader = new BitBufferReader<SevenBitDecoding>();
            reader.CopyFrom(data);
            Assert.AreEqual(123, reader.u8(0, 201));
            Assert.AreEqual(1, reader.u8());
            Assert.AreEqual(42, reader.u8(1, 43));
            Assert.AreEqual(1, reader.u8(0, 10));
            Assert.AreEqual(2, reader.u8(3));
            Assert.AreEqual(0, reader.u8());
            Assert.AreEqual(bitsWritten, reader.BitsRead);
        }

        [Test]
        public void u8MaxValueWritePeek()
        {
            var writer = new BitBufferWriter<SevenBitEncoding>();
            writer.u8(u8.MaxValue);
            var data = writer.ToArray();
            var reader = new BitBufferReader<SevenBitDecoding>();
            reader.CopyFrom(data);
            Assert.AreEqual(u8.MaxValue, reader.u8Peek());
            Assert.AreEqual(reader.u8Peek(), reader.u8());
        }

        public void u8MaxValueWritePeek1024()
        {
            var writer = new BitBufferWriter<SevenBitEncoding>();
            writer.u8(u8.MaxValue);
            var data = writer.ToArray();
            var reader = new BitBufferReader<SevenBitDecoding>();
            reader.CopyFrom(data);
            for (int i = 0; i < 1024; i++)
            {
                Assert.AreEqual(u8.MaxValue, reader.u8Peek());
            }
        }

        [Test]
        public void u8WritePeekRead256()
        {
            var writer = new BitBufferWriter<SevenBitEncoding>(1000);
            for (int i = 0; i < 513; i++)
            {
                var val = (u8)(u8.MaxValue / (i % 2 + 1));
                writer.u8(val);
            }

            var data = writer.ToArray();
            var reader = new BitBufferReader<SevenBitDecoding>();
            reader.CopyFrom(data);
            for (int i = 0; i < 513; i++)
            {
                var val = (u8)(u8.MaxValue / (i % 2 + 1));
                Assert.AreEqual(val, reader.u8Peek());
                Assert.AreEqual(val, reader.u8());
            }
        }

#if DEBUG || NETSTACK_VALIDATE
        [Test]
        public void u8WriteOutOfRange()
        {
            var writer = new BitBufferWriter<SevenBitEncoding>();
            Assert.Throws<ArgumentOutOfRangeException>(() => writer.u8(125, 0, 123));
            Assert.Throws<ArgumentOutOfRangeException>(() => writer.u8(1, 2, 123));
            Assert.Throws<ArgumentException>(() => writer.u8(123, 2));
            Assert.Throws<ArgumentException>(() => writer.u8(44, 55, 33));
        }

        [Test]
        public void u8ReadOutOfRange()
        {
            var reader = new BitBufferReader<SevenBitDecoding>();
            reader.CopyFrom(new u8[666]);
            Assert.Throws<ArgumentException>(() => reader.u8(255, 123));
            Assert.Throws<ArgumentOutOfRangeException>(() => reader.u8(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => reader.u8(33));
        }

        [Test]
        public void u8VerySmallReader()
        {
            var smallReader = new BitBufferReader<SevenBitDecoding>(1);
            smallReader.CopyFrom(new u8[4]);
            smallReader.u8(u8.MinValue, u8.MaxValue);
            Assert.True(smallReader.BitsRead > 0);
            smallReader.u8(u8.MinValue, u8.MaxValue);
            smallReader.u8(u8.MinValue, u8.MaxValue);
            smallReader.u8(u8.MinValue, u8.MaxValue);
            smallReader.u8(u8.MinValue, u8.MaxValue);
            smallReader.u8(u8.MinValue, u8.MaxValue);
            smallReader.u8(u8.MinValue, u8.MaxValue);
            smallReader.u8(u8.MinValue, u8.MaxValue);
            Assert.Throws<ArgumentOutOfRangeException>(() => smallReader.u8(u8.MinValue, u8.MaxValue));
        }
#endif          
    }
}