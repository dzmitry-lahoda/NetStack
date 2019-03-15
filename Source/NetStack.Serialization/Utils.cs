using System.Runtime.InteropServices;

namespace NetStack.Serialization {
    public static class Utils {
        public static int FindHighestBitPosition(byte data) {
            int shiftCount = 0;

            while (data > 0) {
                data >>= 1;
                shiftCount++;
            }

            return shiftCount;
        }

        public static readonly int[] LogTable256 = new int[256]
        {
            0, 0, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
            7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
            7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7
        };

        public static int Log2(uint number) {
            if (number <= 0xffffu) {
                if (number > 0xffu) {
                    return 8 + LogTable256[number >> 8];
                }
                return LogTable256[number];
            }
            if (number <= 0xffffffu) {
                return 16 + LogTable256[number >> 16];
            }
            return 24 + LogTable256[number >> 24];
        }

        public static int BitsRequired(int min, int max) {
            return (min == max) ? 1 : Log2((uint)(max - min)) + 1;
        }

        public static int BitsRequired(uint min, uint max) {
            return (min == max) ? 1 : Log2(max - min) + 1;
        }

        public static int GetStringBitSize(string value, int length)
        {
            var bitLength = 10;

            uint codePage = 0; // Ascii
            for (int i = 0; i < length; i++) {
                var val = value[i];
                if (val > 127) {
                    codePage = 1; // Latin1
                    if (val > 255) {
                        codePage = 2; // LatinExtended 
                        if (val > 511) {
                            codePage = 3; // UTF-16
                            break;
                        }
                    }
                }
            }
            
            if (codePage == 0)
                bitLength += length * 7;
            else if (codePage == 1)
                bitLength += length * 8;
            else if (codePage == 2)
                bitLength += length * 9;
            else if (codePage == 3)
                for (int i = 0; i < length; i++) {
                    if (value[i] > 127)
                        bitLength += 17;
                    else
                        bitLength += 8;
                }

            return bitLength;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct FloatUintUnion {
            [FieldOffset(0)] public float single;
            [FieldOffset(0)] public uint uint32;
        }

#if (ENABLE_MONO)
        [StructLayout(LayoutKind.Explicit)]
        public struct FastAbs {
            [FieldOffset(0)] public uint uint32;
            [FieldOffset(0)] public float single;
        }
#endif
    }
}