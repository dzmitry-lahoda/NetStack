using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Buffers;
#if !(ENABLE_MONO || ENABLE_IL2CPP)
using System.Diagnostics;
using System.Numerics;
#else
using UnityEngine;
#endif

namespace NetStack.Serialization
{
    public static class StringBitBufferExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static string String(this BitBufferReader<SevenBitDecoding> self)
        {
            var size = MemoryPool<char>.Shared.Rent(self.Options.StringLengthMax);
            var length = self.chars(size.Memory.Span);
            using (var pin = size.Memory.Pin())
            {
                return new String((char*)pin.Pointer,0, (int)length);
            }            
        }
    }
}