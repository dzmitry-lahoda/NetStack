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
        public void f32ReadWrite()
        {
            var writer = new BitBufferWriter<SevenBitEncoding>();
            writer.f32(123.456f);
            writer.Finish();
            var data = writer.ToArray();
            var reader = new BitBufferReader<SevenBitDecoding>();
            reader.CopyFrom(data);
            Assert.Equal(123.456f, reader.f32());
        }

        [Fact]
        public void f32MinMaxRequired()
        {
            var writer = new BitBufferWriter<SevenBitEncoding>();
            writer.f32(1234.5f, 0, 12345.6f, 0.01f);
            var data = writer.ToArray();
            var reader = new BitBufferReader<SevenBitDecoding>();
            reader.CopyFrom(data);
            Assert.Equal(1234.5f, reader.f32(0, 12345.6f, 0.01f));
        }


        [Fact]
        public void f32Bits()
        {
            var writer = new BitBufferWriter<SevenBitEncoding>();
            writer.f32(1f, 0f, 1f, 1);
            var data = writer.ToArray();
            var reader = new BitBufferReader<SevenBitDecoding>();
            reader.CopyFrom(data);
            Assert.Equal(1, reader.f32(0f, 1f, 1));
        }        

#if DEBUG || NETSTACK_VALIDATE
        [Fact]
        public void f32WriteOutOfRange()
        {
            var writer = new BitBufferWriter<SevenBitEncoding>();
            Assert.Throws<ArgumentOutOfRangeException>(()=> writer.f32(12345f, 0f, 123f, 0.1f));
        }        
#endif         
   }
}
