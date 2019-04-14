using System.Runtime.CompilerServices;
using i32 = System.Int32;
using u32 = System.UInt32;
#if !(ENABLE_MONO || ENABLE_IL2CPP)
#else
using UnityEngine;
#endif

namespace NetStack.Serialization
{
    public interface IDecompression<T> where T : IRawReader
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        u32 u32(T b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        i32 i32(T b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        i32 decode(u32 value);
    }
}