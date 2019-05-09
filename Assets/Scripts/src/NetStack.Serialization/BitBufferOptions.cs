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
    public readonly struct BitBufferOptions : IEquatable<BitBufferOptions>
    {
        public static readonly BitBufferOptions Default = new BitBufferOptions(charSpanBitsLength: DefaultCharSpanBitsLength, u8SpanBitsLength: DefaultU8SpanBitsLength);

        public const u8 DefaultU8SpanBitsLength = 9;

        public const u8 DefaultCharSpanBitsLength = 8;


        private readonly u16 u8SpanLengthMax;
        private readonly u8 u8SpanBitsLength;
        private readonly u16 charSpanLengthMax;
        private readonly u8 charSpanBitsLength;


        public u16 U8SpanLengthMax => u8SpanLengthMax;

        public u16 CharSpanLengthMax => charSpanLengthMax;

        public u8 U8SpanBitsLength => u8SpanBitsLength;

        public u8 CharSpanBitsLength => charSpanBitsLength;

        /// <summary>
        /// Creates new instance with its own buffer. 
        /// </summary>
        /// <param name="charSpanBitsLength">Bits used to store length of strings.</param>
        /// <param name="u8SpanBitsLength">Bits used to store length of byte arrays.</param>
        public BitBufferOptions(u8 charSpanBitsLength = DefaultCharSpanBitsLength, u8 u8SpanBitsLength = DefaultU8SpanBitsLength)
        {
            if (charSpanBitsLength <= 0)
                Throw.Argument("Should be positive", nameof(charSpanBitsLength));

            if (u8SpanBitsLength <= 0)
                Throw.Argument("Should be positive", nameof(u8SpanBitsLength));

            // one time setup
            this.u8SpanBitsLength = u8SpanBitsLength;
            u8SpanLengthMax = (u16)((1 << u8SpanBitsLength) - 1);
            this.charSpanBitsLength = charSpanBitsLength;
            charSpanLengthMax = (u16)((1 << charSpanBitsLength) - 1);
        }

        public bool Equals(BitBufferOptions other) => 
            this.u8SpanBitsLength == other.u8SpanBitsLength && this.charSpanBitsLength == other.charSpanBitsLength;
    }
}