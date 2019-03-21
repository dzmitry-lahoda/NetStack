﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
#if !(ENABLE_MONO || ENABLE_IL2CPP)
using System.Diagnostics;
using System.Numerics;
#else
using UnityEngine;
#endif

// Until migration to .NET Standard 2.1
using BitOperations = System.Numerics.BitOperations;

namespace NetStack.Serialization
{
    /// <summary>
    /// Slightly slower, but more convenient to write.
    /// </summary>
    public partial class BitBuffer
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitBuffer Bool(bool value)
        {
            AddBool(value);
            return this;
        }    

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitBuffer Byte(byte value)
        {
            AddByte(value);
            return this;
        }        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitBuffer Byte(byte value, int numBits)
        {
            AddByte(value, numBits);
            return this;
        }        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitBuffer Byte(byte value, byte min, byte max)
        {
            AddByte(value, min, max);
            return this;
        }     

        // TODO: cover all methods
    }
}