
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NetStack.Compression
{
    /// <summary>
    /// Lossless compression for float.
    /// </summary>
    /// <seealso href="http://www.fox-toolkit.org/ftp/fasthalffloatconversion.pdf"/>
	/// <seealso href="https://github.com/omgwtfgames/slimmath/blob/master/SlimMath/HalfUtilities.cs"/>
    public static class HalfPrecision
    {
        [StructLayout(LayoutKind.Explicit)]
        private struct Values
        {
            [FieldOffset(0)]
            public float f;
            [FieldOffset(0)]
            public int i;
            [FieldOffset(0)]
            public uint u;
        }

#if NETSTACK_INLINING
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ushort Compress(float value)
        {
            var values = new Values
            {
                f = value
            };

            return Compress(values.i);
        }

        public static ushort Compress(int value)
        {
            int s = (value >> 16) & 0x00008000;
            int e = ((value >> 23) & 0X000000FF) - (127 - 15);
            int m = value & 0X007FFFFF;

            if (e <= 0)
            {
                if (e < -10)
                    return (ushort)s;

                m = m | 0x00800000;

                int t = 14 - e;
                int a = (1 << (t - 1)) - 1;
                int b = (m >> t) & 1;

                m = (m + a + b) >> t;

                return (ushort)(s | m);
            }

            if (e == 0XFF - (127 - 15))
            {
                if (m == 0)
                    return (ushort)(s | 0X7C00);

                m >>= 13;

                return (ushort)(s | 0X7C00 | m | ((m == 0) ? 1 : 0));
            }

            m = m + 0X00000FFF + ((m >> 13) & 1);

            if ((m & 0x00800000) != 0)
            {
                m = 0;
                e++;
            }

            if (e > 30)
                return (ushort)(s | 0X7C00);

            return (ushort)(s | (e << 10) | (m >> 13));
        }

        public static float Decompress(ushort value)
        {
            uint result;
            uint mantissa = (uint)(value & 1023);
            uint exponent = 0XFFFFFFF2;

            if ((value & -33792) == 0)
            {
                if (mantissa != 0)
                {
                    while ((mantissa & 1024) == 0)
                    {
                        exponent--;
                        mantissa = mantissa << 1;
                    }

                    mantissa &= 0XFFFFFBFF;
                    result = ((uint)((((uint)value & 0x8000) << 16) | ((exponent + 127) << 23))) | (mantissa << 13);
                }
                else
                {
                    result = (uint)((value & 0x8000) << 16);
                }
            }
            else
            {
                result = ((((uint)value & 0x8000) << 16) | ((((((uint)value >> 10) & 0X1F) - 15) + 127) << 23)) | (mantissa << 13);
            }

            var values = new Values
            {
                u = result
            };

            return values.f;
        }
    }
}