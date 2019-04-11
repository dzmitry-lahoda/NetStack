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

        private StringBuilder builder;


        internal enum CodePage : byte
        {
            Ascii = 0,
            Latin1 = 1,
            LatinExtended = 2,
            UTF16 = 3
        }

        private const int codePageBitsRequried = 2;

        public static int BitsRequired(string value, int length, int bitLength = BitBufferOptions.DefaultStringLengthBits)
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
    }
}