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
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Numerics;

namespace NetStack.Serialization
{
    /// <summary>
    /// Access to memory to read and write. 
    /// Looses several percents of performance against raw zero index array, but allows:
    /// - to write into span starting from non zero
    /// - native memory usage
    /// - zero allocation and copy by cast u8 array into u32
    /// </summary>
    public interface IMemory<T> where T: unmanaged
    {
        T this[u16 index]
        {
            [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
            get;
            [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
            set;
        }

        u16 Length
        {
            [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
            get;
        }
    }
}