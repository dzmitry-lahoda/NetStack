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

namespace NetStack.Serialization
{
    /// <summary>
    /// Float ranged compression.
    /// </summary>
    public static class HalfFloatBitBufferExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddHalfFloat(this BitBufferWriter self, float value)
        {
            self.raw(HalfPrecision.Compress(value), 16);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ReadHalfFloat(this BitBufferReader self) =>
            HalfPrecision.Decompress((ushort)self.raw(16));        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float PeekHalfFloat(this BitBufferReader self) =>
            HalfPrecision.Decompress((ushort)self.raw(16));
    }
}