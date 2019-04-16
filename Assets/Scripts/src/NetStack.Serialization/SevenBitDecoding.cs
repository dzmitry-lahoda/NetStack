using System.Runtime.CompilerServices;
using i32 = System.Int32;
using u32 = System.UInt32;

namespace NetStack.Serialization
{
    public struct SevenBitDecoding : IDecompression<BitBufferReader<SevenBitDecoding>>
    {
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public u32 u32(BitBufferReader<SevenBitDecoding> b)
        {
            u32 buffer = 0;
            u32 value = 0;
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

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public i32 decode(u32 value) => BitOptsExtensions.ZagZig(value);

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public i32 i32(BitBufferReader<SevenBitDecoding> b) => decode(u32(b));

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public i32 i32(BitBufferReader<SevenBitDecoding> b, i32 numberOfBits)
        {
            u32 value = b.raw(numberOfBits);
            return decode(value);
        }
    }
}