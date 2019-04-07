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
    // core untyped data specific part of bit buffer
    partial class BitBuffer
    {
        private int chunkIndex;
        private uint[] chunks;
        private int totalNumChunks;
        private int totalNumberBits;  

        private int scratchUsedBits;
        private ulong scratch;
    }
}