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
    public class  BitBufferOptions
    {
        public static readonly BitBufferOptions Default = new BitBufferOptions(stringLengthBits: DefaultCharSpanLengthBits, byteArrLengthBits: DefaultU8SpanLengthBits);

        public const i32 DefaultU8SpanLengthBits = 9;

        public const i32 DefaultCharSpanLengthBits = 8;

        private readonly i32 byteArrLengthMax;

        public i32 ByteArrLengthMax => byteArrLengthMax;

        public i32 StringLengthMax => stringLengthMax;

        private readonly i32 byteArrLengthBits;

        public i32 ByteArrLengthBits => byteArrLengthBits;

        private readonly i32 stringLengthBits;

        public i32 StringLengthBits => stringLengthBits;

        private readonly i32 stringLengthMax;

        /// <summary>
        /// Creates new instance with its own buffer. 
        /// </summary>
        /// <param name="stringLengthBits">Bits used to store length of strings.</param>
        /// <param name="byteArrLengthBits">Bits used to store length of byte arrays.</param>
        public BitBufferOptions(i32 stringLengthBits = DefaultCharSpanLengthBits, i32 byteArrLengthBits = DefaultU8SpanLengthBits)
        {
            if (stringLengthBits <= 0)
                throw Argument("Should be positive", nameof(stringLengthBits));

            if (byteArrLengthBits <= 0)
                throw Argument("Should be positive", nameof(byteArrLengthBits));

            // one time setup
            this.byteArrLengthBits = byteArrLengthBits;
            byteArrLengthMax = (1 << byteArrLengthBits) - 1;
            this.stringLengthBits = stringLengthBits;
            stringLengthMax = (1 << stringLengthBits) - 1;
        }
    }
}