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

        // TODO: 
        // hoffman/managed protobuf/GRPC/MessagePack/raw bytes into buffer via Unsafe/via ObjectLayoutInspector and other slow stuff to get if should migrate
        [Benchmark]
        public void ProtoBufferSerialization()
        {
        }
        
        [Benchmark]
        public void MessagePackSerialization()
        {
        }        
    }
}