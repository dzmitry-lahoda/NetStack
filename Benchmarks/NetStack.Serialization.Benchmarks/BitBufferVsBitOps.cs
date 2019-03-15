using System;

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

        }

        [Benchmark]
        public void BitOps0b0000_0000()
        {

        }

    }
}