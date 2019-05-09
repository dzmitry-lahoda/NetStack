using System;
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
    /// <summary>
    /// Store whole update value only if different from baseline prefixed by bool.
    /// </summary>
    public static class BDiffBitBufferExtensions
    {
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public static void f32BDiff<T>(this T self, f32 baseline, f32 update)
        where T : IBitBufferWriter
        {
            if (baseline != update)
            {
                self.b(true);
                self.f32(update);
            }
            else
                self.b(false);
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public static void f32BDiff<T>(this T self, f32 baseline, f32 update, f32 min, f32 max, f32 precision)
              where T : IBitBufferWriter
        {
            if (baseline != update)
            {
                self.b(true);
                self.f32(update, min, max, precision);
            }
            else
                self.b(false);
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public static f32 f32BDiff<TReader>(this TReader self, f32 baseline)
            where TReader : IBitBufferReader
            =>
            self.b() ? self.f32() : baseline;

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public static f32 f32BDiff<TReader>(this TReader self, f32 baseline, f32 min, f32 max, f32 precision) where TReader : IBitBufferReader
            =>
            self.b() ? self.f32(min, max, precision) : baseline;

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public static void i32BDiff<T>(this T self, i32 baseline, i32 update)
              where T : IBitBufferWriter
        {
            if (baseline != update)
            {
                self.b(true);
                self.i32(update);
            }
            else
                self.b(false);
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public static i32 i32BDiff<TReader>(this TReader self, i32 baseline) where TReader : IBitBufferReader
            =>
            self.b() ? self.i32() : baseline;

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public static i32 i32BDiff<TReader>(this TReader self, i32 baseline, i32 min, i32 max) where TReader : IBitBufferReader
            =>
            self.b() ? self.i32(min, max) : baseline;

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public static void i32BDiff<T>(this T self, i32 baseline, i32 update, i32 min, i32 max)
              where T : IBitBufferWriter
        {
            if (baseline != update)
            {
                self.b(true);
                self.i32(update, min, max);
            }
            else
                self.b(false);
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public static void u32BDiff<T>(this T self, u32 baseline, u32 update)
                where T : IBitBufferWriter
        {
            if (baseline != update)
            {
                self.b(true);
                self.u32(update);
            }
            else
                self.b(false);
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public static u32 u32BDiff<TReader>(this TReader self, u32 baseline) where TReader : IBitBufferReader
            =>
            self.b() ? self.u32() : baseline;

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public static u32 u32BDiff<TReader>(this TReader self, u32 baseline, u32 min, u32 max) where TReader : IBitBufferReader
            =>
            self.b() ? self.u32(min, max) : baseline;

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public static void u32BDiff<T>(this T self, u32 baseline, u32 update, u32 min, u32 max)
              where T : IBitBufferWriter
        {
            if (baseline != update)
            {
                self.b(true);
                self.u32(update, min, max);
            }
            else
                self.b(false);
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public static void u64BDiff<T>(this T self, u64 baseline, u64 update)
              where T : IBitBufferWriter
        {
            if (baseline != update)
            {
                self.b(true);
                self.u64(update);
            }
            else
                self.b(false);
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public static u64 u64BDiff<TReader>(this TReader self, u64 baseline) where TReader : IBitBufferReader
            =>
            self.b() ? self.u64() : baseline;


        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public static void f64BDiff<T>(this T self, f64 baseline, f64 update)
              where T : IBitBufferWriter
        {
            if (baseline != update)
            {
                self.b(true);
                self.f64(update);
            }
            else
                self.b(false);
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public static f64 f64BDiff<TReader>(this TReader self, f64 baseline) where TReader : IBitBufferReader
            =>
            self.b() ? self.f64() : baseline;

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public static void i16BDiff<T>(this T self, i16 baseline, i16 update)
              where T : IBitBufferWriter
        {
            if (baseline != update)
            {
                self.b(true);
                self.i16(update);
            }
            else
                self.b(false);
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public static i16 i16BDiff<TReader>(this TReader self, i16 baseline) where TReader : IBitBufferReader
            =>
            self.b() ? self.i16() : baseline;


        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public static i16 i16BDiff<TReader>(this TReader self, i16 baseline, i16 min, i16 max) where TReader : IBitBufferReader
            =>
            self.b() ? self.i16(min, max) : baseline;

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public static void i16BDiff<T>(this T self, i16 baseline, i16 update, i16 min, i16 max)
              where T : IBitBufferWriter
        {
            if (baseline != update)
            {
                self.b(true);
                self.i16(update, min, max);
            }
            else
                self.b(false);
        }


        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public static void i8BDiff<T>(this T self, i8 baseline, i8 update)
              where T : IBitBufferWriter
        {
            if (baseline != update)
            {
                self.b(true);
                self.i8(update);
            }
            else
                self.b(false);
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public static i8 i8BDiff<TReader>(this TReader self, i8 baseline) where TReader : IBitBufferReader
            =>
            self.b() ? self.i8() : baseline;

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public static i8 i8BDiff<TReader>(this TReader self, i8 baseline, i8 min, i8 max) where TReader : IBitBufferReader
            =>
            self.b() ? self.i8(min, max) : baseline;

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public static void i8BDiff<T>(this T self, i8 baseline, i8 update, i8 min, i8 max)
              where T : IBitBufferWriter
        {
            if (baseline != update)
            {
                self.b(true);
                self.i8(update, min, max);
            }
            else
                self.b(false);
        }


        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public static void u8BDiff<T>(this T self, u8 baseline, u8 update)
              where T : IBitBufferWriter
        {
            if (baseline != update)
            {
                self.b(true);
                self.u8(update);
            }
            else
                self.b(false);
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public static u8 u8BDiff<TReader>(this TReader self, u8 baseline) where TReader : IBitBufferReader
            =>
            self.b() ? self.u8() : baseline;


        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public static u8 u8BDiff<TReader>(this TReader self, u8 baseline, u8 min, u8 max) where TReader : IBitBufferReader
            =>
            self.b() ? self.u8(min, max) : baseline;

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public static void u8BDiff<T>(this T self, u8 baseline, u8 update, u8 min, u8 max)
              where T : IBitBufferWriter
        {
            if (baseline != update)
            {
                self.b(true);
                self.u8(update, min, max);
            }
            else
                self.b(false);
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public static void u16BDiff<T>(this T self, u16 baseline, u16 update)
              where T : IBitBufferWriter
        {
            if (baseline != update)
            {
                self.b(true);
                self.u16(update);
            }
            else
                self.b(false);
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public static u16 u16BDiff<TReader>(this TReader self, u16 baseline) where TReader : IBitBufferReader
            =>
            self.b() ? self.u16() : baseline;

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public static u16 u16BDiff<TReader>(this TReader self, u16 baseline, u16 min, u16 max) where TReader : IBitBufferReader
            =>
            self.b() ? self.u16(min, max) : baseline;

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public static void u16BDiff<T>(this T self, u16 baseline, u16 update, u16 min, u16 max)
              where T : IBitBufferWriter
        {
            if (baseline != update)
            {
                self.b(true);
                self.u16(update, min, max);
            }
            else
                self.b(false);
        }


        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public static void i64BDiff<T>(this T self, i64 baseline, i64 update)
              where T : IBitBufferWriter
        {
            if (baseline != update)
            {
                self.b(true);
                self.i64(update);
            }
            else
                self.b(false);
        }

        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public static i64 i64BDiff<TReader>(this TReader self, i64 baseline) where TReader : IBitBufferReader
            =>
            self.b() ? self.i64() : baseline;
    }
}