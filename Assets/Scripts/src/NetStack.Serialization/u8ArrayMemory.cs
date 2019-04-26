using System;
using i32 = System.Int32;
using u8 = System.Byte;
using u32 = System.UInt32;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace NetStack.Serialization
{
    // u8 array pinned and casted into u32 array
    public unsafe struct u8ArrayMemory : IMemory<u32>
    {
        public u8ArrayMemory(u8[] array) 
        {
             handle = array.AsMemory().Pin();
             pointer = (u32*)handle.Pointer;      
             length = array.Length/4;       
        }

        private MemoryHandle handle;
        private unsafe uint* pointer;
        private int length;

        public u32 this[i32 index]
        {
            [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
            get => pointer[index];
            [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
            set => pointer[index] = value;
        }

        public i32 Length
        {
            [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
            get => length;
        }
    }
}