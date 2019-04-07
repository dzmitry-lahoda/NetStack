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
        public static BitBuffer AddHalfFloat(this BitBuffer self, float value)
        {
            self.AddRaw(16, HalfPrecision.Compress(value));
            return self;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ReadHalfFloat(this BitBuffer self) =>
            HalfPrecision.Decompress((ushort)self.ReadRaw(16));        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float PeekHalfFloat(this BitBuffer self) =>
            HalfPrecision.Decompress((ushort)self.ReadRaw(16));
    }
}