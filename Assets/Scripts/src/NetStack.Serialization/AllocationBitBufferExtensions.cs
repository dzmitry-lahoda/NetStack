using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Buffers;
using System.Diagnostics;
using System.Numerics;

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
            var data = pool.Rent(self.LengthWritten);
            self.ToSpan(data);
            return data;
        }               
    }
}