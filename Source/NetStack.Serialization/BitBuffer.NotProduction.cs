using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
#if !(ENABLE_MONO || ENABLE_IL2CPP)
using System.Diagnostics;
using System.Numerics;
#else
using UnityEngine;
#endif

namespace NetStack.Serialization
{
    // GC allocated 
    partial class BitBuffer
    {
        public byte[] ToArray()
        {
            var data = new byte[Length];
            ToArray(data);
            return data;
        }
    }
}