using System.Runtime.CompilerServices;
using u32 = System.UInt32;
#if !(ENABLE_MONO || ENABLE_IL2CPP)
#else
using UnityEngine;
#endif

namespace NetStack.Serialization
{
    public struct NoEncoding : ICompression<BitBufferWriter<NoEncoding>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void u32(BitBufferWriter<NoEncoding> b, u32 value)
        {
            b.raw(value, 32);
        }
    }
}