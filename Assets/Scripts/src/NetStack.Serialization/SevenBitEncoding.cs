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
using System.Diagnostics;
using System.Numerics;

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

//    where T : unmanaged, ICompression<BitBufferWriter<T>> 
// holder also losts 10 percentage of performance
        private struct Holder : IRawWriter
        {
            [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
            public Holder(BitBufferWriter<SevenBitEncoding> b) => this.b = b;

            private BitBufferWriter<SevenBitEncoding> b;

            [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
            public void raw(u32 value, i32 numberOfBits) => b.raw(value, numberOfBits);

            [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
            public void u8(u8 value) => b.u8(value);
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public void u32(BitBufferWriter<SevenBitEncoding> b, u32 value)
        //trying to use generic method or class with call for inline and optimize with only interface constraint losses great of performance
        // => BitOptsExtensions<Holder>.u32(new Holder(b),value);
        {
            // TODO: how to use CPU parallelism here ? unrol loop? couple of temporal variables? 
            // TODO: mere 8 and 8 into one 16? write special handling code for 8 and 16 coming from outside?
            //for (;value >= 0b10000000; b.raw((u8)(value | 0b10000000), 8), value >>= 7) {} b.u8((u8)value);
            while (value >= 0b10000000)
            {
                b.raw((u8)(value | 0b10000000), 8);
                value >>= 7;
            }
            b.u8((u8)value);
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public u32 encode(i32 value) => BitOptsExtensions.ZigZag(value);

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public void i32(BitBufferWriter<SevenBitEncoding> b, i32 value, i32 numberOfBits) =>
            b.raw(encode(value), numberOfBits);
    }
}