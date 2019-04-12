// using System;
// using System.Numerics;
// using BenchmarkDotNet;
// using BenchmarkDotNet.Attributes;
// using NetStack.Serialization;

// namespace NetStack.Serialization
// {
//     [CoreJob]
//     public class Generics
//     {
//         [Params(100000)]
//         public int N;
//         private BitBufferWriter buffer1;
//         private GenricBitBufferWriter<SevenBit> buffer2;
//         private GenricBitBufferWriter<NoEncoding> buffer3;

//         [IterationSetup]
//         public void GlobalSetup()
//         {
//             buffer1 = new BitBufferWriter<SevenBit>(20_000_000);
//             buffer2 = new GenricBitBufferWriter<SevenBit>(20_000_000);
//             buffer3 = new GenricBitBufferWriter<NoEncoding>(20_000_000);
//         }

//         [Benchmark]
//         public void Generic()
//         {
//             for (int i = 0; i < N; i++)
//             {
//                 buffer2.u32(666);buffer2.u32(666);buffer2.u32(666);buffer2.u32(666);                        
//                 buffer2.u32(666);buffer2.u32(666);buffer2.u32(666);buffer2.u32(666);
//                 buffer2.u32(666);buffer2.u32(666);buffer2.u32(666);buffer2.u32(666);
//                 buffer2.u32(666);buffer2.u32(666);buffer2.u32(666);buffer2.u32(666);

//                 buffer2.u32(666);buffer2.u32(666);buffer2.u32(666);buffer2.u32(666);
//                 buffer2.u32(666);buffer2.u32(666);buffer2.u32(666);buffer2.u32(666);
//                 buffer2.u32(666);buffer2.u32(666);buffer2.u32(666);buffer2.u32(666);
//                 buffer2.u32(666);buffer2.u32(666);buffer2.u32(666);buffer2.u32(666);

//                 buffer2.u32(666);buffer2.u32(666);buffer2.u32(666);buffer2.u32(666);
//                 buffer2.u32(666);buffer2.u32(666);buffer2.u32(666);buffer2.u32(666);
//                 buffer2.u32(666);buffer2.u32(666);buffer2.u32(666);buffer2.u32(666);
//                 buffer2.u32(666);buffer2.u32(666);buffer2.u32(666);buffer2.u32(666);

//                 buffer2.u32(666);buffer2.u32(666);buffer2.u32(666);buffer2.u32(666);
//                 buffer2.u32(666);buffer2.u32(666);buffer2.u32(666);buffer2.u32(666);
//                 buffer2.u32(666);buffer2.u32(666);buffer2.u32(666);buffer2.u32(666);
//                 buffer2.u32(666);buffer2.u32(666);buffer2.u32(666);buffer2.u32(666);                                
//             }
//         }
                

//         [Benchmark]
//         public void Concrete()
//         {
//             for (int i = 0; i < N; i++)
//             {
//                 buffer1.u32(666);buffer1.u32(666);buffer1.u32(666);buffer1.u32(666);                        
//                 buffer1.u32(666);buffer1.u32(666);buffer1.u32(666);buffer1.u32(666);
//                 buffer1.u32(666);buffer1.u32(666);buffer1.u32(666);buffer1.u32(666);
//                 buffer1.u32(666);buffer1.u32(666);buffer1.u32(666);buffer1.u32(666);

//                 buffer1.u32(666);buffer1.u32(666);buffer1.u32(666);buffer1.u32(666);
//                 buffer1.u32(666);buffer1.u32(666);buffer1.u32(666);buffer1.u32(666);
//                 buffer1.u32(666);buffer1.u32(666);buffer1.u32(666);buffer1.u32(666);
//                 buffer1.u32(666);buffer1.u32(666);buffer1.u32(666);buffer1.u32(666);

//                 buffer1.u32(666);buffer1.u32(666);buffer1.u32(666);buffer1.u32(666);
//                 buffer1.u32(666);buffer1.u32(666);buffer1.u32(666);buffer1.u32(666);
//                 buffer1.u32(666);buffer1.u32(666);buffer1.u32(666);buffer1.u32(666);
//                 buffer1.u32(666);buffer1.u32(666);buffer1.u32(666);buffer1.u32(666);

//                 buffer1.u32(666);buffer1.u32(666);buffer1.u32(666);buffer1.u32(666);
//                 buffer1.u32(666);buffer1.u32(666);buffer1.u32(666);buffer1.u32(666);
//                 buffer1.u32(666);buffer1.u32(666);buffer1.u32(666);buffer1.u32(666);
//                 buffer1.u32(666);buffer1.u32(666);buffer1.u32(666);buffer1.u32(666);                                
//             }
//         }      

//         [Benchmark]
//         public void NoEncoding()
//         {
//             for (int i = 0; i < N; i++)
//             {
//                 buffer3.u32(666);buffer3.u32(666);buffer3.u32(666);buffer3.u32(666);                        
//                 buffer3.u32(666);buffer3.u32(666);buffer3.u32(666);buffer3.u32(666);
//                 buffer3.u32(666);buffer3.u32(666);buffer3.u32(666);buffer3.u32(666);
//                 buffer3.u32(666);buffer3.u32(666);buffer3.u32(666);buffer3.u32(666);

//                 buffer3.u32(666);buffer3.u32(666);buffer3.u32(666);buffer3.u32(666);
//                 buffer3.u32(666);buffer3.u32(666);buffer3.u32(666);buffer3.u32(666);
//                 buffer3.u32(666);buffer3.u32(666);buffer3.u32(666);buffer3.u32(666);
//                 buffer3.u32(666);buffer3.u32(666);buffer3.u32(666);buffer3.u32(666);

//                 buffer3.u32(666);buffer3.u32(666);buffer3.u32(666);buffer3.u32(666);
//                 buffer3.u32(666);buffer3.u32(666);buffer3.u32(666);buffer3.u32(666);
//                 buffer3.u32(666);buffer3.u32(666);buffer3.u32(666);buffer3.u32(666);
//                 buffer3.u32(666);buffer3.u32(666);buffer3.u32(666);buffer3.u32(666);

//                 buffer3.u32(666);buffer3.u32(666);buffer3.u32(666);buffer3.u32(666);
//                 buffer3.u32(666);buffer3.u32(666);buffer3.u32(666);buffer3.u32(666);
//                 buffer3.u32(666);buffer3.u32(666);buffer3.u32(666);buffer3.u32(666);
//                 buffer3.u32(666);buffer3.u32(666);buffer3.u32(666);buffer3.u32(666);                                
//             }
//         }            
//     }
// }