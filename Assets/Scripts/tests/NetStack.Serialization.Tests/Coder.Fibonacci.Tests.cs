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
using NetStack.Serialization;

namespace tests
{
    public partial class CoderTests
    {
       [Test]
        public void Fibonacci9()
        {
               var rawWriter = new BitBufferWriter<RawEncoding<u32ArrayMemory>>();
            var value = 0b1001u;
            Coder.Fibonacci.Encode(rawWriter, value);
            var reader = new BitBufferReader<RawDecoding>();
            reader.CopyFrom(rawWriter.ToArray());
            var decoded = Coder.Fibonacci.Decode(reader);
            Assert.AreEqual(value, decoded);
        }

        [Test]
        public void Fibonacci129()
        {
            var rawWriter = new BitBufferWriter<RawEncoding<u32ArrayMemory>>();
            var value = 0b1000_0001u;
            Coder.Fibonacci.Encode(rawWriter, value);
            var reader = new BitBufferReader<RawDecoding>();
            reader.CopyFrom(rawWriter.ToArray());
            var decoded = Coder.Fibonacci.Decode(reader);
            Assert.AreEqual(value, decoded);
        }

        [Test]
        public void FibonacciX()
        {
            var rawWriter = new BitBufferWriter<RawEncoding<u32ArrayMemory>>();
            var value = 0b1111_1111_1111_1111u;
            Coder.Fibonacci.Encode(rawWriter, value);
            var reader = new BitBufferReader<RawDecoding>();
            reader.CopyFrom(rawWriter.ToArray());
            var decoded = Coder.Fibonacci.Decode(reader);
            Assert.AreEqual(value, decoded);
        }        

        public string ToBinary(u64 value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            string s = "";

            foreach (var item in bytes)
            {
                s = Convert.ToString(item, 2).PadLeft(8, '0') + s;
            }


            return s;
        }
    }
}