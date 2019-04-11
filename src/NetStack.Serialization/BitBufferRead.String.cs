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
    partial class BitBufferRead
    {
      
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
            uint length = ReadRaw(config.StringLengthBits);

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