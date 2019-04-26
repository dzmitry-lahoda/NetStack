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
        public void i8ReadWriteRaw() => i8ReadWrite<RawEncoding<u32ArrayMemory>, RawDecoding>();

        [Test]
        public void i8ReadWriteEncoded() => i8ReadWrite<SevenBitEncoding<u32ArrayMemory>, SevenBitDecoding>();

        private void i8ReadWrite<TEncoder, TDecoder>() 
             where TEncoder:struct, ICompression<BitBufferWriterBase<u32ArrayMemory>> 
             where TDecoder:unmanaged, IDecompression<BitBufferReader<TDecoder>> 
        {
            var writer = new BitBufferWriter<TEncoder>();
            writer.i8(i8.MinValue);
            i8 half = i8.MaxValue / 2;
            writer.i8(half);
            writer.i8(i8.MaxValue);
            var data = writer.ToArray();
            var reader = new BitBufferReader<TDecoder>();
            reader.CopyFrom(data);
            Assert.AreEqual(i8.MinValue, reader.i8());
            Assert.AreEqual(half, reader.i8Peek());
            Assert.AreEqual(half, reader.i8());
            Assert.AreEqual(i8.MaxValue, reader.i8());
        }

        [Test]
        public void i8ReadWriteLimits()
        {
            var writer = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            writer.i8(13, 0, 14);
            writer.i8(2, 4);
            var bitsWritten = writer.BitsWritten;
            var data = writer.ToArray();
            var reader = new BitBufferReader<SevenBitDecoding>();
            reader.CopyFrom(data);
            Assert.AreEqual(13, reader.i8(0, 14));
            Assert.AreEqual(2, reader.i8(4));
            Assert.AreEqual(bitsWritten, reader.BitsRead);
        }     

       [Test]
        public void i8ReadWriteSmallLimits()
        {
            var writer = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            writer.i32(500);
            writer.i16(1);
            writer.i32(0);
            writer.u8(0, 0, 4);
            writer.u8(1, 0, 4);
            writer.u8(2, 0, 4);
            writer.u8(3, 0, 4);
            writer.u8(4, 0, 4);
            writer.i32(0, 0, 4);
            writer.i32(1, 0, 4);
            writer.i32(2, 0, 4);
            writer.i32(3, 0, 4);
            writer.i32(4, 0, 4);            
            #if !NO_EXCEPTIONS
            
            Assert.Throws<System.ArgumentOutOfRangeException>(()=>writer.u8(5, 0, 4));            
            Assert.Throws<System.ArgumentOutOfRangeException>(()=>writer.u8(255, 0, 4));
            Assert.Throws<System.ArgumentOutOfRangeException>(()=>writer.i32(5, 0, 4));            
            Assert.Throws<System.ArgumentOutOfRangeException>(()=>writer.i32(255, 0, 4));
            #endif
        }            
   }
}
