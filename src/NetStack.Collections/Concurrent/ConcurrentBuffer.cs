using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace NetStack.Collections.Concurrent
{
	[StructLayout(LayoutKind.Explicit, Size = 192)]
	public sealed class ConcurrentBuffer {
		[FieldOffset(0)]
		private readonly Cell[] _buffer;
		[FieldOffset(8)]
		private readonly int _bufferMask;
		[FieldOffset(64)]
		private int _enqueuePosition;
		[FieldOffset(128)]
		private int _dequeuePosition;

		public int Count => _enqueuePosition - _dequeuePosition;

		public ConcurrentBuffer(int bufferSize) {
			if (bufferSize < 2)
				throw new ArgumentException("Buffer size should be greater than or equal to two");

			if ((bufferSize & (bufferSize - 1)) != 0)
				throw new ArgumentException("Buffer size should be a power of two");

			_bufferMask = bufferSize - 1;
			_buffer = new Cell[bufferSize];

			for (var i = 0; i < bufferSize; i++) {
				_buffer[i] = new Cell(i, null);
			}

			_enqueuePosition = 0;
			_dequeuePosition = 0;
		}

		public void Enqueue(object item) {
			while (true) {
				if (TryEnqueue(item))
					break;

				Thread.SpinWait(1);
			}
		}

		public bool TryEnqueue(object item) {
			do {
				var buffer = _buffer;
				var position = _enqueuePosition;
				var index = position & _bufferMask;
				var cell = buffer[index];

				if (cell.Sequence == position && Interlocked.CompareExchange(ref _enqueuePosition, position + 1, position) == position) {
					buffer[index].Element = item;

						Volatile.Write(ref buffer[index].Sequence, position + 1);

					return true;
				}

				if (cell.Sequence < position)
					return false;
			}

			while (true);
		}

		public object Dequeue() {
			while (true) {
				object element;

				if (TryDequeue(out element))
					return element;
			}
		}

		public bool TryDequeue(out object result) {
			do {
				var buffer = _buffer;
				var bufferMask = _bufferMask;
				var position = _dequeuePosition;
				var index = position & bufferMask;
				var cell = buffer[index];

				if (cell.Sequence == position + 1 && Interlocked.CompareExchange(ref _dequeuePosition, position + 1, position) == position) {
					result = cell.Element;
					buffer[index].Element = null;

						Volatile.Write(ref buffer[index].Sequence, position + bufferMask + 1);

					return true;
				}

				if (cell.Sequence < position + 1) {
					result = default(object);

					return false;
				}
			}

			while (true);
		}

		[StructLayout(LayoutKind.Explicit, Size = 16)]
		private struct Cell {
			[FieldOffset(0)]
			public int Sequence;
			[FieldOffset(8)]
			public object Element;

			public Cell(int sequence, object element) {
				Sequence = sequence;
				Element = element;
			}
		}
	}
}