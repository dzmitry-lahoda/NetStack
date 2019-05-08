﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;
using System.Numerics;

using i8 = System.SByte;
using i16 = System.Int16;
using i32 = System.Int32;
using i64 = System.Int64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;
using f32 = System.Single;
using f64 = System.Double;


namespace NetStack.Serialization
{
    public interface IBitBufferWriter : IRawWriter

    {
        i32 LengthWritten {get;}
        
        void f32(f32 update);
        
        void f32(f32 update, f32 min, f32 max, f32 precision);

        void f64(f64 update);

        void u16(u16 value);

        void u16(u16 value, u16 min, u16 max);

        void i16(i16 value);

        void i16(i16 value, i16 min, i16 max);

        void i32(i32 value, i32 min, i32 max);

        void i32(i32 value);

        void u32(u32 value);

        void u32(u32 value, u32 min, u32 max);

        void i64(i64 value);

        void u64(u64 value);

        void i8(i8 value);

        void i8(i8 value, i8 min, i8 max);

        void u8(u8 value, u8 min, u8 max);

        void c(ReadOnlySpan<char> value);

        i32 ToSpan(Span<u8> data);
    }
}