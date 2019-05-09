using System;
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
using System.Buffers;
using System.Runtime.CompilerServices;

namespace NetStack.Serialization
{
    // u8 array pinned and casted into u32 array
    public unsafe struct u8ArrayMemory : IMemory<u32>
    {
        public u8ArrayMemory(u8[] data) 
        {
            if (data.Length > u16.MaxValue * 4)
            {
                var msg = $"Can work only with arrays of {u16.MaxValue*4} maximal length, but {data.Length} was provided. To work with larger arrays, please use {typeof(IMemory<u32>)} pointing to chunk";
                Throw.ArgumentOutOfRange(msg, nameof(data));
            }

             handle = data.AsMemory().Pin();
             pointer = (u32*)handle.Pointer;      
             length = (u16)(data.Length/4);       
        }

        private MemoryHandle handle;
        private unsafe uint* pointer;
        private u16 length;

        public u32 this[u16 index]
        {
            [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
            get => pointer[index];
            [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
            set => pointer[index] = value;
        }

        public u16 Length
        {
            [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
            get => length;
        }
    }
}