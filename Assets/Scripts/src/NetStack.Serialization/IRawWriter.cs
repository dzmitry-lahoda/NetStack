using System;
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
using System.Buffers;
using System.Runtime.CompilerServices;

namespace NetStack.Serialization
{
    public interface IRawWriter
    {
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        void u32(u32 value, i32 numberOfBits);

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        void u8(u8 value);

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        void b(bool value);
    }
}