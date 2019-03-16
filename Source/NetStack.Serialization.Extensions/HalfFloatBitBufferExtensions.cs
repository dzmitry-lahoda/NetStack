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
        [MethodImpl(256)]
        public static BitBuffer AddHalfFloat(this BitBuffer self, float value)
        {
            self.Add(16, HalfPrecision.Compress(value));
            return self;
        }

        [MethodImpl(256)]
        public static float ReadHalfFloat(this BitBuffer self) =>
            HalfPrecision.Decompress((ushort)self.Read(16));        

        [MethodImpl(256)]
        public static float PeekHalfFloat(this BitBuffer self) =>
            HalfPrecision.Decompress((ushort)self.Peek(16));
    }
}