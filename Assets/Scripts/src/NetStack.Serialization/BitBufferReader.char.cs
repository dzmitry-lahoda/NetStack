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
    partial class BitBufferReader<T>
    {
        /// <summary>
        /// Reads string.`
        /// </summary>
        /// <param name="outputValue">Span to fill</param>
        public u32 c(Span<char> outputValue)
        {
            u32 codePage = raw(2);
            u32 length = raw(config.CharSpanBitsLength);

            switch (codePage)
            {
                case 0:
                    for (var i = 0; i < length; i++)
                    {
                        outputValue[i] = (char)raw(BitBuffer.bitsASCII);
                    }
                    break;
                case 1:
                    for (var i = 0; i < length; i++)
                    {
                        outputValue[i] = (char)raw(BitBuffer.bitsLATIN1);
                    }
                    break;
                case 2:
                    for (var i = 0; i < length; i++)
                    {
                        outputValue[i] = (char)raw(BitBuffer.bitsLATINEXT);
                    }
                    break;
                default:
                    for (var i = 0; i < length; i++)
                    {
                        var needs16 = raw(1);
                        if (needs16 == 1)
                           outputValue[i] = (char)raw(BitBuffer.bitsUTF16);
                        else
                           outputValue[i] = (char)raw(BitBuffer.bitsASCII);
                    }
                    break;
            }

            return length;
        }
    }
}