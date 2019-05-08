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
    partial class BitBufferReader<T> : IRawReader
         where T:struct, IDecompression<BitBufferReader<T>> 
    {        
        /// <summary>
        /// Reads 7 bit encoded uint value.
        /// </summary>
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public u32 u32()
        {
            T decoder = default;
            return decoder.u32(this);
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public i32 i32()
        {
            T dencoder = default;
            return dencoder.i32(this);
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public i32 i32(i32 numberOfBits)
        {
            T encoder = default;
            return encoder.i32(this, numberOfBits);
        }
    }
}