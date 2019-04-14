using System.Runtime.CompilerServices;
using i32 = System.Int32;
using u32 = System.UInt32;
#if !(ENABLE_MONO || ENABLE_IL2CPP)
#else
using UnityEngine;
#endif

// Until migration to .NET Standard 2.1

namespace NetStack.Serialization
{
    public struct SevenBitDecoding : IDecompression<BitBufferReader<SevenBitDecoding>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public u32 u32(BitBufferReader<SevenBitDecoding> b)
        {
            u32 buffer = 0x0u;
            u32 value = 0x0u;
            i32 shift = 0;

            do
            {
                buffer = b.raw(8);

                value |= (buffer & 0b0111_1111u) << shift;
                shift += 7;
            }
            while ((buffer & 0b1000_0000u) > 0);

            return value;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public i32 decode(u32 value) => (i32)((value >> 1) ^ (-(i32)(value & 1)));

        public i32 i32(BitBufferReader<SevenBitDecoding> b) => decode(u32(b));
    }
}