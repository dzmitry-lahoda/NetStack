using System.Runtime.CompilerServices;
using i32 = System.Int32;
using u32 = System.UInt32;
#if !(ENABLE_MONO || ENABLE_IL2CPP)
#else
using UnityEngine;
#endif

namespace NetStack.Serialization
{
    public struct RawDecoding : IDecompression<BitBufferReader<RawDecoding>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public u32 u32(BitBufferReader<RawDecoding> b) => b.raw(32);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public i32 decode(u32 value) => (i32)value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public i32 i32(BitBufferReader<RawDecoding> b) => decode(u32(b));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public i32 i32(BitBufferReader<RawDecoding> b, i32 numberOfBits) => i32(b);
    }
}