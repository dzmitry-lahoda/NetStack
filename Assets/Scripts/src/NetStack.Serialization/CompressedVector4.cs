using System;
using System.Runtime.CompilerServices;

#if !(ENABLE_MONO || ENABLE_IL2CPP)
using System.Numerics;
#else
using UnityEngine;
#endif


namespace NetStack.Compression
{
    public struct CompressedVector4
    {
        public uint x;
        public uint y;
        public uint z;
        public uint w;

        public CompressedVector4(uint x, uint y, uint z, uint w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
    }
}