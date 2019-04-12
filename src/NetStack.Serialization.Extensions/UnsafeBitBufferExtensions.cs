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
    /// <summary>
    /// Unmanaged unsafe read and write of explicit layouted structures for fast prototyping.
    /// </summary>
    /// <remarks>
    /// Please be aware that:
    /// - padding (empty spaces in data alignment) are also written.
    /// - small numbers if aligned not properly will end up sometimes in large space because became large numbers
    /// - does not works if remote and local of different endianess.
    /// - sequential layout (with different padding) may not work. 
    /// - auto layout will not work.
    /// </remarks>
    public static class UnsafeBitBufferExtensions
    {
        /// <summary>
        /// Unmanaged unsafe write of explicit layouted structure into buffer.
        /// </summary>
        /// <typeparam name="T">Any struct with no references to managed heap.</typeparam>
        /// <param name="value">The value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void block<T>(this BitBufferWriter<SevenBit> self, in T value)
            where T : unmanaged
        {
            var size = Unsafe.SizeOf<T>();
            if (size <= 4)
            {
                ref var last = ref Unsafe.As<T, byte>(ref Unsafe.AsRef(in value));
                self.WriteSmallUnmanaged(ref last, size);
            }
            else
            {
                ref var reinterpretedValue = ref Unsafe.As<T, uint>(ref Unsafe.AsRef(in value));
                while (size > 0)
                {
                    if (size > 4)
                    {
                        self.u32(reinterpretedValue);
                        size -= 4;
                        reinterpretedValue = ref Unsafe.Add(ref reinterpretedValue, 1);
                    }
                    else
                    {
                        ref var last = ref Unsafe.As<uint, byte>(ref reinterpretedValue);
                        self.WriteSmallUnmanaged(ref last, size);
                        size = 0;
                    }
                }
            }
        }

        private static void WriteSmallUnmanaged(this BitBufferWriter<SevenBit> self, ref byte value, int size)
        {
            if (size == 1)
            {
                self.u8(value);
            }
            else if (size == 2)
            {
                ref var reinterpretedValue = ref Unsafe.As<byte, ushort>(ref value);
                self.u16(reinterpretedValue);
            }
            else if (size == 3)
            {
                ref var reinterpretedValue1 = ref Unsafe.As<byte, ushort>(ref value);
                self.u16(reinterpretedValue1);
                ref var reinterpretedValue2 = ref Unsafe.Add(ref value, 2);
                self.u8(reinterpretedValue2);
            }
            else if (size == 4)
            {
                ref var reinterpretedValue = ref Unsafe.As<byte, uint>(ref value);
                self.u32(reinterpretedValue);
            }
        }

        /// <summary>
        /// Unmanaged unsafe read of explicit layouted structure from buffer.
        /// </summary>
        /// <typeparam name="T">Element with predefined size.</typeparam>
        /// <returns>The value.</returns>
        public static T block<T>(this BitBufferReader<SevenBitRe> self)
           where T : unmanaged
        {
            var size = Unsafe.SizeOf<T>();
            T value = default;
            if (size < 4)
            {
                ref var last = ref Unsafe.As<T, byte>(ref Unsafe.AsRef(in value));
                self.ReadSmallUnmanaged(ref last, size);
            }
            else
            {
                ref var reinterpretedValue = ref Unsafe.As<T, uint>(ref Unsafe.AsRef(in value));
                while (size > 0)
                {
                    if (size >= 4)
                    {
                        reinterpretedValue = self.u32();
                        size -= 4;
                        reinterpretedValue = ref Unsafe.Add(ref reinterpretedValue, 1);
                    }
                    else
                    {
                        ref var last = ref Unsafe.As<uint, byte>(ref reinterpretedValue);
                        self.ReadSmallUnmanaged(ref last, size);
                        size = 0;
                    }
                }
            }

            return value;
        }

        private static void ReadSmallUnmanaged(this BitBufferReader<SevenBitRe> self, ref byte value, int size)
        {
            if (size == 1)
            {
                value = self.u8();
            }
            else if (size == 2)
            {
                ref var reinterpretedValue = ref Unsafe.As<byte, ushort>(ref value);
                reinterpretedValue = self.u16();
            }
            else if (size == 3)
            {
                ref var reinterpretedValue1 = ref Unsafe.As<byte, ushort>(ref value);
                reinterpretedValue1 = self.u16();
                ref var reinterpretedValue2 = ref Unsafe.Add(ref value, 2);
                reinterpretedValue2 = self.u8();
            } 
        }
    }
}