using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using BenchmarkDotNet;
using BenchmarkDotNet.Attributes;
using NetStack.Serialization;

namespace NetStack.Serialization
{
    //https://stackoverflow.com/questions/1550560/encoding-an-integer-in-7-bit-format-of-c-sharp-binaryreader-readstring

 #if !(ENABLE_MONO || ENABLE_IL2CPP)
    [CoreJob]
    #endif
    public class SevenBitsWays
    {
        [Params(10_000_000)]
        public int N;
        private byte[] buffer1;
        private int buffer1I;

        private byte[] buffer2;
        private int buffer2I;

        private byte[] buffer3;
        private int buffer3I;

        private byte[] buffer4;
        private int buffer4I;


        [IterationSetup]
        public void GlobalSetup()
        {
            buffer1 = new byte[N * 3];
            buffer1I = 0;
            buffer2 = new byte[N * 3];
            buffer2I = 0;
            buffer3 = new byte[N * 3];
            buffer3I = 0;
            buffer4 = new byte[N * 3];
            buffer4I = 0;
        }

        [Benchmark]
        public void DoWhileWithLocalVariableLess10_000()
        {
            uint x = 0;
            for (uint i = 0; i < N; i++)
            {
                var value = (i % 10_000);
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
        public void CoreFxSevenBitWriteLess10_000()
        {
            uint x = 0;
            for (uint value = 0; value < N; value++)
            {
                var unsigned = (uint)(value % 10_000);
                while (unsigned >= 0b10000000)
                {
                    raw2((byte)(unsigned | 0b10000000));
                    unsigned >>= 7;
                }
                raw2((byte)unsigned);
            }
        }


        [Benchmark]
        public void DoWhileWithLocalVariableLessN()
        {
            uint x = 0;
            for (uint i = 0; i < N; i++)
            {
                var value = (i);
                do
                {
                    uint buffer = value & 0b0111_1111u;
                    value >>= 7;
                    if (value > 0)
                        buffer |= 0b1000_0000u;
                    raw3((byte)buffer);
                }
                while (value > 0);
            }
        }

        //https://github.com/dotnet/corefx/blob/a7e2585c2a0748fc8105866dc3a25d076e5cfbc6/src/Common/src/CoreLib/System/IO/BinaryWriter.cs#L457

        //https://gitlab.com/Syroot/BinaryData/blob/master/src/Syroot.BinaryData.Memory/SpanWriter.cs#L292
        [Benchmark]
        public void CoreFxSevenBitWriteLessN()
        {
            uint x = 0;
            for (uint value = 0; value < N; value++)
            {
                var unsigned = (uint)(value);
                while (unsigned >= 0b10000000)
                {
                    raw2((byte)(unsigned | 0b10000000));
                    unsigned >>= 7;
                }
                raw4((byte)unsigned);
            }
        }


        [MethodImpl(MyMethodImplOptions.AggressiveInlining)]
        private void raw1(byte buffer)
        {
            buffer1[buffer1I] = buffer;
            buffer1I++;
        }

        [MethodImpl(MyMethodImplOptions.AggressiveInlining)]
        private void raw2(byte buffer)
        {
            buffer2[buffer2I] = buffer;
            buffer2I++;
        }
        [MethodImpl(MyMethodImplOptions.AggressiveInlining)]
        private void raw3(byte buffer)
        {
            buffer3[buffer3I] = buffer;
            buffer3I++;
        }
        [MethodImpl(MyMethodImplOptions.AggressiveInlining)]
        private void raw4(byte buffer)
        {
            buffer4[buffer4I] = buffer;
            buffer4I++;
        }

    }
}