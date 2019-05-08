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

namespace NetStack.Serialization
{
    public struct RawEncoding<TMemory> : ICompression<RawBitWriter<TMemory>>
        where TMemory: struct, IMemory<u32>
    {
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public void i32(RawBitWriter<TMemory> b, int value) => b.u32((u32)value, 32);
        
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public void u32(RawBitWriter<TMemory> b, u32 value) => b.u32(value, 32);

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public u32 encode(i32 value) => (u32)value;

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public void i32(RawBitWriter<TMemory> b, i32 value, i32 numberOfBits) => i32(b, value);
    }
}