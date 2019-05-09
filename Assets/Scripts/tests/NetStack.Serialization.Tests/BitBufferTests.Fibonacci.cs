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
        public void u64FibnacciMaxMinus1()
        {
            var rawWriter = new BitBufferWriter<RawEncoding<u32ArrayMemory>>();
            var reader = new BitBufferReader<RawDecoding<u32ArrayMemory>>();
            Coder.Fibonacci.u64Encode(rawWriter, u64.MaxValue - 1);
            reader.CopyFrom(rawWriter.ToArray());
            var decoded = Coder.Fibonacci.u64Decode(reader);
            Assert.AreEqual(u64.MaxValue -1 , decoded);
            rawWriter.Reset();
        }

        [Test]
        public void u64FibnacciMax()
        {
            var rawWriter = new BitBufferWriter<RawEncoding<u32ArrayMemory>>();
            var reader = new BitBufferReader<RawDecoding<u32ArrayMemory>>();
            Coder.Fibonacci.u64Encode(rawWriter, u64.MaxValue);
            reader.CopyFrom(rawWriter.ToArray());
            var decoded = Coder.Fibonacci.u64Decode(reader);
            Assert.AreEqual(u64.MaxValue, decoded);
            rawWriter.Reset();
        }

        [Test]
        public void u64Fibnacci()
        {
            var rawWriter = new BitBufferWriter<RawEncoding<u32ArrayMemory>>();
            var reader = new BitBufferReader<RawDecoding<u32ArrayMemory>>();
            for (u64 i = u64.MinValue; i < u16.MaxValue; i++)
            {
                Coder.Fibonacci.u64Encode(rawWriter, i);
                reader.CopyFrom(rawWriter.ToArray());
                var decoded = Coder.Fibonacci.u64Decode(reader);
                Assert.AreEqual(i, decoded);
                rawWriter.Reset();
            }
        }

         [Test]
        public void u32FibonacciMaxMinus1()
        {
            var rawWriter = new BitBufferWriter<FibonacciEncoding<u32ArrayMemory>>();
            rawWriter.u32(u32.MaxValue - 1);
            var reader = new BitBufferReader<FibonacciDecoding>();
            reader.CopyFrom(rawWriter.ToArray());
            var decoded = reader.u32();
            Assert.AreEqual(u32.MaxValue - 1, decoded);
        }

        [Test]
        public void u16Fibonacci()
        {
            var rawWriter = new BitBufferWriter<FibonacciEncoding<u32ArrayMemory>>();
            u16 value = 0b1000_0001;
            rawWriter.u16(value);
            var reader = new BitBufferReader<FibonacciDecoding>();
            reader.CopyFrom(rawWriter.ToArray());
            var decoded = reader.u16();
            Assert.AreEqual(value, decoded);
        }


        [Test]
        public void FibonacciX()
        {
            var rawWriter = new BitBufferWriter<RawEncoding<u32ArrayMemory>>();
            u64 value = 0b1111_1111_1111_1111u;
            Coder.Fibonacci.u64Encode(rawWriter, value);
            var reader = new BitBufferReader<RawDecoding<u32ArrayMemory>>();
            reader.CopyFrom(rawWriter.ToArray());
            var decoded = Coder.Fibonacci.u64Decode(reader);
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