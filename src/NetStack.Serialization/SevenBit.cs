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
    public struct SevenBit : ICompression<BitBufferWriter<SevenBit>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void u32(BitBufferWriter<SevenBit> b, u32 value)
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
    }
}