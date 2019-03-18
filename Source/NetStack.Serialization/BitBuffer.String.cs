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

        public static int BitsRequired(string value, int length)
        {
            var bitLength = 10;

            uint codePage = 0; // Ascii
            for (int i = 0; i < length; i++)
            {
                var val = value[i];
                if (val > 127)
                {
                    codePage = 1; // Latin1
                    if (val > 255)
                    {
                        codePage = 2; // LatinExtended 
                        if (val > 511)
                        {
                            codePage = 3; // UTF-16
                            break;
                        }
                    }
                }
            }

            if (codePage == 0)
                bitLength += length * 7;
            else if (codePage == 1)
                bitLength += length * 8;
            else if (codePage == 2)
                bitLength += length * 9;
            else if (codePage == 3)
                for (int i = 0; i < length; i++)
                {
                    if (value[i] > 127)
                        bitLength += 17;
                    else
                        bitLength += 8;
                }

            return bitLength;
        }        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitBuffer AddString(string value)
        {
            Debug.Assert(value != null, "String is null");
            Debug.Assert(value.Length <= stringLengthMax, "String too long, raise the stringLengthMax value or split the string.");

            int length = value.Length;
            if (length > stringLengthMax)
                length = stringLengthMax;

            if (length * 17 + 10 > (totalNumBits - bitsWritten)) // possible overflow
            {
                if (BitsRequired(value, length) > (totalNumBits - bitsWritten))
                    throw new ArgumentOutOfRangeException("String would not fit in bitstream.");
            }

            uint codePage = 0; // Ascii
            for (int i = 0; i < length; i++)
            {
                var val = value[i];
                if (val > 127)
                {
                    codePage = 1; // Latin1
                    if (val > 255)
                    {
                        codePage = 2; // LatinExtended 
                        if (val > 511)
                        {
                            codePage = 3; // UTF-16
                            break;
                        }
                    }
                }
            }

            Add(2, codePage);
            Add(stringLengthBits, (uint)length);

            switch (codePage)
            {
                case 0:
                    for (int i = 0; i < length; i++)
                    {
                        Add(bitsASCII, value[i]);
                    }
                    break;
                case 1:
                    for (int i = 0; i < length; i++)
                    {
                        Add(bitsLATIN1, value[i]);
                    }
                    break;
                case 2:
                    for (int i = 0; i < length; i++)
                    {
                        Add(bitsLATINEXT, value[i]);
                    }
                    break;
                default:
                    for (int i = 0; i < length; i++)
                    {
                        if (value[i] > 127)
                        {
                            Add(1, 1);
                            Add(bitsUTF16, value[i]);
                        }
                        else
                        {
                            Add(1, 0);
                            Add(bitsASCII, value[i]);
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
            uint codePage = Read(2);
            uint length = Read(stringLengthBits);

            switch (codePage)
            {
                case 0:
                    for (int i = 0; i < length; i++)
                    {
                        outVal.Append((char)Read(bitsASCII));
                    }
                    break;
                case 1:
                    for (int i = 0; i < length; i++)
                    {
                        outVal.Append((char)Read(bitsLATIN1));
                    }
                    break;
                case 2:
                    for (int i = 0; i < length; i++)
                    {
                        outVal.Append((char)Read(bitsLATINEXT));
                    }
                    break;
                default:
                    for (int i = 0; i < length; i++)
                    {
                        var needs16 = Read(1);
                        if (needs16 == 1)
                            outVal.Append((char)Read(bitsUTF16));
                        else
                            outVal.Append((char)Read(bitsASCII));
                    }
                    break;
            }
        }
    }
}