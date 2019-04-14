using System.Runtime.CompilerServices;
using i32 = System.Int32;
using u32 = System.UInt32;
#if !(ENABLE_MONO || ENABLE_IL2CPP)
#else
using UnityEngine;
#endif

namespace NetStack.Serialization
{
    public static class BitOptsExtensions
    {
        //https://gist.github.com/mfuerstenau/ba870a29e16536fdbaba
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public static int ZagZig(uint value) => (i32)((value >> 1) ^ (-(i32)(value & 1)));

        //https://gist.github.com/mfuerstenau/ba870a29e16536fdbaba
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public static uint ZigZag(int value) => (u32)((value << 1) ^ (value >> 31));

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public static void u32<T>(T b, u32 value)
            where T: IRawWriter
        {
            do
            {
                // TODO: how to use CPU parallelism here ? unrol loop? couple of temporal variables? 
                // TODO: mere 8 and 8 into one 16? write special handling code for 8 and 16 coming from outside?
                var buffer = value & 0b0111_1111u;
                value >>= 7;

                if (value > 0)
                    buffer |= 0b1000_0000u;

                b.raw(buffer, 8);
            }
            while (value > 0);
        }
    }
}