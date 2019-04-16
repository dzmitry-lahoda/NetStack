using System.Runtime.CompilerServices;
using i32 = System.Int32;
using u32 = System.UInt32;

namespace NetStack.Serialization
{
    public interface IDecompression<T> where T : IRawReader
    {
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        u32 u32(T b);

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        i32 i32(T b);

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        i32 decode(u32 value);

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        i32 i32(T b, i32 numberOfBits);
    }
}