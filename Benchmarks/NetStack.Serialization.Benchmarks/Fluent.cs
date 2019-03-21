using System;
using System.Numerics;
using BenchmarkDotNet;
using BenchmarkDotNet.Attributes;
using NetStack.Serialization;

namespace NetStack.Serialization
{
    [CoreJob]
    public class FluentVsVoid
    {
        [Params(10000)]
        public int N;

        private BitBuffer buffer;

        [IterationSetup]
        public void GlobalSetup()
        {
            buffer = new BitBuffer(20000);
        }

        [Benchmark]
        public void VoidBool()
        {
            for (int i = 0; i < N; i++)
            {
                buffer.AddBool(true);buffer.AddBool(true);buffer.AddBool(true);buffer.AddBool(true);                        
                buffer.AddBool(true);buffer.AddBool(true);buffer.AddBool(true);buffer.AddBool(true);
                buffer.AddBool(true);buffer.AddBool(true);buffer.AddBool(true);buffer.AddBool(true);
                buffer.AddBool(true);buffer.AddBool(true);buffer.AddBool(true);buffer.AddBool(true);

                buffer.AddBool(true);buffer.AddBool(true);buffer.AddBool(true);buffer.AddBool(true);
                buffer.AddBool(true);buffer.AddBool(true);buffer.AddBool(true);buffer.AddBool(true);
                buffer.AddBool(true);buffer.AddBool(true);buffer.AddBool(true);buffer.AddBool(true);
                buffer.AddBool(true);buffer.AddBool(true);buffer.AddBool(true);buffer.AddBool(true);

                buffer.AddBool(true);buffer.AddBool(true);buffer.AddBool(true);buffer.AddBool(true);
                buffer.AddBool(true);buffer.AddBool(true);buffer.AddBool(true);buffer.AddBool(true);
                buffer.AddBool(true);buffer.AddBool(true);buffer.AddBool(true);buffer.AddBool(true);
                buffer.AddBool(true);buffer.AddBool(true);buffer.AddBool(true);buffer.AddBool(true);

                buffer.AddBool(true);buffer.AddBool(true);buffer.AddBool(true);buffer.AddBool(true);
                buffer.AddBool(true);buffer.AddBool(true);buffer.AddBool(true);buffer.AddBool(true);
                buffer.AddBool(true);buffer.AddBool(true);buffer.AddBool(true);buffer.AddBool(true);
                buffer.AddBool(true);buffer.AddBool(true);buffer.AddBool(true);buffer.AddBool(true);                                
            }
        }
                
        [Benchmark]
        public void FluentBool()
        {
            for (int i = 0; i < N; i++)
            {
                buffer.Bool(true).Bool(true).Bool(true).Bool(true)
                      .Bool(true).Bool(true).Bool(true).Bool(true)
                      .Bool(true).Bool(true).Bool(true).Bool(true)
                      .Bool(true).Bool(true).Bool(true).Bool(true)

                      .Bool(true).Bool(true).Bool(true).Bool(true)
                      .Bool(true).Bool(true).Bool(true).Bool(true)
                      .Bool(true).Bool(true).Bool(true).Bool(true)
                      .Bool(true).Bool(true).Bool(true).Bool(true)

                      .Bool(true).Bool(true).Bool(true).Bool(true)
                      .Bool(true).Bool(true).Bool(true).Bool(true)
                      .Bool(true).Bool(true).Bool(true).Bool(true)
                      .Bool(true).Bool(true).Bool(true).Bool(true)                                            

                      .Bool(true).Bool(true).Bool(true).Bool(true)
                      .Bool(true).Bool(true).Bool(true).Bool(true)
                      .Bool(true).Bool(true).Bool(true).Bool(true)
                      .Bool(true).Bool(true).Bool(true).Bool(true);
            }
        }        
    }
}