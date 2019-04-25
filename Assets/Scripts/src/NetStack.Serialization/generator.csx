// dotnet script generator.csx

using System.Numerics;
using System;
using System.Linq;
using System.Diagnostics;
using i8 = System.SByte;
using i16 = System.Int16;
using i32 = System.Int32;
using i64 = System.Int64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;

// Pre-compute all Fibonacci numbers that can fit in number
public void FibonacciLookup64()
{
    var lookup = new u64[92];
    lookup[0] = 1;
    lookup[1] = 2;
    for (var i = 2; i < lookup.Length; i++)
    {
        checked
        {
            u64 value = lookup[i - 1] + lookup[i - 2];
            lookup[i] = value;
        }
    }

    Debug.WriteLine(lookup.Aggregate("", (acc, el) => acc + ", " + el));
    Console.WriteLine(lookup.Aggregate("", (acc, el) => acc + ", " + el));
}


public void FibonacciLookup32()
{
    var lookup = new u32[46];
    lookup[0] = 1;
    lookup[1] = 2;
    for (var i = 2; i < lookup.Length; i++)
    {
        checked
        {
            lookup[i] = lookup[i - 1] + lookup[i - 2];
        }
    }
}


public void FibonacciLookup16()
{
    var lookup = new u16[23];
    lookup[0] = 1;
    lookup[1] = 2;
    for (var i = 2; i < lookup.Length; i++)
    {
        checked
        {
            lookup[i] = (u16)(lookup[i - 1] + lookup[i - 2]);
        }
    }
}

public void FibonacciLookup8()
{
    var lookup = new u8[12];
    lookup[0] = 1;
    lookup[1] = 2;
    for (var i = 2; i < lookup.Length; i++)
    {
        checked
        {
            lookup[i] = (u8)(lookup[i - 1] + lookup[i - 2]);
        }
    }
}

FibonacciLookup64();