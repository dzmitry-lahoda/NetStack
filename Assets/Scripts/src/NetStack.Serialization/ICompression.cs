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
    public interface ICompression<in T> where T : IRawWriter
    {
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        void u32(T b, u32 value);

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        void u16(T b, u16 value);        

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        void i32(T b, i32 value);

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        u32 encode(i32 value);

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        void i32(T b, i32 value, u8 numberOfBits);
        
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        void u8(T b, u8 value);
    }
}