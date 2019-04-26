using System.Runtime.CompilerServices;
using i32 = System.Int32;
using u32 = System.UInt32;

namespace NetStack.Serialization
{
    public struct RawDecoding : IDecompression<BitBufferReader<RawDecoding>>
    {
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public u32 u32(BitBufferReader<RawDecoding> b) => b.u32(32);

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public i32 decode(u32 value) => (i32)value;

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public i32 i32(BitBufferReader<RawDecoding> b) => decode(u32(b));

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public i32 i32(BitBufferReader<RawDecoding> b, i32 numberOfBits) => i32(b);
    }
}