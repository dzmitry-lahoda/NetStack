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
using System.Runtime.CompilerServices;
using System;

namespace NetStack.Serialization
{
    public struct u32ArrayMemory : IMemory<u32>
    {
        private u32[] data;
        
        public u32ArrayMemory(u32[] data) 
        {
            if (data.Length > u16.MaxValue)
            {
                var msg = $"Can work only with arrays of {u16.MaxValue} maximal length, but {data.Length} was provided. To work with larger arrays, please use {typeof(IMemory<u32>)} pointing to chunk";
                Throw.ArgumentOutOfRange(msg, nameof(data));
            }
                
            this.data = data;
        } 

        public u32 this[u16 index]
        {
            [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
            get => data[index];
            [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
            set => data[index] = value;
        }

        public u16 Length
        {
            [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
            get => (u16)data.Length;
        }
    }
}