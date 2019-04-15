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
        private GenericBitBufferWriter<GenericSevenBit, ArraySpan> buffer2;
        private BitBufferWriter<RawEncoding> buffer3;

        private MemoryBufferWriter<MemorySevenBit> buffer4;
        private GenericBitBufferWriter<GenericSevenBit2, MemorySpan> buffer5;

        [IterationSetup]
        public void GlobalSetup()
        {
            if (N <= 0) throw new Exception();
            buffer1 = new BitBufferWriter<SevenBitEncoding>(2 * 64 *N);
            buffer2 = new GenericBitBufferWriter<GenericSevenBit, ArraySpan>(new ArraySpan{ chunks = new uint[ 2 * 64 *N]});
            buffer3 = new BitBufferWriter<RawEncoding>(2 * 64 *N);
            buffer4 = new MemoryBufferWriter<MemorySevenBit>(2 * 64 *N);
            var x = new MemorySpan{ chunks = new Memory<uint>(new uint[ 2 * 64 *N])};
            buffer5 = new GenericBitBufferWriter<GenericSevenBit2, MemorySpan>(x);
            
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

        //[Benchmark]
        public void SimpleGenericMemory()
        {
            for (int i = 0; i < N; i++)
            {
                buffer5.i32(666);buffer5.i32(666);buffer5.i32(666);buffer5.i32(666);                        
                buffer5.i32(666);buffer5.i32(666);buffer5.i32(666);buffer5.i32(666);
                buffer5.i32(666);buffer5.i32(666);buffer5.i32(666);buffer5.i32(666);
                buffer5.i32(666);buffer5.i32(666);buffer5.i32(666);buffer5.i32(666);

                buffer5.i32(666);buffer5.i32(666);buffer5.i32(666);buffer5.i32(666);
                buffer5.i32(666);buffer5.i32(666);buffer5.i32(666);buffer5.i32(666);
                buffer5.i32(666);buffer5.i32(666);buffer5.i32(666);buffer5.i32(666);
                buffer5.i32(666);buffer5.i32(666);buffer5.i32(666);buffer5.i32(666);

                buffer5.i32(666);buffer5.i32(666);buffer5.i32(666);buffer5.i32(666);
                buffer5.i32(666);buffer5.i32(666);buffer5.i32(666);buffer5.i32(666);
                buffer5.i32(666);buffer5.i32(666);buffer5.i32(666);buffer5.i32(666);
                buffer5.i32(666);buffer5.i32(666);buffer5.i32(666);buffer5.i32(666);

                buffer5.i32(666);buffer5.i32(666);buffer5.i32(666);buffer5.i32(666);
                buffer5.i32(666);buffer5.i32(666);buffer5.i32(666);buffer5.i32(666);
                buffer5.i32(666);buffer5.i32(666);buffer5.i32(666);buffer5.i32(666);
                buffer5.i32(666);buffer5.i32(666);buffer5.i32(666);buffer5.i32(666);                                
            }
        }   
       
        //[Benchmark]
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