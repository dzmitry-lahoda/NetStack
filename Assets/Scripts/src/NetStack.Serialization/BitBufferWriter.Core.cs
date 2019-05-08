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
    partial class BitBufferWriter<T> : RawBitWriter<u32ArrayMemory>, IBitBufferWriter
        where T : struct, ICompression<BitBufferWriter<T>> // https://en.wikipedia.org/wiki/Curiously_recurring_template_pattern
    {
        // true if has not capacity to write numberOfBits
        public bool CannotAdd(u32 numberOfBits) => BitsWritten + numberOfBits > totalNumberBits;

        /// <summary>
        /// Hom much bits can be yet written into buffer before it cannot add bits more.
        /// </summary>
        public u32 BitsAvailable => (u32)(totalNumberBits - BitsWritten);

        /// <summary>
        /// Adds value 7 bit encoded value.
        /// Store seven right bits, if more than 8 with 1, then set 1 to continue.
        /// </summary>
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public void u32(u32 value)
        {
            T encoder = default;
            encoder.u32(this, value);
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public void u16(u16 value)
        {
            T encoder = default;
            encoder.u16(this, value);
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public void u8(u8 value)
        {
            T encoder = default;
            encoder.u8(this, value);
        }
        
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public void i32(i32 value)
        {
            T encoder = default;
            encoder.i32(this, value);
        }

        /// <summary>
        /// Store value ZigZag encoded in number of bits.
        /// </summary>
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public void i32(i32 value, u8 numberOfBits)
        {
            T encoder = default;
            encoder.i32(this, value, numberOfBits);
        }
    }
}