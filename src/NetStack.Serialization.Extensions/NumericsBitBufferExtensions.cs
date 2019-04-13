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
using NetStack.Compression;

namespace NetStack.Serialization
{
    /// <summary>
    /// Vector and quaternion compression.
    /// </summary>
    public static class NumericsBitBufferExtensions
    {
        private const float SmallestThreeUnpack = 0.70710678118654752440084436210485f + 0.0000001f; // addition to rounding to fit in -0....1023 instead of 0...1024
        private const float SmallestThreePack = 1f / SmallestThreeUnpack;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddQuaternion(this BitBufferWriter<SevenBitEncoding> self, Quaternion quaternion, int bitsPerComponent = 12)
        {
            float halfrangeFloat = (1 << bitsPerComponent - 1);
            float packer = SmallestThreePack * halfrangeFloat;

            uint m = 0, a, b, c;
            float maxValue = float.MinValue;
            bool signminus = false;

#if (ENABLE_MONO)
            var fastAbs = new Utils.FastAbs();
#endif
            for (uint quadIndex = 0; quadIndex <= 3; quadIndex++)
            {
                float element = 0f;
                float abs = 0f;

#if !(ENABLE_MONO || ENABLE_IL2CPP)
                switch (quadIndex)
                {
                    case 0:
                        element = quaternion.X;
                        break;
                    case 1:
                        element = quaternion.Y;
                        break;
                    case 2:
                        element = quaternion.Z;
                        break;
                    case 3:
                        element = quaternion.W;
                        break;
                }
#else
                switch (quadIndex) {
                    case 0:
                        element = quaternion.x;
                        break;
                    case 1:
                        element = quaternion.y;
                        break;
                    case 2:
                        element = quaternion.z;
                        break;
                    case 3:
                        element = quaternion.w;
                        break;
                }

#endif
#if (ENABLE_MONO)
                fastAbs.single = element;
                fastAbs.uint32 &= 0x7FFFFFFF;
                abs = fastAbs.single;
#else
                abs = Math.Abs(element);
#endif

                if (abs > maxValue)
                {
                    signminus = (element < 0f);
                    m = quadIndex;
                    maxValue = abs;
                }
            }

#if !(ENABLE_MONO || ENABLE_IL2CPP)
            float aflt, bflt, cflt;
            switch (m)
            {
                case 0: aflt = quaternion.Y; bflt = quaternion.Z; cflt = quaternion.W; break;
                case 1: aflt = quaternion.X; bflt = quaternion.Z; cflt = quaternion.W; break;
                case 2: aflt = quaternion.X; bflt = quaternion.Y; cflt = quaternion.W; break;
                default: aflt = quaternion.X; bflt = quaternion.Y; cflt = quaternion.Z; break;
            }
#else
            float aflt, bflt, cflt;
            switch (m) {
                case 0: aflt = quaternion.y; bflt = quaternion.z; cflt = quaternion.w; break;
                case 1: aflt = quaternion.x; bflt = quaternion.z; cflt = quaternion.w; break;
                case 2: aflt = quaternion.x; bflt = quaternion.y; cflt = quaternion.w; break;
                default: aflt = quaternion.x; bflt = quaternion.y; cflt = quaternion.z; break;
            }
#endif

            if (signminus)
            {
                a = (uint)((-aflt * packer) + halfrangeFloat);
                b = (uint)((-bflt * packer) + halfrangeFloat);
                c = (uint)((-cflt * packer) + halfrangeFloat);
            }
            else
            {
                a = (uint)((aflt * packer) + halfrangeFloat);
                b = (uint)((bflt * packer) + halfrangeFloat);
                c = (uint)((cflt * packer) + halfrangeFloat);
            }

            self.raw(m, 2);
            self.raw(a, bitsPerComponent);
            self.raw(b, bitsPerComponent);
            self.raw(c, bitsPerComponent);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion ReadQuaternion(this BitBufferReader<SevenBitDecoding> self, int bitsPerValue = 12)
        {
            int halfrange = (1 << bitsPerValue - 1); //  - 1
            float unpacker = SmallestThreeUnpack * (1f / halfrange);

            uint m = self.raw(2);
            int aint = (int)self.raw(bitsPerValue);
            int bint = (int)self.raw(bitsPerValue);
            int cint = (int)self.raw(bitsPerValue);

            aint -= halfrange;
            bint -= halfrange;
            cint -= halfrange;

            float a = aint * unpacker;
            float b = bint * unpacker;
            float c = cint * unpacker;

            float d = (float)Math.Sqrt(1f - ((a * a) + (b * b) + (c * c)));

            switch (m)
            {
                case 0:
                    return new Quaternion(d, a, b, c);
                case 1:
                    return new Quaternion(a, d, b, c);
                case 2:
                    return new Quaternion(a, b, d, c);
                default:
                    return new Quaternion(a, b, c, d);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion PeekQuaternion(this BitBufferReader<SevenBitDecoding> self, int bitsPerValue = 12)
        {
            var curReadpos = self.BitsRead;
            var value = self.ReadQuaternion(bitsPerValue);
            self.SetReadPosition(curReadpos);          
            return value;
        }        
    }
}