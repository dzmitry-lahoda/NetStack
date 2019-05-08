using System.Runtime.CompilerServices;
using i8 = System.SByte;
using i16 = System.Int16;
using i32 = System.Int32;
using i64 = System.Int64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;
using f32 = System.Single;
using f64 = System.Double;

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

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public u8 u8(BitBufferReader<RawDecoding> b) => b.u8Raw();
    }
}