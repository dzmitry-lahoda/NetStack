using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Buffers;
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
    public static class AllocationBitBufferExtensions
    {
        [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
        public unsafe static string String<TReader>(this TReader self)
            where TReader : IBitBufferReader
        {
            var size = MemoryPool<char>.Shared.Rent(self.Options.CharSpanLengthMax);
            var length = self.c(size.Memory.Span);
            using (var pin = size.Memory.Pin())
                return new String((char*)pin.Pointer,0, (int)length);         
        }

        public static void c<TReader>(this TReader self, string value) 
               where TReader: IBitBufferWriter
            => 
            self.c(value.AsSpan());

        /// <summary>
        /// Dot not use for production. GC allocated array.
        /// </summary>
        /// <returns></returns>
        public static byte[] ToArray<T>(this T self) 
          where T: RawBitWriter<u32ArrayMemory>, IBitBufferWriter
        {
            var data = new byte[self.LengthWritten];
            self.ToSpan(data);
            return data;
        }

        /// <summary>
        /// Rents array 
        /// </summary>
        public static byte[] ToArray<T>(this T self, ArrayPool<byte> pool = null) 
                  where T: RawBitWriter<u32ArrayMemory>, IBitBufferWriter
        {
            pool = pool ?? ArrayPool<byte>.Shared;
            var data = pool.Rent((i32)self.LengthWritten);
            self.ToSpan(data);
            return data;
        }               
    }
}