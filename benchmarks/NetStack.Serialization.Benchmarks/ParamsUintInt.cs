using System;
using System.Numerics;
using BenchmarkDotNet;
using BenchmarkDotNet.Attributes;
using NetStack.Serialization;

namespace NetStack.Serialization
{
    [CoreJob]
    public class ParamsUintInt
    {
        [Params(10000)]
        public int N;

        long DoU(uint a, int b) 
        {
            return (a << 3) + (b ^ 2) + 1;
        }

        long Do(int b, uint a) 
        {
            return (a << 3) + (b ^ 2) + 1;
        }

        [Benchmark]
        public void UIntInt()
        {
            uint x = 0;
            for (int i = 0; i < N; i++)
            {
                x += (uint)DoU(x, i);
            }
        }

       [Benchmark]
        public void IntUInt()
        {
            uint x = 0;
            for (int i = 0; i < N; i++)
            {
                x += (uint)Do(i, x);
            }
        } 
    }          
}