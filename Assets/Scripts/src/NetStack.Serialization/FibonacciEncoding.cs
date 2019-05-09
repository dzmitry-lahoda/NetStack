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

    public struct FibonacciEncoding<TMemory> : ICompression<RawBitWriter<TMemory>>
        where TMemory : struct, IMemory<u32>
    {
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public void i32(RawBitWriter<TMemory> b, i32 value)
        {
            u32(b, encode(value));
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public void u32(RawBitWriter<TMemory> b, u32 value)
        {
                value++;
                u64 map = 0;
                i32 index = -1;
                for (var i = Coder.Fibonacci.u32Lookup.Length - 1; i >= 0; i--)
                {
                    if (value >= Coder.Fibonacci.u32Lookup[i])
                    {
                        if (index == -1)
                        {
                            index = i + 1;
                            map = BitOperations.WriteBit(map, index, true);
                        }

                        map = BitOperations.WriteBit(map, i, true);
                        value -= Coder.Fibonacci.u32Lookup[i];
                    }
                }

                for (var i = 0; i <= index; i++)
                {
                    // TODO: optimize
                    var bit = BitOperations.ExtractBit(map, i);
                    b.b(bit);
                }
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public void u16(RawBitWriter<TMemory> b, u16 value)
        {
            value++;
            u32 map = 0;
            i32 index = -1;
            for (var i = Coder.Fibonacci.u16Lookup.Length - 1; i >= 0; i--)
            {
                if (value >= Coder.Fibonacci.u16Lookup[i])
                {
                    if (index == -1)
                    {
                        index = i + 1;
                        map = BitOperations.WriteBit(map, index, true);
                    }

                    map = BitOperations.WriteBit(map, i, true);
                    value -= Coder.Fibonacci.u16Lookup[i];
                }
            }

            for (var i = 0; i <= index; i++)
            {
                // TODO: optimize
                var bit = BitOperations.ExtractBit(map, i);
                b.b(bit);
            }
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public void u8(RawBitWriter<TMemory> b, u8 value) => b.u8Raw((u8)value);

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public u32 encode(i32 value) => Coder.ZigZag.Encode(value);

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public void i32(RawBitWriter<TMemory> b, i32 value, u8 numberOfBits) =>
            b.u32(encode(value), numberOfBits);
    }
}