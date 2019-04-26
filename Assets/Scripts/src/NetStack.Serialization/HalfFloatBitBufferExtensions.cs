using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;
using System.Numerics;

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
    /// Float ranged compression.
    /// </summary>
    public static class HalfFloatBitBufferExtensions
    {
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public static void f32Half(this BitBufferWriter<SevenBitEncoding> self, f32 value) =>
            self.raw(HalfPrecision.Compress(value), 16);

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public static f32 f32Half(this BitBufferReader<SevenBitDecoding> self) =>
            HalfPrecision.Decompress((u16)self.raw(16));

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public static f32 f32HalfPeek(this BitBufferReader<SevenBitDecoding> self) =>
            HalfPrecision.Decompress((u16)self.raw(16));
    }
}