using System;
using System.Runtime.CompilerServices;

#if !(ENABLE_MONO || ENABLE_IL2CPP)
	using System.Numerics;
#else
using UnityEngine;
#endif

namespace NetStack.Compression
{
    public struct CompressedVector2
    {
        public uint x;
        public uint y;

        public CompressedVector2(uint x, uint y)
        {
            this.x = x;
            this.y = y;
        }
    }
}