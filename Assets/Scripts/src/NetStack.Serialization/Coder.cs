
using i8 = System.SByte;
using i16 = System.Int16;
using i32 = System.Int32;
using i64 = System.Int64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;
using System.Runtime.CompilerServices;

namespace System.Numerics
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
            private static ReadOnlySpan<u64> u64Lookup => new u64[]
            {
                1, 2, 3, 5, 8, 13, 21, 34, 
                55, 89, 144, 233, 377, 610, 987, 1597,
                2584, 4181, 6765, 10946, 17711, 28657, 46368, 75025, 
                121393, 196418, 317811, 514229, 832040, 1346269, 2178309, 3524578, 
                5702887, 9227465, 14930352, 24157817, 39088169, 63245986, 102334155, 165580141, 267914296, 433494437, 701408733, 1134903170, 1836311903, 2971215073, 4807526976, 7778742049, 12586269025, 20365011074, 32951280099, 53316291173, 86267571272, 139583862445, 225851433717, 365435296162, 591286729879, 956722026041, 1548008755920, 2504730781961, 4052739537881, 6557470319842, 10610209857723, 17167680177565, 27777890035288, 44945570212853, 72723460248141, 117669030460994, 190392490709135, 308061521170129, 498454011879264, 806515533049393, 1304969544928657, 2111485077978050, 3416454622906707, 5527939700884757, 8944394323791464, 14472334024676221, 23416728348467685, 37889062373143906, 61305790721611591, 99194853094755497, 160500643816367088, 259695496911122585, 420196140727489673, 679891637638612258, 1100087778366101931, 1779979416004714189, 2880067194370816120, 4660046610375530309, 7540113804746346429, 12200160415121876738
            };

            /// <summary>
            ///     Minimum value this codec can support.
            /// </summary>
            public static readonly u64 MinValue = u64.MinValue;

            /// <summary>
            ///     The maximum value of a symbol this codec can support.
            /// </summary>
            public static readonly u64 MaxValue = u64.MaxValue - 1;

            public static u64 Encode(u64 value)
            {
#if !NO_EXCEPTIONS
                // Check for overflow
                if (value > MaxValue)
                {
                    Throw.Overflow("Exceeded FibonacciCodec's maximum supported symbol value of " + MaxValue + ".");
                }
#endif
                value++;
                u64 map = 0;
                int index = -1;
                for (var i = u64Lookup.Length - 1; i >= 0; i--)
                {
                    if (value >= u64Lookup[i])
                    {
                        if (index == -1)
                        {
                            index = i + 1;
                            map = BitOperations.WriteBit(map, i + 1, true);
                        }

                        map = BitOperations.WriteBit(map, i, true);
                        value -= u64Lookup[i];
                    }
                }

                return map;
            }

           public static u64 Decode(u64 value)
            {   
                var decoded = 42u;
                
                return decoded;
            }            
        }
    }
}
