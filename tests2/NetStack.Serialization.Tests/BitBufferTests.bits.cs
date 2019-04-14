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
using System.Runtime.CompilerServices;

namespace NetStack.Serialization
{
    partial class BitBufferTests
    {
        [Fact]
        public void BitsRequired()
        {
            Assert.Equal(Unsafe.SizeOf<i32>() * 8, BitBuffer.BitsRequired(i32.MinValue, i32.MaxValue));
            Assert.Equal(Unsafe.SizeOf<i8>() * 8, BitBuffer.BitsRequired(i8.MinValue, i8.MaxValue));
            Assert.Equal(2, BitBuffer.BitsRequired(1, 3));
        }
    }
}
