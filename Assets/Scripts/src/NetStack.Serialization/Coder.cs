
using i8 = System.SByte;
using i16 = System.Int16;
using i32 = System.Int32;
using i64 = System.Int64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;
using u128 = System.UInt128;

using System.Runtime.CompilerServices;
using System;
using System.Numerics;
using System.IO;

namespace NetStack.Serialization
{
    public class Coder
    {
        // backport of values from 3.0 to target performance into future, compile fold old target still get improvements on new runtime
        public const MethodImplOptions AggressiveInliningAndOptimization = (MethodImplOptions)(0x0100 | 0x0200);

        /// <summary>
        ///     Encode signed values as unsigned using ProtoBuffer ZigZag bijection encoding algorithm.
        ///     https://developers.google.com/protocol-buffers/docs/encoding
        ///     https://gist.github.com/mfuerstenau/ba870a29e16536fdbaba
        /// </summary>
        public static class ZigZag
        {
            /// <summary>
            ///     Encode a signed long into an ZigZag unsigned long.
            /// </summary>
            [MethodImpl(Coder.AggressiveInliningAndOptimization)]
            public static u64 Encode(i64 value) => (u64)((value << 1) ^ (value >> 63));

            /// <summary>
            ///     Decode a ZigZag unsigned long back into a signed long.
            /// </summary>
            [MethodImpl(Coder.AggressiveInliningAndOptimization)]
            public static i64 Decode(u64 value) => (i64)((value >> 1) ^ (~(value & 1) + 1));


            [MethodImpl(Coder.AggressiveInliningAndOptimization)]
            public static i32 Decode(u32 value) => (i32)((value >> 1) ^ (-(i32)(value & 1)));

            [MethodImpl(Coder.AggressiveInliningAndOptimization)]
            public static u32 Encode(i32 value) => (u32)((value << 1) ^ (value >> 31));
        }


        public static class Fibonacci
        {
            internal static ReadOnlySpan<u16> u16Lookup => new u16[]
            {
                   1, 2, 3, 5, 8, 13, 21,
                   34, 55, 89, 144, 233, 377, 610, 987, 1597,
                   2584, 4181, 6765, 10946, 17711, 28657, 46368
            };

            internal static ReadOnlySpan<u32> u32Lookup => new u32[]
             {
                1, 2, 3, 5, 8, 13, 21,
                34, 55, 89, 144, 233, 377, 610, 987, 1597,
                2584, 4181, 6765, 10946, 17711, 28657, 46368, 75025,
                121393, 196418, 317811, 514229, 832040, 1346269, 2178309, 3524578,
                5702887, 9227465, 14930352, 24157817, 39088169, 63245986, 102334155, 165580141,
                267914296, 433494437, 701408733, 1134903170, 1836311903, 2971215073
              };


            internal static ReadOnlySpan<u64> u64Lookup => new u64[]
            {
                1, 2, 3, 5, 8, 13, 21, 34,
                55, 89, 144, 233, 377, 610, 987, 1597,
                2584, 4181, 6765, 10946, 17711, 28657, 46368, 75025,
                121393, 196418, 317811, 514229, 832040, 1346269, 2178309, 3524578,
                5702887, 9227465, 14930352, 24157817, 39088169, 63245986, 102334155, 165580141,
                267914296, 433494437, 701408733, 1134903170, 1836311903, 2971215073, 4807526976, 7778742049,
                12586269025, 20365011074, 32951280099, 53316291173, 86267571272, 139583862445, 225851433717, 365435296162,
                591286729879, 956722026041, 1548008755920, 2504730781961, 4052739537881, 6557470319842, 10610209857723, 17167680177565,
                27777890035288, 44945570212853, 72723460248141, 117669030460994, 190392490709135, 308061521170129, 498454011879264, 806515533049393,
                1304969544928657, 2111485077978050, 3416454622906707, 5527939700884757, 8944394323791464, 14472334024676221, 23416728348467685, 37889062373143906,
                61305790721611591, 99194853094755497, 160500643816367088, 259695496911122585, 420196140727489673, 679891637638612258, 1100087778366101931, 1779979416004714189,
                2880067194370816120, 4660046610375530309, 7540113804746346429, 12200160415121876738
            };




            /// <summary>
            ///     Minimum value this codec can support.
            /// </summary>
            public static readonly u64 MinValue = u64.MinValue;

            /// <summary>
            ///     The maximum value of a symbol this codec can support.
            /// </summary>
            public static readonly u64 MaxValue = u64.MaxValue - 1;


            // does not supports zero, so need to add one,
            // but because of this does not supports MaxValue
            // but in order not to surprise need to support

            private static void inc(ref u64 value, ref bool one)
            {
                if (value != u64.MaxValue)
                    value++;
                else
                {
                    one = true;
                }
            }

            private static bool goe(u64 value, bool one, u64 comparand)
            {
                if (one)
                {
                    return value >= (comparand - 1);
                }

                return value >= comparand;
            }

            private static void minus(ref u64 value, ref bool one, u64 substration)
            {
                if (one)
                {
                    one = false;
                    value -= (substration - 1);
                }
                else
                {
                    value -= substration;
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static u128 WriteBit(u128 value, int bitOffset, bool on)
            {
                u128 onn = on ? u128.One << bitOffset : 0;
                return (value & ~(u128.One << bitOffset)) | onn;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static bool ExtractBit(u128 value, int bitOffset)
        => (value & (u128.One << bitOffset)) != 0;

            public static void Encode(IBitBufferWriter self, u64 value)
            {
                bool one = false;
                inc(ref value, ref one);
                u128 map = 0;
                i32 index = -1;
                for (var i = u64Lookup.Length - 1; i >= 0; i--)
                {
                    if (goe(value, one, u64Lookup[i]))
                    {
                        if (index == -1)
                        {
                            // last 1 leads to unique double 11 which is first and only one

                            index = i + 1;
                            map = WriteBit(map, index, true);
                        }

                        map = WriteBit(map, i, true);
                        minus(ref value, ref one, u64Lookup[i]);
                    }
                }

                for (var i = 0; i <= index; i++)
                {
                    // TODO: optimize
                    var bit = ExtractBit(map, i);
                    self.b(bit);
                }
            }


            public static void u32Encode(IBitBufferWriter self, u32 value)
            {
#if !NO_EXCEPTIONS
                if (value > MaxValue) Throw.Overflow($"Exceeded FibonacciCodec's maximum supported symbol value of {MaxValue}");
#endif
                value++;
                u64 map = 0;
                i32 index = -1;
                for (var i = u32Lookup.Length - 1; i >= 0; i--)
                {
                    if (value >= u32Lookup[i])
                    {
                        if (index == -1)
                        {
                            index = i + 1;
                            map = BitOperations.WriteBit(map, index, true);
                        }

                        map = BitOperations.WriteBit(map, i, true);
                        value -= u32Lookup[i];
                    }
                }

                for (var i = 0; i <= index; i++)
                {
                    // TODO: optimize
                    var bit = BitOperations.ExtractBit(map, i);
                    self.b(bit);
                }
            }

            public static void u16Encode(IBitBufferWriter self, u16 value)
            {
#if !NO_EXCEPTIONS
                if (value > MaxValue) Throw.Overflow($"Exceeded FibonacciCodec's maximum supported symbol value of {MaxValue}");
#endif
                value++;
                u32 map = 0;
                i32 index = -1;
                for (var i = u16Lookup.Length - 1; i >= 0; i--)
                {
                    if (value >= u16Lookup[i])
                    {
                        if (index == -1)
                        {
                            index = i + 1;
                            map = BitOperations.WriteBit(map, index, true);
                        }

                        map = BitOperations.WriteBit(map, i, true);
                        value -= u16Lookup[i];
                    }
                }

                for (var i = 0; i <= index; i++)
                {
                    // TODO: optimize
                    var bit = BitOperations.ExtractBit(map, i);
                    self.b(bit);
                }
            }

            public static u32 u32Decode(BitBufferReader<RawDecoding> self)
            {
                bool lastbit = false;
                u8 fib = 0;
                u32 result = 0;
                bool sub = false;
                while (true) // TODO: prevent loop with sane check
                {
                    // TODO: optimize
                    if (self.b())
                    {
                        if (lastbit)
                        {
                            break;
                        }

                        result += u32Lookup[fib];
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

            public static u16 u16Decode(BitBufferReader<RawDecoding> self)
            {
                bool lastbit = false;
                u8 fib = 0;
                u16 result = 0;
                while (true) // TODO: prevent loop with sane check
                {
                    // TODO: optimize
                    if (self.b())
                    {
                        if (lastbit)
                        {
                            break;
                        }

                        result += u16Lookup[fib];
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

            public static u64 Decode(BitBufferReader<RawDecoding> self)
            {
                bool lastbit = false;
                u8 fib = 0;
                u64 result = 0;
                bool sub = false;
                while (true) // TODO: prevent loop with sane check
                {
                    // TODO: optimize
                    if (self.b())
                    {
                        if (lastbit)
                        {
                            break;
                        }

                        result += u64Lookup[fib];
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
        }
    }
}
