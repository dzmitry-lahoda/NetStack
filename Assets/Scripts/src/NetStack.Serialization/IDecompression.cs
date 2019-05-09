using System.Runtime.CompilerServices;
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
    public interface IDecompression<in T> where T : IRawReader
    {
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        u32 u32(T b);

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        i32 i32(T b);

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        u16 u16(T b);   

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        u8 u8(T b);        

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        i32 decode(u32 value);

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        i32 i32(T b, u8 numberOfBits);
    }
}