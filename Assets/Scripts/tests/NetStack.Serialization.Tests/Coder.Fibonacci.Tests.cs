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
            var reader = new BitBufferReader<RawDecoding>();
            Coder.Fibonacci.Encode(rawWriter, u64.MaxValue - 1);
            reader.CopyFrom(rawWriter.ToArray());
            var decoded = Coder.Fibonacci.Decode(reader);
            Assert.AreEqual(u64.MaxValue -1 , decoded);
            rawWriter.Reset();
        }

        [Test]
        public void u64FibnacciMax()
        {
            var rawWriter = new BitBufferWriter<RawEncoding<u32ArrayMemory>>();
            var reader = new BitBufferReader<RawDecoding>();
            Coder.Fibonacci.Encode(rawWriter, u64.MaxValue);
            reader.CopyFrom(rawWriter.ToArray());
            var decoded = Coder.Fibonacci.Decode(reader);
            Assert.AreEqual(u64.MaxValue, decoded);
            rawWriter.Reset();
        }

        [Test]
        public void u64Fibnacci()
        {
            var rawWriter = new BitBufferWriter<RawEncoding<u32ArrayMemory>>();
            var reader = new BitBufferReader<RawDecoding>();
            for (u64 i = u64.MinValue; i < u16.MaxValue; i++)
            {
                Coder.Fibonacci.Encode(rawWriter, i);
                reader.CopyFrom(rawWriter.ToArray());
                var decoded = Coder.Fibonacci.Decode(reader);
                Assert.AreEqual(i, decoded);
                rawWriter.Reset();
            }
        }

         [Test]
        public void u32FibonacciMaxMinus1()
        {
            var rawWriter = new BitBufferWriter<RawEncoding<u32ArrayMemory>>();
           Coder.Fibonacci.u32Encode(rawWriter, u32.MaxValue - 1);
            var reader = new BitBufferReader<RawDecoding>();
            reader.CopyFrom(rawWriter.ToArray());
            var decoded = Coder.Fibonacci.u32Decode(reader);
            Assert.AreEqual(u32.MaxValue - 1, decoded);
        }

        [Test]
        public void u16Fibonacci()
        {
            var rawWriter = new BitBufferWriter<RawEncoding<u32ArrayMemory>>();
            u16 value = 0b1000_0001;
            Coder.Fibonacci.u16Encode(rawWriter, value);
            var reader = new BitBufferReader<RawDecoding>();
            reader.CopyFrom(rawWriter.ToArray());
            var decoded = Coder.Fibonacci.u16Decode(reader);
            Assert.AreEqual(value, decoded);
        }


        [Test]
        public void FibonacciX()
        {
            var rawWriter = new BitBufferWriter<RawEncoding<u32ArrayMemory>>();
            u64 value = 0b1111_1111_1111_1111u;
            Coder.Fibonacci.Encode(rawWriter, value);
            var reader = new BitBufferReader<RawDecoding>();
            reader.CopyFrom(rawWriter.ToArray());
            var decoded = Coder.Fibonacci.Decode(reader);
            Assert.AreEqual(value, decoded);
        }

        [Test]
        public void FibonacciVsSeventBit()
        {
            var fib = new BitBufferWriter<RawEncoding<u32ArrayMemory>>();
            u16 value = 250;
            Coder.Fibonacci.Encode(fib, value);
            var sb = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>();
            sb.u16(value);

            Assert.AreEqual(16, sb.BitsWritten);
            Assert.AreEqual(13, fib.BitsWritten);
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