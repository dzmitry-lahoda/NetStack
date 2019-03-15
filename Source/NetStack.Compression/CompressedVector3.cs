using System;
using System.Runtime.CompilerServices;

#if !(ENABLE_MONO || ENABLE_IL2CPP)
using System.Numerics;
#else
	using UnityEngine;
#endif

namespace NetStack.Compression
{
    public struct CompressedVector3 {
		public uint x;
		public uint y;
		public uint z;

		public CompressedVector3(uint x, uint y, uint z) {
			this.x = x;
			this.y = y;
			this.z = z;
		}
	}
}