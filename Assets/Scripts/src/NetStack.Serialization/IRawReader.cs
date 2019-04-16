using System.Runtime.CompilerServices;
using i32 = System.Int32;
using u32 = System.UInt32;

namespace NetStack.Serialization
{
    public interface IRawReader
    {
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        u32 raw(i32 numberOfBits);
    }
}