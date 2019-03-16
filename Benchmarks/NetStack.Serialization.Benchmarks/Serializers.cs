using System;
using System.Numerics;
using BenchmarkDotNet;
using BenchmarkDotNet.Attributes;
using NetStack.Serialization;

namespace NetStack.Serialization
{
    [CoreJob]
    public class Serializers
    {
        
        [Benchmark]
        public void BitBufferSerialization()
        {

        }

        // TODO: because frequently used in GRPC and other slow stuff to get if should migrate
        [Benchmark]
        public void ProtoBufferSerialization()
        {
        }
    }
}