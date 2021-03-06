using System.Numerics;
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
    /// <summary>
    /// Reads 7 bit encoded values.
    /// </summary>
    public struct SevenBitDecoding<TMemory> : IDecompression<RawBitReader<TMemory>>
        where TMemory : struct, IMemory<u32>    
    {
        // https://stackoverflow.com/questions/1550560/encoding-an-integer-in-7-bit-format-of-c-sharp-binaryreader-readstring
        // https://gitlab.com/Syroot/BinaryData/blob/master/src/Syroot.BinaryData.Memory/SpanReader.cs#L262
        // https://github.com/dotnet/corefx/blob/master/src/Common/src/CoreLib/System/IO/BinaryReader.cs#L619
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public u32 u32(RawBitReader<TMemory> b)
        {
            u32 buffer = 0;
            u32 value = 0;
            i32 shift = 0;

            do
            {
                // has no guard against corrupted bytes - may read overflow
                buffer = b.u8Raw();
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
        public i32 i32(RawBitReader<TMemory> b) => decode(u32(b));

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public i32 i32(RawBitReader<TMemory> b, u8 numberOfBits)
        {
            u32 value = b.u32(numberOfBits);
            return decode(value);
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public u8 u8(RawBitReader<TMemory> b) => b.u8Raw();

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public u16 u16(RawBitReader<TMemory> b) => (u16)u32(b);
    }
}