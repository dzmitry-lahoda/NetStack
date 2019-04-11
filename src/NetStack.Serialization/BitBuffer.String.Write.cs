using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
#if !(ENABLE_MONO || ENABLE_IL2CPP)
using System.Diagnostics;
using System.Numerics;
#else
using UnityEngine;
#endif

namespace NetStack.Serialization
{
    // GC allocated String stuff
    partial class BitBuffer
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitBuffer AddString(string value)
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

            AddRaw((uint)codePage, codePageBitsRequried);
            AddRaw((uint)length, config.StringLengthBits);

            switch (codePage)
            {
                case CodePage.Ascii:
                    for (int i = 0; i < length; i++)
                    {
                        AddRaw(value[i], bitsASCII);
                    }
                    break;
                case CodePage.Latin1:
                    for (int i = 0; i < length; i++)
                    {
                        AddRaw(value[i], bitsLATIN1);
                    }
                    break;
                case CodePage.LatinExtended:
                    for (int i = 0; i < length; i++)
                    {
                        AddRaw(value[i], bitsLATINEXT);
                    }
                    break;
                default:
                    for (int i = 0; i < length; i++)
                    {
                        if (value[i] > 127)
                        {
                            AddRaw(1, 1);
                            AddRaw(value[i], bitsUTF16);
                        }
                        else
                        {
                            AddRaw(0, 1);
                            AddRaw(value[i], bitsASCII);
                        }
                    }
                    break;
            }

            return this;
        }
    }
}