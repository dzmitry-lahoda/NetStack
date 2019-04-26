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
    partial class BitBufferWriter<T>
    {
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public void u8(ReadOnlySpan<u8> value)
        {
            if (value.Length > config.U8SpanLengthMax)
                Throw.Argument($"Byte array too big, raise the {nameof(config.U8SpanBitsLength)} value or split the array.");

            if (value.Length + 9 > (totalNumberBits - BitsWritten))
                Throw.InvalidOperation("Byte array too big for buffer.");

            raw((u32)value.Length, config.U8SpanBitsLength);
            for (var index = 0; index < value.Length; index++)
                u8(value[index]);
        }
    }
}