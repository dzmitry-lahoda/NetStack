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
    }
}