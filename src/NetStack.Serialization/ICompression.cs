using System.Runtime.CompilerServices;
using u32 = System.UInt32;
#if !(ENABLE_MONO || ENABLE_IL2CPP)
#else
using UnityEngine;
#endif

namespace NetStack.Serialization
{
    public interface ICompression<T> where T: IRawWriter
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void u32(T b, u32 value); 

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        uint zigzag(int value);
    }

    public interface IDecompression<T> where T: IRawReader
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        u32 u32(T b); 
    }    
}