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
using System.Runtime.CompilerServices;

namespace NetStack.Serialization
{
    partial class BitBufferTests
    {
        [Test]
        public void BitsRequired()
        {
            Assert.AreEqual(Unsafe.SizeOf<i32>() * 8, BitBuffer.BitsRequired(i32.MinValue, i32.MaxValue));
            Assert.AreEqual(Unsafe.SizeOf<i8>() * 8, BitBuffer.BitsRequired(i8.MinValue, i8.MaxValue));
            Assert.AreEqual(2, BitBuffer.BitsRequired(1, 3));
        }
    }
}
