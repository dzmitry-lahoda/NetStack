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
        public void u32ReadWriteRaw() => u32ReadWrite<RawEncoding<u32ArrayMemory>, RawDecoding<u32ArrayMemory>>();

        [Test]
        public void u32ReadWriteEncoded() => u32ReadWrite<SevenBitEncoding<u32ArrayMemory>, SevenBitDecoding<u32ArrayMemory>>();

        private void u32ReadWrite<TEncoder, TDecoder>() 
             where TEncoder:struct, ICompression<RawBitWriter<u32ArrayMemory>> 
             where TDecoder:struct, IDecompression<BitBufferReader<TDecoder>> 
        {
            var writer = new BitBufferWriter<TEncoder>();
            writer.u32(u32.MinValue);
            writer.u32(u32.MaxValue);
            var data = writer.ToArray();
            var reader = new BitBufferReader<TDecoder>();
            reader.CopyFrom(data);
            Assert.AreEqual(u32.MinValue, reader.u32());
            Assert.AreEqual(u32.MaxValue, reader.u32());
        }

        [Test]
        public void u32ReadWriteLimitsRaw() => u32ReadWriteLimits<RawEncoding<u32ArrayMemory>, RawDecoding<u32ArrayMemory>>();

        [Test]
        public void u32ReadWriteLimitsEncoded() => u32ReadWriteLimits<SevenBitEncoding<u32ArrayMemory>, SevenBitDecoding<u32ArrayMemory>>();

        public void u32ReadWriteLimits<TEncoder, TDecoder>() 
             where TEncoder:struct, ICompression<RawBitWriter<u32ArrayMemory>> 
             where TDecoder:struct, IDecompression<RawBitReader<u32ArrayMemory>> 
        {
            var writer = new BitBufferWriter<TEncoder>();
            writer.u32(123123, 0, 13213123);
            writer.u32(123, 20);
            var data =writer.ToArray();
            var reader = new BitBufferReader<TDecoder>();
            reader.CopyFrom(data);
            Assert.AreEqual(123123u, reader.u32(0, 13213123));
            Assert.AreEqual(123u, reader.u32(20));
        }             
    }
}
