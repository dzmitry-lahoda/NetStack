using i32 = System.Int32;
using u32 = System.UInt32;
#if !(ENABLE_MONO || ENABLE_IL2CPP)
#else
using UnityEngine;
#endif

namespace NetStack.Serialization
{
    public interface IRawReader
    {
        u32 raw(i32 numberOfBits);
    }
}