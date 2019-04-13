using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
#if !(ENABLE_MONO || ENABLE_IL2CPP)
using System.Diagnostics;
using System.Numerics;
#else
using UnityEngine;
#endif
using NetStack.Compression;
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void f16(this BitBufferWriter<SevenBitEncoding> self, f32 value) => 
            self.raw(HalfPrecision.Compress(value), 16);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 f16(this BitBufferReader<SevenBitDecoding> self) =>
            HalfPrecision.Decompress((u16)self.raw(16));        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 f16Peek(this BitBufferReader<SevenBitDecoding> self) =>
            HalfPrecision.Decompress((u16)self.raw(16));
    }
}