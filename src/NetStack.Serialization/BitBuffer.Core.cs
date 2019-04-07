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
        private uint[] chunks;
        
        private int totalNumChunks;
        
        private int totalNumberBits;  

        internal uint[] Chunks
        {
            set 
            {
                chunks = value;
                totalNumChunks = chunks.Length;
                totalNumberBits = totalNumChunks * 8 * Unsafe.SizeOf<uint>();   
            }
        }

        // bit index
        private int chunkIndex;
        private int scratchUsedBits;
        
        // last partially read value
        private ulong scratch;

    }
}