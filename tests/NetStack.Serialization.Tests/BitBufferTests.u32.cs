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
        public void u32ReadWriteRaw() => u32ReadWrite<RawEncoding, RawDecoding>();

        [Fact]
        public void u32ReadWriteEncoded() => u32ReadWrite<SevenBitEncoding, SevenBitDecoding>();

        private void u32ReadWrite<TEncoder, TDecoder>() 
             where TEncoder:unmanaged, ICompression<BitBufferWriter<TEncoder>> 
             where TDecoder:unmanaged, IDecompression<BitBufferReader<TDecoder>> 
        {
            var writer = new BitBufferWriter<TEncoder>();
            writer.u32(u32.MinValue);
            writer.u32(u32.MaxValue);
            var data = writer.ToArray();
            var reader = new BitBufferReader<TDecoder>();
            reader.CopyFrom(data);
            Assert.Equal(u32.MinValue, reader.u32());
            Assert.Equal(u32.MaxValue, reader.u32());
        }

        [Fact]
        public void u32ReadWriteLimitsRaw() => u32ReadWriteLimits<RawEncoding, RawDecoding>();

        [Fact]
        public void u32ReadWriteLimitsEncoded() => u32ReadWriteLimits<SevenBitEncoding, SevenBitDecoding>();

        public void u32ReadWriteLimits<TEncoder, TDecoder>() 
             where TEncoder:unmanaged, ICompression<BitBufferWriter<TEncoder>> 
             where TDecoder:unmanaged, IDecompression<BitBufferReader<TDecoder>> 
        {
            var writer = new BitBufferWriter<TEncoder>();
            writer.u32(123123, 0, 13213123);
            writer.u32(123, 20);
            var data =writer.ToArray();
            var reader = new BitBufferReader<TDecoder>();
            reader.CopyFrom(data);
            Assert.Equal(123123u, reader.u32(0, 13213123));
            Assert.Equal(123u, reader.u32(20));
        }             
    }
}
