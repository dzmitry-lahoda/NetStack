﻿using System;
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
        public void Encodings()
        {
            var writer1 = new BitBufferWriter<SevenBit>();
            var writer2 = new BitBufferWriter<NoEncoding>();
            writer1.i32(i32.MaxValue);
            writer2.i32(i32.MaxValue);
            Assert.Equal(40, writer1.BitsWritten);
            Assert.Equal(32, writer2.BitsWritten);
        }

        [Fact]
        public void NoEncodings()
        {
            var writer = new BitBufferWriter<NoEncoding>();
            writer.i32(i32.MaxValue);
            var data = writer.ToArray();
            var value = BitConverter.ToInt32(data, 0);
            Assert.Equal(i32.MaxValue, value);
        }        
    }
}