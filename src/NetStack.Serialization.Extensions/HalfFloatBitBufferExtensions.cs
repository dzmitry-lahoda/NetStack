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
        public static void AddHalfFloat(this BitBufferWrite self, float value)
        {
            self.AddRaw(HalfPrecision.Compress(value), 16);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ReadHalfFloat(this BitBufferRead self) =>
            HalfPrecision.Decompress((ushort)self.raw(16));        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float PeekHalfFloat(this BitBufferRead self) =>
            HalfPrecision.Decompress((ushort)self.raw(16));
    }
}