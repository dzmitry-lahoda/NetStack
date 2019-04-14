using System;
using System.Runtime.InteropServices;
using System.Text;
using static System.Except;
using i8 = System.SByte;
using i16 = System.Int16;
using i64 = System.Int64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;
using f32 = System.Single;
using f64 = System.Double;
using System.Buffers;
#if !(ENABLE_MONO || ENABLE_IL2CPP)
using System.Diagnostics;
using System.Numerics;
#else
using UnityEngine;
#endif

namespace NetStack.Serialization
{
    public interface IRawWriter
    {
        void raw(u32 value, int numberOfBits);
    }
}