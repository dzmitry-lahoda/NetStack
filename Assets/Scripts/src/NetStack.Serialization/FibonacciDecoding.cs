using System.Numerics;
using System.Runtime.CompilerServices;
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
    public struct FibonacciDecoding : IDecompression<BitBufferReader<FibonacciDecoding>>
    {
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public u32 u32(BitBufferReader<FibonacciDecoding> b)
        {
            bool lastbit = false;
            u8 fib = 0;
            u32 result = 0;
            bool sub = false;
            while (true) // TODO: prevent loop with sane check
            {
                // TODO: optimize
                if (b.b())
                {
                    if (lastbit)
                    {
                        break;
                    }

                    result += Coder.Fibonacci.u32Lookup[fib];
                    // if (!sub)
                    // {
                    //     result -= 1;
                    //     sub = false;
                    // }

                    lastbit = true;
                }
                else
                {
                    lastbit = false;
                }

                fib++;
            }

            return result - 1;
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public u16 u16(BitBufferReader<FibonacciDecoding> b)
        {
            bool lastbit = false;
            u8 fib = 0;
            u16 result = 0;
            while (true) // TODO: prevent loop with sane check
            {
                // TODO: optimize
                if (b.b())
                {
                    if (lastbit)
                    {
                        break;
                    }

                    result += Coder.Fibonacci.u16Lookup[fib];
                    lastbit = true;
                }
                else
                {
                    lastbit = false;
                }

                fib++;
            }

            return (u16)(result - 1);
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public i32 decode(u32 value) => Coder.ZigZag.Decode(value);

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public i32 i32(BitBufferReader<FibonacciDecoding> b) => decode(u32(b));

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public i32 i32(BitBufferReader<FibonacciDecoding> b, u8 numberOfBits)
        {
            u32 value = b.u32(numberOfBits);
            return decode(value);
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public u8 u8(BitBufferReader<FibonacciDecoding> b) => (u8)b.u16();
    }
}