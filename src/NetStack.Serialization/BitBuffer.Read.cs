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
    partial class BitBuffer
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool PeekBool() 
        {            
            var tmp1 = scratchUsedBits;
            var tmp2 = chunkIndex;
            var tmp3 = scratch;
            var result = ReadBool();
            scratchUsedBits = tmp1;
            chunkIndex = tmp2;
            scratch = tmp3;
            return true;
        }
    }
}