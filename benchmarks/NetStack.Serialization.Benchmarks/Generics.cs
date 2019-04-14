using System;
using System.Numerics;
using BenchmarkDotNet;
using BenchmarkDotNet.Attributes;
using NetStack.Serialization;

namespace NetStack.Serialization
{
    [CoreJob]
    public class Generics
    {
        [Params(100000)]
        public int N;
        private BitBufferWriter<SevenBitEncoding> buffer1;
        private GenericBitBufferWriter<SevenBitEncoding2> buffer2;
        private BitBufferWriter<RawEncoding> buffer3;

        [IterationSetup]
        public void GlobalSetup()
        {
            buffer1 = new BitBufferWriter<SevenBitEncoding>(20_000_000);
            buffer2 = new GenericBitBufferWriter<SevenBitEncoding2>(20_000_000);
            buffer3 = new BitBufferWriter<RawEncoding>(20_000_000);
        }

        [Benchmark]
        public void SimpleGenericFastAsManual()
        {
            for (int i = 0; i < N; i++)
            {
                buffer2.i32(666);buffer2.i32(666);buffer2.i32(666);buffer2.i32(666);                        
                buffer2.i32(666);buffer2.i32(666);buffer2.i32(666);buffer2.i32(666);
                buffer2.i32(666);buffer2.i32(666);buffer2.i32(666);buffer2.i32(666);
                buffer2.i32(666);buffer2.i32(666);buffer2.i32(666);buffer2.i32(666);

                buffer2.i32(666);buffer2.i32(666);buffer2.i32(666);buffer2.i32(666);
                buffer2.i32(666);buffer2.i32(666);buffer2.i32(666);buffer2.i32(666);
                buffer2.i32(666);buffer2.i32(666);buffer2.i32(666);buffer2.i32(666);
                buffer2.i32(666);buffer2.i32(666);buffer2.i32(666);buffer2.i32(666);

                buffer2.i32(666);buffer2.i32(666);buffer2.i32(666);buffer2.i32(666);
                buffer2.i32(666);buffer2.i32(666);buffer2.i32(666);buffer2.i32(666);
                buffer2.i32(666);buffer2.i32(666);buffer2.i32(666);buffer2.i32(666);
                buffer2.i32(666);buffer2.i32(666);buffer2.i32(666);buffer2.i32(666);

                buffer2.i32(666);buffer2.i32(666);buffer2.i32(666);buffer2.i32(666);
                buffer2.i32(666);buffer2.i32(666);buffer2.i32(666);buffer2.i32(666);
                buffer2.i32(666);buffer2.i32(666);buffer2.i32(666);buffer2.i32(666);
                buffer2.i32(666);buffer2.i32(666);buffer2.i32(666);buffer2.i32(666);                                
            }
        }
                

        [Benchmark]
        public void Generic7Bit()
        {
            for (int i = 0; i < N; i++)
            {
                buffer1.i32(666);buffer1.i32(666);buffer1.i32(666);buffer1.i32(666);                        
                buffer1.i32(666);buffer1.i32(666);buffer1.i32(666);buffer1.i32(666);
                buffer1.i32(666);buffer1.i32(666);buffer1.i32(666);buffer1.i32(666);
                buffer1.i32(666);buffer1.i32(666);buffer1.i32(666);buffer1.i32(666);

                buffer1.i32(666);buffer1.i32(666);buffer1.i32(666);buffer1.i32(666);
                buffer1.i32(666);buffer1.i32(666);buffer1.i32(666);buffer1.i32(666);
                buffer1.i32(666);buffer1.i32(666);buffer1.i32(666);buffer1.i32(666);
                buffer1.i32(666);buffer1.i32(666);buffer1.i32(666);buffer1.i32(666);

                buffer1.i32(666);buffer1.i32(666);buffer1.i32(666);buffer1.i32(666);
                buffer1.i32(666);buffer1.i32(666);buffer1.i32(666);buffer1.i32(666);
                buffer1.i32(666);buffer1.i32(666);buffer1.i32(666);buffer1.i32(666);
                buffer1.i32(666);buffer1.i32(666);buffer1.i32(666);buffer1.i32(666);

                buffer1.i32(666);buffer1.i32(666);buffer1.i32(666);buffer1.i32(666);
                buffer1.i32(666);buffer1.i32(666);buffer1.i32(666);buffer1.i32(666);
                buffer1.i32(666);buffer1.i32(666);buffer1.i32(666);buffer1.i32(666);
                buffer1.i32(666);buffer1.i32(666);buffer1.i32(666);buffer1.i32(666);                                
            }
        }      

        [Benchmark]
        public void NoEncoding()
        {
            for (int i = 0; i < N; i++)
            {
                buffer3.i32(666);buffer3.i32(666);buffer3.i32(666);buffer3.i32(666);                        
                buffer3.i32(666);buffer3.i32(666);buffer3.i32(666);buffer3.i32(666);
                buffer3.i32(666);buffer3.i32(666);buffer3.i32(666);buffer3.i32(666);
                buffer3.i32(666);buffer3.i32(666);buffer3.i32(666);buffer3.i32(666);

                buffer3.i32(666);buffer3.i32(666);buffer3.i32(666);buffer3.i32(666);
                buffer3.i32(666);buffer3.i32(666);buffer3.i32(666);buffer3.i32(666);
                buffer3.i32(666);buffer3.i32(666);buffer3.i32(666);buffer3.i32(666);
                buffer3.i32(666);buffer3.i32(666);buffer3.i32(666);buffer3.i32(666);

                buffer3.i32(666);buffer3.i32(666);buffer3.i32(666);buffer3.i32(666);
                buffer3.i32(666);buffer3.i32(666);buffer3.i32(666);buffer3.i32(666);
                buffer3.i32(666);buffer3.i32(666);buffer3.i32(666);buffer3.i32(666);
                buffer3.i32(666);buffer3.i32(666);buffer3.i32(666);buffer3.i32(666);

                buffer3.i32(666);buffer3.i32(666);buffer3.i32(666);buffer3.i32(666);
                buffer3.i32(666);buffer3.i32(666);buffer3.i32(666);buffer3.i32(666);
                buffer3.i32(666);buffer3.i32(666);buffer3.i32(666);buffer3.i32(666);
                buffer3.i32(666);buffer3.i32(666);buffer3.i32(666);buffer3.i32(666);                                
            }
        }            
    }
}