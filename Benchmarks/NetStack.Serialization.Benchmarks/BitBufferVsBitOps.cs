using System;
using System.Numerics;
using BenchmarkDotNet;
using BenchmarkDotNet.Attributes;
using NetStack.Serialization;

namespace benchmarks
{
    [CoreJob]
    public class UtilsVsBitOps
    {
        [Benchmark]
        public void BitBuffer0b0000_0000()
        {
            int x = 0;
            for (int j = 0; j < 1_000; j++)
                x = BitBuffer.FindHighestBitPosition(0b0000_0000);
        }

        [Benchmark]
        public void BitOps0b0000_0000()
        {
            int x = 0;
            for (int j = 0; j < 1_000; j++)
                x = 8 - BitOps.LeadingZeroCount(0b0000_0000);
        }

        [Benchmark]
        public void BitBuffer0b1111_1111()
        {
            int x = 0;
            for (int j = 0; j < 1_000; j++)
                x = BitBuffer.FindHighestBitPosition(0b1111_1111);
        }

        [Benchmark]
        public void BitOps0b1111_1111()
        {
            int x = 0;
            for (int j = 0; j < 1_000; j++)
                x = 8 - BitOps.LeadingZeroCount(0b1111_1111);
        }

        [Benchmark]
        public void BitBuffer0b0000_1111()
        {
            int x = 0;
            for (int j = 0; j < 1_000; j++)
                x = BitBuffer.FindHighestBitPosition(0b1111_1111);
        }

        [Benchmark]
        public void BitOps0b0000_1111()
        {
            int x = 0;
            for (int j = 0; j < 1_000; j++)
                x = 8 - BitOps.LeadingZeroCount(0b1111_1111);
        }
    }
}