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
        public static readonly BitBufferOptions Default = new BitBufferOptions(stringLengthBits: DefaultStringLengthBits, byteArrLengthBits: defaultByteArrLengthBits);

        private const int defaultByteArrLengthBits = 9;

        public const int DefaultStringLengthBits = 8;

        private readonly int byteArrLengthMax;

        public int ByteArrLengthMax => byteArrLengthMax;

        public int StringLengthMax => stringLengthMax;

        private readonly int byteArrLengthBits;

        public int ByteArrLengthBits => byteArrLengthBits;

        private readonly int stringLengthBits;

        public int StringLengthBits => stringLengthBits;

        private readonly int stringLengthMax;

        /// <summary>
        /// Creates new instance with its own buffer. 
        /// </summary>
        /// <param name="stringLengthBits">Bits used to store length of strings.</param>
        /// <param name="byteArrLengthBits">Bits used to store length of byte arrays.</param>
        public BitBufferOptions(int stringLengthBits = DefaultStringLengthBits, int byteArrLengthBits = defaultByteArrLengthBits)
        {
            if (stringLengthBits <= 0)
                throw new ArgumentException("Should be positive", nameof(stringLengthBits));

            if (byteArrLengthBits <= 0)
                throw new ArgumentException("Should be positive", nameof(byteArrLengthBits));

            // one time setup
            this.byteArrLengthBits = byteArrLengthBits;
            byteArrLengthMax = (1 << byteArrLengthBits) - 1;
            this.stringLengthBits = stringLengthBits;
            stringLengthMax = (1 << stringLengthBits) - 1;
        }
    }
}