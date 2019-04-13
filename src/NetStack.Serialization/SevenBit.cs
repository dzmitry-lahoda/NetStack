using System.Runtime.CompilerServices;
using u32 = System.UInt32;
#if !(ENABLE_MONO || ENABLE_IL2CPP)
#else
using UnityEngine;
#endif

namespace NetStack.Serialization
{
    // circular constrained generics work on .NET Core as fast as manual code (even slightly faster on .NET Core 2.2 x86-64 if container is class)
    // Unity FPS samples has usage of constrained generic (and IL2CPP does LLVM) indicates these should work there to
    // going container to be struct seems to be more complex and permaturely (will wait C# 8)
    public struct SevenBitEncoding : ICompression<BitBufferWriter<SevenBitEncoding>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void u32(BitBufferWriter<SevenBitEncoding> b, u32 value)
        {
            do
            {
                var buffer = value & 0b0111_1111u;
                value >>= 7;

                if (value > 0)
                    buffer |= 0b1000_0000u;

                b.raw(buffer, 8);
            }
            while (value > 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint zigzag(int value) => (uint)((value << 1) ^ (value >> 31));
    }

    public struct SevenBitDecoding : IDecompression<BitBufferReader<SevenBitDecoding>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public u32 u32(BitBufferReader<SevenBitDecoding> b)
        {
            u32 buffer = 0x0u;
            u32 value = 0x0u;
            int shift = 0;

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
        public int zagzig(uint value) => (int)((value >> 1) ^ (-(int)(value & 1)));
    }
}