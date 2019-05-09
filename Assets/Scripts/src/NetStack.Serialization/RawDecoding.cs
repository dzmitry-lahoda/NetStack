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
    public struct RawDecoding<TMemory> : IDecompression<RawBitReader<TMemory>>
        where TMemory : struct, IMemory<u32>   
    {
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public u32 u32(RawBitReader<TMemory> b) => b.u32(32);

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public i32 decode(u32 value) => (i32)value;

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public i32 i32(RawBitReader<TMemory> b) => decode(u32(b));

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public i32 i32(RawBitReader<TMemory> b, i32 numberOfBits) => i32(b);

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public u8 u8(RawBitReader<TMemory> b) => b.u8Raw();

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public u16 u16(RawBitReader<TMemory> b) => b.u16(16);
    }
}