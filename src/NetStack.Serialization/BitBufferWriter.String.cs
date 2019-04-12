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

namespace NetStack.Serialization
{
    // GC allocated String stuff
    partial class BitBufferWriter<T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void String(string value)
        {
            // non critical path (until string is one or couple of chars), so may consider throw
            Debug.Assert(value != null, "String is null");
            Debug.Assert(value.Length <= config.StringLengthMax, $"String too long, raise the {nameof(config.StringLengthBits)} value or split the string.");

            int length = value.Length;
            if (length > config.StringLengthMax)
                length = config.StringLengthMax;

            if (length * 17 + 10 > (totalNumberBits - BitsWritten)) // possible overflow
            {
                if (BitsRequired(value, length) > (totalNumberBits - BitsWritten))
                    throw new ArgumentOutOfRangeException("String would not fit in bitstream.");
            }

            var codePage = CodePage.Ascii;
            for (int i = 0; i < length; i++)
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

            raw((uint)codePage, codePageBitsRequried);
            raw((uint)length, config.StringLengthBits);

            switch (codePage)
            {
                case CodePage.Ascii:
                    for (int i = 0; i < length; i++)
                    {
                        raw(value[i], bitsASCII);
                    }
                    break;
                case CodePage.Latin1:
                    for (int i = 0; i < length; i++)
                    {
                        raw(value[i], bitsLATIN1);
                    }
                    break;
                case CodePage.LatinExtended:
                    for (int i = 0; i < length; i++)
                    {
                        raw(value[i], bitsLATINEXT);
                    }
                    break;
                default:
                    for (int i = 0; i < length; i++)
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