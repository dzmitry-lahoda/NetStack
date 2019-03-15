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
    /// Unmanaged read and write.
    /// Please be aware that padding (empty spaces in data alignment) are also written.
    /// Please be aware that small numbers if aligned not properly will end up sometimes in large space because became large numbers.
    /// </summary>
    public static class BitBufferExtensions
    {
        /// <summary>
        /// Writes struct.
        /// </summary>
        /// <typeparam name="T">Any struct with no references to managed heap.</typeparam>
        /// <param name="readonlyValue">The readonly value.</param>
        [MethodImpl(256)]
        public static BitBuffer AddBlock<T>(this BitBuffer self, in T value)
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
                        self.AddUInt(reinterpretedValue);
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

            return self;
        }

        private static void WriteSmallUnmanaged(this BitBuffer self, ref byte value, int size)
        {
            if (size == 1)
            {
                self.AddByte(value);
            }
            else if (size == 2)
            {
                ref var reinterpretedValue = ref Unsafe.As<byte, ushort>(ref value);
                self.AddUShort(reinterpretedValue);
            }
            else if (size == 3)
            {
                ref var reinterpretedValue1 = ref Unsafe.As<byte, ushort>(ref value);
                self.AddUShort(reinterpretedValue1);
                ref var reinterpretedValue2 = ref Unsafe.Add(ref value, 2);
                self.AddByte(reinterpretedValue2);
            }
            else if (size == 4)
            {
                ref var reinterpretedValue = ref Unsafe.As<byte, uint>(ref value);
                self.AddUInt(reinterpretedValue);
            }
        }

        /// <summary>
        /// Reads one element from stream.
        /// </summary>
        /// <typeparam name="T">Element with predefined size.</typeparam>
        /// <returns>The value.</returns>
        public static T ReadBlock<T>(this BitBuffer self)
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
                        reinterpretedValue = self.ReadUInt();
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

        private static void ReadSmallUnmanaged(this BitBuffer self, ref byte value, int size)
        {
            if (size == 1)
            {
                value = self.ReadByte();
            }
            else if (size == 2)
            {
                ref var reinterpretedValue = ref Unsafe.As<byte, ushort>(ref value);
                reinterpretedValue = self.ReadUShort();
            }
            else if (size == 3)
            {
                ref var reinterpretedValue1 = ref Unsafe.As<byte, ushort>(ref value);
                reinterpretedValue1 = self.ReadUShort();
                ref var reinterpretedValue2 = ref Unsafe.Add(ref value, 2);
                reinterpretedValue2 = self.ReadByte();
            } 
        }
    }
}