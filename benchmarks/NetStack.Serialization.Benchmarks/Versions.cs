using System;
using System.Numerics;
using BenchmarkDotNet;
using BenchmarkDotNet.Attributes;
using NetStack.Serialization;

namespace NetStack.Serialization
{
    [CoreJob]
    public class Versions
    {
        [Params(400_000)]
        public int N;
        private BitBufferWriter<SevenBitEncoding> buffer1;
        private GenericBitBufferWriter<GenericSevenBit> buffer2;
        private BitBufferWriter<RawEncoding> buffer3;

        private MemoryBufferWriter<MemorySevenBit> buffer4;

        [IterationSetup]
        public void GlobalSetup()
        {
            if (N <= 0) throw new Exception();
            buffer1 = new BitBufferWriter<SevenBitEncoding>(2 * 64 *N);
            buffer2 = new GenericBitBufferWriter<GenericSevenBit>(2 * 64 *N);
            buffer3 = new BitBufferWriter<RawEncoding>(2 * 64 *N);
            buffer4 = new MemoryBufferWriter<MemorySevenBit>(2 * 64 *N);
            
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
        public void GenericMemorySpan()
        {
            for (int i = 0; i < N; i++)
            {
                buffer4.i32(666);buffer4.i32(666);buffer4.i32(666);buffer4.i32(666);                        
                buffer4.i32(666);buffer4.i32(666);buffer4.i32(666);buffer4.i32(666);
                buffer4.i32(666);buffer4.i32(666);buffer4.i32(666);buffer4.i32(666);
                buffer4.i32(666);buffer4.i32(666);buffer4.i32(666);buffer4.i32(666);

                buffer4.i32(666);buffer4.i32(666);buffer4.i32(666);buffer4.i32(666);
                buffer4.i32(666);buffer4.i32(666);buffer4.i32(666);buffer4.i32(666);
                buffer4.i32(666);buffer4.i32(666);buffer4.i32(666);buffer4.i32(666);
                buffer4.i32(666);buffer4.i32(666);buffer4.i32(666);buffer4.i32(666);

                buffer4.i32(666);buffer4.i32(666);buffer4.i32(666);buffer4.i32(666);
                buffer4.i32(666);buffer4.i32(666);buffer4.i32(666);buffer4.i32(666);
                buffer4.i32(666);buffer4.i32(666);buffer4.i32(666);buffer4.i32(666);
                buffer4.i32(666);buffer4.i32(666);buffer4.i32(666);buffer4.i32(666);

                buffer4.i32(666);buffer4.i32(666);buffer4.i32(666);buffer4.i32(666);
                buffer4.i32(666);buffer4.i32(666);buffer4.i32(666);buffer4.i32(666);
                buffer4.i32(666);buffer4.i32(666);buffer4.i32(666);buffer4.i32(666);
                buffer4.i32(666);buffer4.i32(666);buffer4.i32(666);buffer4.i32(666);                                
            }            
        } 

        //[Benchmark]
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

        //[Benchmark]
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