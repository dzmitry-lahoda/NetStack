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
    partial class BitBufferReader<T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public i32 u8ArrayLengthPeek() => (i32)raw(config.ByteArrLengthBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public i32 u8(Span<u8> outValue) => u8(outValue, 0);

        /// <summary>
        /// Reads length prefixed array from buffer.
        /// </summary>
        /// <param name="outValue">Array to write into.</param>
        /// <param name="offset">Byte offset of output array to start write</param>
        /// <returns>Length of read array.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public i32 u8(Span<u8> outValue, i32 offset)
        {
            var length = (int)raw(config.ByteArrLengthBits);        
            if (totalNumberBits - BitsRead < length * 8)
                  throw InvalidOperation("The length for this read is bigger than bitbuffer");

            // 1                    1        0   OK
            // 1                    1        1   FAIL    
             if (length > outValue.Length - offset) 
                throw Argument(nameof(outValue), "The supplied byte array is too small for requested read");

            for (i32 index = offset; index < length; index++)
                outValue[index] = u8(); // TODO: can read faster if read by 4 bytes?
 
            return length;
        }  
    }
}