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
        public BitBuffer Byte(byte value, int numberOfBits)
        {
            AddByte(value, numberOfBits);
            return this;
        }        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitBuffer Byte(byte value, byte min, byte max)
        {
            AddByte(value, min, max);
            return this;
        }     

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitBuffer SByte(sbyte value)
        {
            AddSByte(value);
            return this;
        }        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitBuffer SByte(sbyte value, int numberOfBits)
        {
            AddSByte(value, numberOfBits);
            return this;
        }        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitBuffer SByte(sbyte value, sbyte min, sbyte max)
        {
            AddSByte(value, min, max);
            return this;
        }     

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitBuffer Int(int value)
        {
            AddInt(value);
            return this;
        }        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitBuffer Int(int value, int numberOfBits)
        {
            AddInt(value, numberOfBits);
            return this;
        }        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitBuffer Int(int value, int min, int max)
        {
            AddInt(value, min, max);
            return this;
        }  

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitBuffer UInt(uint value)
        {
            AddUInt(value);
            return this;
        }        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitBuffer UInt(uint value, int numberOfBits)
        {
            AddUInt(value, numberOfBits);
            return this;
        }        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitBuffer UInt(uint value, uint min, uint max)
        {
            AddUInt(value, min, max);
            return this;
        }  

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitBuffer Short(short value)
        {
            AddShort(value);
            return this;
        }        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitBuffer Short(short value, int numberOfBits)
        {
            AddShort(value, numberOfBits);
            return this;
        }        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitBuffer Short(short value, short min, short max)
        {
            AddShort(value, min, max);
            return this;
        }                              

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitBuffer UShort(ushort value)
        {
            AddUShort(value);
            return this;
        }        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitBuffer UShort(ushort value, int numberOfBits)
        {
            AddUShort(value, numberOfBits);
            return this;
        }        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitBuffer UShort(ushort value, ushort min, ushort max)
        {
            AddUShort(value, min, max);
            return this;
        }  


        // TODO: cover all methods
    }
}