using System;
using System.Threading;

namespace NetStack.Threading
{
    public sealed class ConcurrentPool<T> where T : class
    {

        private SpinLock _lock;
        private readonly Func<T> _factory;
        private Segment _head;
        private Segment _tail;

        public ConcurrentPool(int capacity, Func<T> factory)
        {
            _lock = new SpinLock();
            _head = _tail = new Segment(capacity);
            _factory = factory;
        }

        public T Acquire()
        {
            while (true)
            {
                var localHead = _head;
                var count = localHead.Count;

                if (count == 0)
                {
                    if (localHead.Next != null)
                    {

                        bool lockTaken = false;

                        try
                        {
                            _lock.Enter(ref lockTaken);

                            if (_head.Next != null && _head.Count == 0)
                                _head = _head.Next;
                        }

                        finally
                        {
                            if (lockTaken)
                                _lock.Exit(false);
                        }

                    }
                    else
                    {
                        return _factory();
                    }
                }
                else if (Interlocked.CompareExchange(ref localHead.Count, count - 1, count) == count)
                {
                    var localLow = Interlocked.Increment(ref localHead.Low) - 1;
                    var index = localLow & localHead.Mask;
                    T item;
                    var spinWait = new SpinWait();

                    while ((item = Interlocked.Exchange(ref localHead.Items[index], null)) == null)
                    {
                        spinWait.SpinOnce();
                    }

                    return item;
                }
            }
        }

        public void Release(T item)
        {
            while (true)
            {
                var localTail = _tail;
                var count = localTail.Count;

                if (count == localTail.Items.Length)
                {

                    bool lockTaken = false;

                    try
                    {
                        _lock.Enter(ref lockTaken);

                        if (_tail.Next == null && count == _tail.Items.Length)
                            _tail = _tail.Next = new Segment(_tail.Items.Length << 1);
                    }

                    finally
                    {
                        if (lockTaken)
                            _lock.Exit(false);
                    }

                }
                else if (Interlocked.CompareExchange(ref localTail.Count, count + 1, count) == count)
                {
                    var localHigh = Interlocked.Increment(ref localTail.High) - 1;
                    var index = localHigh & localTail.Mask;
                    var spinWait = new SpinWait();

                    while (Interlocked.CompareExchange(ref localTail.Items[index], item, null) != null)
                    {
                        spinWait.SpinOnce();
                    }

                    return;
                }
            }
        }

        private class Segment
        {
            public readonly T[] Items;
            public readonly int Mask;
            public int Low;
            public int High;
            public int Count;
            public Segment Next;

            public Segment(int size)
            {
                if (size <= 0 || ((size & (size - 1)) != 0))
                    throw new ArgumentOutOfRangeException("Segment size must be power of two");

                Items = new T[size];
                Mask = size - 1;
            }
        }
    }
}