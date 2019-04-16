using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using static System.Except;
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
#if !(ENABLE_MONO || ENABLE_IL2CPP)
using System.Diagnostics;
using System.Numerics;
#else
using UnityEngine;
#endif

// Until migration to .NET Standard 2.1
using BitOperations = System.Numerics.BitOperations;

namespace NetStack.Serialization
{

    public static class MyMethodImplOptions
    {
        //0x0200 | 0x0100
        public const short AggressiveInlining =  0x0100;
}
 
    public struct GenericSevenBit : ICompression<GenericBitBufferWriter<GenericSevenBit, ArraySpan>>
    {
        public void i32(GenericBitBufferWriter<GenericSevenBit, ArraySpan> b, i32 value)
        {
            u32(b, encode(value));
        }

        [MethodImpl(MyMethodImplOptions.AggressiveInlining)]
        public void u32(GenericBitBufferWriter<GenericSevenBit, ArraySpan> b, u32 value)
        {
            do
            {
                var buffer = value & 0b0111_1111u;
                value >>= 7;

                if (value > 0)
                    buffer |= 0b1000_0000u;

                b.raw(buffer, 8);
            }
            while (value > 0);
        }

        [MethodImpl(MyMethodImplOptions.AggressiveInlining)]
        public uint encode(i32 value) => (u32)((value << 1) ^ (value >> 31));

        [MethodImpl(MyMethodImplOptions.AggressiveInlining)]
        public void i32(GenericBitBufferWriter<GenericSevenBit, ArraySpan> b, i32 value, i32 numberOfBits)
        {
             b.raw(encode(value), numberOfBits);
        }
    }

    public struct GenericSevenBit2 : ICompression<GenericBitBufferWriter<GenericSevenBit2, MemorySpan>>
    {
        public void i32(GenericBitBufferWriter<GenericSevenBit2, MemorySpan> b, i32 value)
        {
            u32(b, encode(value));
        }

        [MethodImpl(MyMethodImplOptions.AggressiveInlining)]
        public void u32(GenericBitBufferWriter<GenericSevenBit2, MemorySpan> b, u32 value)
        {
            do
            {
                var buffer = value & 0b0111_1111u;
                value >>= 7;

                if (value > 0)
                    buffer |= 0b1000_0000u;

                b.raw(buffer, 8);
            }
            while (value > 0);
        }

        [MethodImpl(MyMethodImplOptions.AggressiveInlining)]
        public uint encode(i32 value) => (u32)((value << 1) ^ (value >> 31));

        [MethodImpl(MyMethodImplOptions.AggressiveInlining)]
        public void i32(GenericBitBufferWriter<GenericSevenBit2, MemorySpan> b, i32 value, i32 numberOfBits)
        {
             b.raw(encode(value), numberOfBits);
        }
    }

}