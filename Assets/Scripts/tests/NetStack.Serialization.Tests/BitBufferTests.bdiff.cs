using System;
using System.Numerics;
using NUnit.Framework;
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


namespace NetStack.Serialization
{
    partial class BitBufferTests
    {
        [Test]
        public void i16Bdiff()
        {
            var writer = new BitBufferWriter<SevenBitEncoding>();
            writer.i16BDiff(0, 1);
            writer.i16BDiff(1, 1);
            var data = writer.ToArray();            
            var reader = new BitBufferReader<SevenBitDecoding>();
            reader.CopyFrom(data);
            Assert.AreEqual(1, reader.i16BDiff(0));
            Assert.AreEqual(1, reader.i16BDiff(1));
        }

        [Test]
        public void i16BdiffLimits()
        {
            var writer = new BitBufferWriter<SevenBitEncoding>();
            writer.i16BDiff(0, 1, -1, 10);
            writer.i16BDiff(1, 1, 0, 100);
            var data = writer.ToArray();            
            var reader = new BitBufferReader<SevenBitDecoding>();
            reader.CopyFrom(data);
            Assert.AreEqual(1, reader.i16BDiff(0, -1, 10));
            Assert.AreEqual(1, reader.i16BDiff(1, 0, 100));
        }        

        [Test]
        public void i32Bdiff()
        {
            var writer = new BitBufferWriter<SevenBitEncoding>();
            writer.i32BDiff(0, 1);
            writer.i32BDiff(1, 1);
            var data = writer.ToArray();            
            var reader = new BitBufferReader<SevenBitDecoding>();
            reader.CopyFrom(data);
            Assert.AreEqual(1, reader.i32BDiff(0));
            Assert.AreEqual(1, reader.i32BDiff(1));
        }   

        [Test]
        public void i32BdiffLimits()
        {
            var writer = new BitBufferWriter<SevenBitEncoding>();
            writer.i32BDiff(0, 1, -1, 10);
            writer.i32BDiff(1, 1, 0, 100);
            var data = writer.ToArray();            
            var reader = new BitBufferReader<SevenBitDecoding>();
            reader.CopyFrom(data);
            Assert.AreEqual(1, reader.i32BDiff(0, -1, 10));
            Assert.AreEqual(1, reader.i32BDiff(1, 0, 100));
        }         

        [Test]
        public void i8Bdiff()
        {
            var writer = new BitBufferWriter<SevenBitEncoding>();
            writer.i8BDiff(0, 1);
            writer.i8BDiff(1, 1);
            var data = writer.ToArray();            
            var reader = new BitBufferReader<SevenBitDecoding>();
            reader.CopyFrom(data);
            Assert.AreEqual(1, reader.i8BDiff(0));
            Assert.AreEqual(1, reader.i8BDiff(1));
        }              

        [Test]
        public void u16Bdiff()
        {
            var writer = new BitBufferWriter<SevenBitEncoding>();
            writer.u16BDiff(0, 1);
            writer.u16BDiff(1, 1);
            var data = writer.ToArray();            
            var reader = new BitBufferReader<SevenBitDecoding>();
            reader.CopyFrom(data);
            Assert.AreEqual(1, reader.u16BDiff(0));
            Assert.AreEqual(1, reader.u16BDiff(1));
        }     

        [Test]
        public void u32Bdiff()
        {
            var writer = new BitBufferWriter<SevenBitEncoding>();
            writer.u32BDiff(0, 1);
            writer.u32BDiff(1, 1);
            var data = writer.ToArray();            
            var reader = new BitBufferReader<SevenBitDecoding>();
            reader.CopyFrom(data);
            Assert.AreEqual(1, reader.u32BDiff(0));
            Assert.AreEqual(1, reader.u32BDiff(1));
        }          


        [Test]
        public void u8Bdiff()
        {
            var writer = new BitBufferWriter<SevenBitEncoding>();
            writer.u8BDiff(0, 1);
            writer.u8BDiff(1, 1);
            var data = writer.ToArray();            
            var reader = new BitBufferReader<SevenBitDecoding>();
            reader.CopyFrom(data);
            Assert.AreEqual(1, reader.u8BDiff(0));
            Assert.AreEqual(1, reader.u8BDiff(1));
        }   

        [Test]
        public void f32Bdiff()
        {
            var writer = new BitBufferWriter<SevenBitEncoding>();
            writer.f32BDiff(0f, 1f);
            writer.f32BDiff(1f, 1f);
            var data = writer.ToArray();            
            var reader = new BitBufferReader<SevenBitDecoding>();
            reader.CopyFrom(data);
            Assert.AreEqual(1f, reader.f32BDiff(0f));
            Assert.AreEqual(1f, reader.f32BDiff(1f));
        }               
    }
}
