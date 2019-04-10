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
    public static class BitBufferExtensionsFluent
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitBuffer Bool(this BitBuffer self, bool value)
        {
            self.AddBool(value);
            return self;
        }    

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitBuffer Byte(this BitBuffer self, byte value)
        {
            self.AddByte(value);
            return self;
        }        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitBuffer Byte(this BitBuffer self, byte value, int numberOfBits)
        {
            self.AddByte(value, numberOfBits);
            return self;
        }        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitBuffer Byte(this BitBuffer self, byte value, byte min, byte max)
        {
            self.AddByte(value, min, max);
            return self;
        }     

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitBuffer SByte(this BitBuffer self, sbyte value)
        {
            self.AddSByte(value);
            return self;
        }        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitBuffer SByte(this BitBuffer self, sbyte value, int numberOfBits)
        {
            self.AddSByte(value, numberOfBits);
            return self;
        }        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitBuffer SByte(this BitBuffer self, sbyte value, sbyte min, sbyte max)
        {
            self.AddSByte(value, min, max);
            return self;
        }     

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitBuffer Int(this BitBuffer self, int value)
        {
            self.AddInt(value);
            return self;
        }        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitBuffer Int(this BitBuffer self, int value, int numberOfBits)
        {
            self.AddInt(value, numberOfBits);
            return self;
        }        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitBuffer Int(this BitBuffer self, int value, int min, int max)
        {
            self.AddInt(value, min, max);
            return self;
        }  

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitBuffer UInt(this BitBuffer self, uint value)
        {
            self.AddUInt(value);
            return self;
        }        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitBuffer UInt(this BitBuffer self, uint value, int numberOfBits)
        {
            self.AddUInt(value, numberOfBits);
            return self;
        }        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitBuffer UInt(this BitBuffer self, uint value, uint min, uint max)
        {
            self.AddUInt(value, min, max);
            return self;
        }  

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitBuffer Short(this BitBuffer self, short value)
        {
            self.AddShort(value);
            return self;
        }        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitBuffer Short(this BitBuffer self, short value, int numberOfBits)
        {
            self.AddShort(value, numberOfBits);
            return self;
        }        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitBuffer Short(this BitBuffer self, short value, short min, short max)
        {
            self.AddShort(value, min, max);
            return self;
        }                              

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitBuffer UShort(this BitBuffer self, ushort value)
        {
            self.AddUShort(value);
            return self;
        }        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitBuffer UShort(this BitBuffer self, ushort value, int numberOfBits)
        {
            self.AddUShort(value, numberOfBits);
            return self;
        }        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitBuffer UShort(this BitBuffer self, ushort value, ushort min, ushort max)
        {
            self.AddUShort(value, min, max);
            return self;
        }  


        // TODO: cover all methods
    }
}