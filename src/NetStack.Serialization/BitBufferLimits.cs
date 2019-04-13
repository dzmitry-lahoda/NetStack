using System;
using System.Runtime.CompilerServices;
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
#if !(ENABLE_MONO || ENABLE_IL2CPP)
using System.Diagnostics;
using System.Numerics;
#else
using UnityEngine;
#endif


namespace NetStack.Serialization
{
    ///<summary>
    /// Constants to help proper split messages into fragments.
    /// </summary>
    /// <seealso href="https://en.wikipedia.org/wiki/Maximum_transmission_unit"/>
    /// <seealso href="https://tools.ietf.org/html/rfc5389/"/>
    /// <seealso href="https://tools.ietf.org/html/rfc1191"/>
    public static class BitBufferLimits
    {
        public const i32 MtuIeee802Dot3 = 1492;

        public const i32 MtuIeee802 = 508;
    }
}
