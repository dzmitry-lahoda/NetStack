﻿using System;
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
using System.Diagnostics;
using System.Numerics;

namespace NetStack.Serialization
{
    partial class BitBufferWriter<T>
    {
        /// <summary>
        /// Adds span of chars into buffer.
        /// </summary>
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public void c(ReadOnlySpan<char> value)
        {
            if (value.Length > config.CharSpanLengthMax)
                Throw.ArgumentOutOfRange($"String too long, raise the {nameof(config.CharSpanBitsLength)} value or split the string.");

            var length = value.Length;

            if (length * 17 + 10 > (totalNumberBits - BitsWritten)) // possible overflow
            {
                if (BitBuffer.BitsRequired(value, length) > (totalNumberBits - BitsWritten))
                    Throw.ArgumentOutOfRange("String would not fit in bitstream.");
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

            u32((u32)codePage, BitBuffer.codePageBitsRequired);
            u32((u32)length, config.CharSpanBitsLength);

            switch (codePage)
            {
                case CodePage.Ascii:
                    for (var i = 0; i < length; i++)
                    {
                        u32(value[i], BitBuffer.bitsASCII);
                    }
                    break;
                case CodePage.Latin1:
                    for (var i = 0; i < length; i++)
                    {
                        u32(value[i], BitBuffer.bitsLATIN1);
                    }
                    break;
                case CodePage.LatinExtended:
                    for (var i = 0; i < length; i++)
                    {
                        u32(value[i], BitBuffer.bitsLATINEXT);
                    }
                    break;
                default:
                    for (var i = 0; i < length; i++)
                    {
                        if (value[i] > 127)
                        {
                            u32(1, 1);
                            u32(value[i], BitBuffer.bitsUTF16);
                        }
                        else
                        {
                            u32(0, 1);
                            u32(value[i], BitBuffer.bitsASCII);
                        }
                    }
                    break;
            }
        }
    }
}