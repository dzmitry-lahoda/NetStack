using NUnit.Framework;
using i8 = System.SByte;
using i16 = System.Int16;
using i32 = System.Int32;
using i64 = System.Int64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;
using System.Numerics;

namespace tests
{
    public partial class CoderTests
    {
        [Test]
        public void ZigZag()
        {
            Assert.AreEqual(-123, Coder.ZigZag.Decode(Coder.ZigZag.Encode(-123)));
            Assert.AreEqual(-123L, Coder.ZigZag.Decode(Coder.ZigZag.Encode(-123L)));
        }
    }
}