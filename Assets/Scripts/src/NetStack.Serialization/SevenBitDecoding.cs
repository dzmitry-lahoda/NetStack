using System.Numerics;
using System.Runtime.CompilerServices;
using i32 = System.Int32;
using u32 = System.UInt32;

namespace NetStack.Serialization
{
    public struct SevenBitDecoding : IDecompression<BitBufferReader<SevenBitDecoding>>
    {
        // https://stackoverflow.com/questions/1550560/encoding-an-integer-in-7-bit-format-of-c-sharp-binaryreader-readstring
        // https://gitlab.com/Syroot/BinaryData/blob/master/src/Syroot.BinaryData.Memory/SpanReader.cs#L262
        // https://github.com/dotnet/corefx/blob/master/src/Common/src/CoreLib/System/IO/BinaryReader.cs#L619
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public u32 u32(BitBufferReader<SevenBitDecoding> b)
        {
            u32 buffer = 0;
            u32 value = 0;
            i32 shift = 0;

            do
            {
                // has no guard against corrupted bytes - may read overflow
                buffer = b.u8();
                value |= (buffer & 0b0111_1111u) << shift;
                shift += 7;
            }
            while ((buffer & 0b1000_0000u) > 0);

            return value;

            // u32 value = 0;
            // for (var i = 0; i < sizeof(u32) + 1; i++)
            // {
            //     byte readByte = b.u8();
            //     value |= (readByte & 0b0111_1111) << i * 7;
            //     if ((readByte & 0b1000_0000) == 0)
            //         return value;
            // }

            return value;
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public i32 decode(u32 value) => Coder.ZigZag.Decode(value);

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public i32 i32(BitBufferReader<SevenBitDecoding> b) => decode(u32(b));

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public i32 i32(BitBufferReader<SevenBitDecoding> b, i32 numberOfBits)
        {
            u32 value = b.u32(numberOfBits);
            return decode(value);
        }
    }
}