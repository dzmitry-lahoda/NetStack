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
        public void BitOpsLog2()
        {

        }

        [Benchmark]
        public void MyLog2()
        {
        }
    }
}