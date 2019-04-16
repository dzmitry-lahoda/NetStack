using System;
using BenchmarkDotNet;
#if !(ENABLE_MONO || ENABLE_IL2CPP)
using BenchmarkDotNet.Running;
#endif

namespace NetStack.Serialization
{
    class Program
    {
        static void Main(string[] args)
        {
            #if !(ENABLE_MONO || ENABLE_IL2CPP)
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
            #endif
        }
    }
}
