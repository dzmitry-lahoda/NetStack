using System;
using System.Numerics;
using BenchmarkDotNet;
using BenchmarkDotNet.Attributes;
using NetStack.Serialization;

namespace NetStack.Serialization
{
    //https://stackoverflow.com/questions/1550560/encoding-an-integer-in-7-bit-format-of-c-sharp-binaryreader-readstring

    [CoreJob]
    public class SevenBitsWays
    {
        [Params(10_000_000)]
        public int N;
        private byte[] buffer1;
        private int buffer1I;

        private byte[] buffer2;
        private int buffer2I;

        [IterationSetup]
        public void GlobalSetup()
        {
            buffer1 = new byte[N * 4];
            buffer1I = 0;
            buffer2 = new byte[N * 4];
            buffer2I = 0;
        }

        [Benchmark]
        public void DoWhileWithLocalVariable()
        {
            uint x = 0;
            for (uint i = 0; i < N; i++)
            {
                var value = i;
                do
                {
                    uint buffer = value & 0b0111_1111u;
                    value >>= 7;
                    if (value > 0)
                        buffer |= 0b1000_0000u;
                    raw1((byte)buffer);
                }
                while (value > 0);
            }
        }

        //https://github.com/dotnet/corefx/blob/a7e2585c2a0748fc8105866dc3a25d076e5cfbc6/src/Common/src/CoreLib/System/IO/BinaryWriter.cs#L457

        //https://gitlab.com/Syroot/BinaryData/blob/master/src/Syroot.BinaryData.Memory/SpanWriter.cs#L292
        [Benchmark]
        public void CoreFxSevenBitWrite()
        {
            uint x = 0;
            for (uint value = 0; value < N; value++)
            {
                var unsigned = (uint)value;
                while (unsigned >= 0b10000000)
                {
                    raw2((byte)(unsigned | 0b10000000));
                    unsigned >>= 7;
                }
                raw2((byte)unsigned);
            }
        }

        private void raw2(byte buffer)
        {
            buffer2[buffer2I] = buffer;
            buffer2I++;
        }

        private void raw1(byte buffer)
        {
            buffer1[buffer1I] = buffer;
            buffer1I++;
        }
    }
}