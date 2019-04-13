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
        public int u8(byte[] outValue) => u8(outValue, 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int u8(byte[] outValue, int offset)
        {
            outValue.NotNull();

            var length = (int)raw(config.ByteArrLengthBits);        
            if (totalNumberBits - BitsRead < length * 8)
                  throw InvalidOperation("The length for this read is bigger than bitbuffer");

            if (length > outValue.Length + offset) 
                throw Argument(nameof(outValue), "The supplied byte array is too small for requested read");

            for (int index = offset; index < length; index++)
                outValue[index] = u8(); // TODO: faste way to read?
 
            return length;
        }  
    }
}