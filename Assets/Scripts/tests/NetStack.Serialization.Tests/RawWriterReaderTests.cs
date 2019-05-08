

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
    partial class RawWriterReaderTests
    {
        [Test]
        public void u8()
        {
            var writer = new RawBitWriter<u32ArrayMemory>(new u32ArrayMemory(new u32[42]));
            u16 v1 = 42;
            writer.u16Raw(v1, 16);
        }      
    }
}