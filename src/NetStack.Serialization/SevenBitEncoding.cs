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
    // circular constrained generics work on .NET Core as fast as manual code (even slightly faster on .NET Core 2.2 x86-64 if container is class)
    // Unity FPS samples has usage of constrained generic (and IL2CPP does LLVM) indicates these should work there to
    // going container to be struct seems to be more complex and permaturely (will wait C# 8)
    public struct SevenBitEncoding : ICompression<BitBufferWriter<SevenBitEncoding>>
    {
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public void i32(BitBufferWriter<SevenBitEncoding> b, i32 value)
        {
            // have tried to have only encode, with no i32 method, 
            // but double layer of constrained generics does not propagate on .NET Core and leads to loss of performance
            u32(b, encode(value));
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public void u32(BitBufferWriter<SevenBitEncoding> b, u32 value) => BitOptsExtensions.u32<BitBufferWriter<SevenBitEncoding>>(b,value);

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public u32 encode(i32 value) => BitOptsExtensions.ZigZag(value);

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public void i32(BitBufferWriter<SevenBitEncoding> b, i32 value, i32 numberOfBits) => 
            b.raw(encode(value), numberOfBits);
    }
}