using System;
using System.Numerics;
using BenchmarkDotNet;
using BenchmarkDotNet.Attributes;
using NetStack.Serialization;

namespace NetStack.Serialization
{
    #if !(ENABLE_MONO || ENABLE_IL2CPP)
    [CoreJob]
    #endif
    public class Versions
    {
        [Params(400_000)]
        public int N;
        private BitBufferWriter<SevenBitEncoding<u32ArrayMemory>> buffer1;
        private BitBufferWriter<RawEncoding<u32ArrayMemory>> buffer3;

        [IterationSetup]
        public void GlobalSetup()
        {
            if (N <= 0) throw new Exception();
            buffer1 = new BitBufferWriter<SevenBitEncoding<u32ArrayMemory>>(2 * 64 * N);
            buffer3 = new BitBufferWriter<RawEncoding<u32ArrayMemory>>(2 * 64 * N);
        }

       
        [Benchmark]
        public void BitBufferWriterSevenBitEncoding()
        {
            for (int i = 0; i < N; i++)
            {
                buffer1.i32(666); buffer1.i32(666); buffer1.i32(666); buffer1.i32(666);
                buffer1.i32(666); buffer1.i32(666); buffer1.i32(666); buffer1.i32(666);
                buffer1.i32(666); buffer1.i32(666); buffer1.i32(666); buffer1.i32(666);
                buffer1.i32(666); buffer1.i32(666); buffer1.i32(666); buffer1.i32(666);

                buffer1.i32(666); buffer1.i32(666); buffer1.i32(666); buffer1.i32(666);
                buffer1.i32(666); buffer1.i32(666); buffer1.i32(666); buffer1.i32(666);
                buffer1.i32(666); buffer1.i32(666); buffer1.i32(666); buffer1.i32(666);
                buffer1.i32(666); buffer1.i32(666); buffer1.i32(666); buffer1.i32(666);

                buffer1.i32(666); buffer1.i32(666); buffer1.i32(666); buffer1.i32(666);
                buffer1.i32(666); buffer1.i32(666); buffer1.i32(666); buffer1.i32(666);
                buffer1.i32(666); buffer1.i32(666); buffer1.i32(666); buffer1.i32(666);
                buffer1.i32(666); buffer1.i32(666); buffer1.i32(666); buffer1.i32(666);

                buffer1.i32(666); buffer1.i32(666); buffer1.i32(666); buffer1.i32(666);
                buffer1.i32(666); buffer1.i32(666); buffer1.i32(666); buffer1.i32(666);
                buffer1.i32(666); buffer1.i32(666); buffer1.i32(666); buffer1.i32(666);
                buffer1.i32(666); buffer1.i32(666); buffer1.i32(666); buffer1.i32(666);
            }
        }

        [Benchmark]
        public void BitBufferWriterRawEncoding()
        {
            for (int i = 0; i < N; i++)
            {
                buffer3.i32(666); buffer3.i32(666); buffer3.i32(666); buffer3.i32(666);
                buffer3.i32(666); buffer3.i32(666); buffer3.i32(666); buffer3.i32(666);
                buffer3.i32(666); buffer3.i32(666); buffer3.i32(666); buffer3.i32(666);
                buffer3.i32(666); buffer3.i32(666); buffer3.i32(666); buffer3.i32(666);

                buffer3.i32(666); buffer3.i32(666); buffer3.i32(666); buffer3.i32(666);
                buffer3.i32(666); buffer3.i32(666); buffer3.i32(666); buffer3.i32(666);
                buffer3.i32(666); buffer3.i32(666); buffer3.i32(666); buffer3.i32(666);
                buffer3.i32(666); buffer3.i32(666); buffer3.i32(666); buffer3.i32(666);

                buffer3.i32(666); buffer3.i32(666); buffer3.i32(666); buffer3.i32(666);
                buffer3.i32(666); buffer3.i32(666); buffer3.i32(666); buffer3.i32(666);
                buffer3.i32(666); buffer3.i32(666); buffer3.i32(666); buffer3.i32(666);
                buffer3.i32(666); buffer3.i32(666); buffer3.i32(666); buffer3.i32(666);

                buffer3.i32(666); buffer3.i32(666); buffer3.i32(666); buffer3.i32(666);
                buffer3.i32(666); buffer3.i32(666); buffer3.i32(666); buffer3.i32(666);
                buffer3.i32(666); buffer3.i32(666); buffer3.i32(666); buffer3.i32(666);
                buffer3.i32(666); buffer3.i32(666); buffer3.i32(666); buffer3.i32(666);
            }
        }
    }
}