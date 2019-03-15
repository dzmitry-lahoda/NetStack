using System;
using BenchmarkDotNet;
using BenchmarkDotNet.Running;

namespace benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        }
    }
}
