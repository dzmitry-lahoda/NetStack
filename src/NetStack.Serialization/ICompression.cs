using System.Runtime.CompilerServices;
using u32 = System.UInt32;
#if !(ENABLE_MONO || ENABLE_IL2CPP)
#else
using UnityEngine;
#endif

namespace NetStack.Serialization
{
    public interface ICompression<T> where T: IRaw
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void u32(T b, u32 value); 
    }

    public interface IDecompression<T> where T: IRaw2
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        u32 u32(T b); 
    }    
}