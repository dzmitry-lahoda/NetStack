using i32 = System.Int32;
using u32 = System.UInt32;
using System.Runtime.CompilerServices;

namespace NetStack.Serialization
{
    public struct u32ArrayMemory : IMemory<u32>
    {
        private u32[] data;
        
        public u32ArrayMemory(u32[] data) => this.data = data;

        public u32 this[i32 index]
        {
            [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
            get => data[index];
            [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
            set => data[index] = value;
        }

        public i32 Length
        {
            [MethodImpl(Optimization.AggressiveInliningAndOptimization)]
            get => data.Length;
        }
    }
}