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
        public void f64ReadWrite()
        {
            var writer = new BitBufferWriter();
            writer.f64(f64.MaxValue);
            var data = writer.ToArray();
            var reader = new BitBufferReader();
            reader.FromArray(data);
            Assert.Equal(f64.MaxValue, reader.f64());
        }
    }
}
