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
    abstract partial class BitBuffer<TStorage>
    {
        protected const int bitsASCII = 7;
        protected const int bitsLATIN1 = 8;
        protected const int bitsLATINEXT = 9;
        protected const int bitsUTF16 = 16;

        internal enum CodePage : u8
        {
            Ascii = 0,
            Latin1 = 1,
            LatinExtended = 2,
            UTF16 = 3
        }

        protected const i32 codePageBitsRequired = 2;

        public static i32 BitsRequired(ReadOnlySpan<char> value, i32 length, i32 bitLength = BitBufferOptions.DefaultCharSpanBitsLength)
        {
            if (value.Length == 0)
                return bitLength;

            var codePage = CodePage.Ascii;
            for (var i = 0; i < length; i++)
            {
                var val = value[i];
                if (val > 127)
                {
                    codePage = CodePage.Latin1;
                    if (val > 255)
                    {
                        codePage = CodePage.LatinExtended;
                        if (val > 511)
                        {
                            codePage = CodePage.UTF16;
                            break;
                        }
                    }
                }
            }

            switch (codePage)
            {
                case CodePage.Ascii:
                    bitLength += length * 7;
                    break;
                case CodePage.Latin1:
                    bitLength += length * 8;
                    break;
                case CodePage.LatinExtended:
                    bitLength += length * 9;
                    break;
                default:
                    for (int i = 0; i < length; i++)
                    {
                        if (value[i] > 127)
                            bitLength += 17;
                        else
                            bitLength += 8;
                    }
                    break;
            }

            return bitLength + codePageBitsRequired;
        }
    }
}