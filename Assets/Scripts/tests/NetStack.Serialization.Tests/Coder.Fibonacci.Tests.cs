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
using System;
using System.Linq;
using System.Diagnostics;
using System.ComponentModel;

namespace tests
{
    public partial class CoderTests
    {
        [Test]
        public void FibonacciLookup()
        {
           
            var value = 0b1000_0001u;
            //var x2 = BitOperations.WriteBit(x, 7, false);
            var coded = Coder.Fibonacci.Encode(value);
            var valueBinary = ToBinary(value);
            Assert.AreEqual("0000000000000000000000000000000000000000000000000000011010001010", ToBinary(coded));
            Assert.True(coded > value);
            var decoded = Coder.Fibonacci.Decode(coded);
            //Assert.AreEqual(123u, decoded);
        }

        public string ToBinary(u64 value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            string s = "";

            foreach (var item in bytes)
            {
                s = Convert.ToString(item, 2).PadLeft(8, '0') + s;  //可以把2改成8或者16，那么基数就是8和16了。
            }


            return s;
        }
    }
}