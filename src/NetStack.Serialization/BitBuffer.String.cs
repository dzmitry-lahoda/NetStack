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
        private const int bitsASCII = 7;
        private const int bitsLATIN1 = 8;
        private const int bitsLATINEXT = 9;
        private const int bitsUTF16 = 16;

        private int stringLengthBits;
        private int stringLengthMax;
        private StringBuilder builder;


        internal enum CodePage : byte
        {
            Ascii = 0,
            Latin1 = 1,
            LatinExtended = 2,
            UTF16 = 3
        }

        private const int codePageBitsRequried = 2;

        public static int BitsRequired(string value, int length, int bitLength = defaultStringLengthBits)
        {
#if DEBUG || NETSTACK_VALIDATE
    if (value == null)
    {
        throw new ArgumentNullException(nameof(value));
    }
#endif
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
                    for (int i = 0; i < length; i++) {
                        if (value[i] > 127)
                            bitLength += 17;
                        else
                            bitLength += 8;
                    }
                    break;
            }

            return bitLength + codePageBitsRequried;
        }        


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitBuffer AddString(string value)
        {
            // non critical path (until string is one or couple of chars), so may consider throw
            Debug.Assert(value != null, "String is null");
            Debug.Assert(value.Length <= stringLengthMax, $"String too long, raise the {nameof(stringLengthBits)} value or split the string.");

            int length = value.Length;
            if (length > stringLengthMax)
                length = stringLengthMax;

            if (length * 17 + 10 > (totalNumberBits - BitsPassed2)) // possible overflow
            {
                if (BitsRequired(value, length) > (totalNumberBits - BitsPassed2))
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
            AddRaw((uint)length, stringLengthBits);

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ReadString()
        {
            builder.Length = 0;

            ReadString(builder);

            return builder.ToString();
        }

        private void ReadString(StringBuilder outVal)
        {
            uint codePage = ReadRaw(2);
            uint length = ReadRaw(stringLengthBits);

            switch (codePage)
            {
                case 0:
                    for (int i = 0; i < length; i++)
                    {
                        outVal.Append((char)ReadRaw(bitsASCII));
                    }
                    break;
                case 1:
                    for (int i = 0; i < length; i++)
                    {
                        outVal.Append((char)ReadRaw(bitsLATIN1));
                    }
                    break;
                case 2:
                    for (int i = 0; i < length; i++)
                    {
                        outVal.Append((char)ReadRaw(bitsLATINEXT));
                    }
                    break;
                default:
                    for (int i = 0; i < length; i++)
                    {
                        var needs16 = ReadRaw(1);
                        if (needs16 == 1)
                            outVal.Append((char)ReadRaw(bitsUTF16));
                        else
                            outVal.Append((char)ReadRaw(bitsASCII));
                    }
                    break;
            }
        }
    }
}