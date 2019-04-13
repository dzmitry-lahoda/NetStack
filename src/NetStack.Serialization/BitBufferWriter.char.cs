using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Buffers;
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

namespace NetStack.Serialization
{
    // GC allocated String stuff
    partial class BitBufferWriter<T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void chars(ReadOnlySpan<char> value)
        {
            if (value.Length > config.StringLengthMax)
                        throw ArgumentOutOfRange($"String too long, raise the {nameof(config.StringLengthBits)} value or split the string.");

            int length = value.Length;

            if (length * 17 + 10 > (totalNumberBits - BitsWritten)) // possible overflow
            {
                if (BitsRequired(value, length) > (totalNumberBits - BitsWritten))
                    throw ArgumentOutOfRange("String would not fit in bitstream.");
            }

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

            raw((u32)codePage, codePageBitsRequried);
            raw((u32)length, config.StringLengthBits);

            switch (codePage)
            {
                case CodePage.Ascii:
                    for (var i = 0; i < length; i++)
                    {
                        raw(value[i], bitsASCII);
                    }
                    break;
                case CodePage.Latin1:
                    for (var i = 0; i < length; i++)
                    {
                        raw(value[i], bitsLATIN1);
                    }
                    break;
                case CodePage.LatinExtended:
                    for (var i = 0; i < length; i++)
                    {
                        raw(value[i], bitsLATINEXT);
                    }
                    break;
                default:
                    for (var i = 0; i < length; i++)
                    {
                        if (value[i] > 127)
                        {
                            raw(1, 1);
                            raw(value[i], bitsUTF16);
                        }
                        else
                        {
                            raw(0, 1);
                            raw(value[i], bitsASCII);
                        }
                    }
                    break;
            }
        }
    }
}