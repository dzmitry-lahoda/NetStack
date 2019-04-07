![Log](docs/logo.png)

Comprehensively tested code for creating concurrent networking systems for multiplayer games.

NetStack is dependant on `System.Memory`,  `System.Runtime.CompilerServices.Unsafe`. Optimized for `C# 7.3+`, `Unity 2018.3+` and `.NET Core 2.1+`. Works o `.NET Standard 2.0`. 

NetStack does NOT depends on `System.IO.Pipelines` and `System.Threading.Channels`.
  

# Exceptions, validation.

All validation and exception are behind `#if DEBUG || NETSTACK_VALIDATE`.

# Modules

- Compression
  - [Half precision](https://en.wikipedia.org/wiki/Half-precision_floating-point_format) algorithm
  - [Bounded range](https://gafferongames.com/post/snapshot_compression/#optimizing-position) algorithm
  - [Smallest three](https://gafferongames.com/post/snapshot_compression/#optimizing-orientation) algorithm
- Serialization ([Nuget](https://www.nuget.org/packages/NetStack.Serialization))
  - Lightweight and straightforward
  - Fast processing
  - [Span](https://docs.microsoft.com/en-us/dotnet/api/system.span) support
  - [Fluent builder](http://www.stefanoricciardi.com/2010/04/14/a-fluent-builder-in-c/) support
  - Compact bit-packing
    - [ZigZag](https://developers.google.com/protocol-buffers/docs/encoding#signed-integers) encoding
    - [Variable-length](https://rosettacode.org/wiki/Variable-length_quantity) encoding
    - TODO: opimize write of 2,3,4 bits values
    - TODO: allow to start write from where previous bit buffer finished (Unity FPSSample)
    - TODO: allow for interfaces with constrained generic usage (Unity FPSSample) so can do RAW bytes write
    - TODO: allow zero copy read write by init from byte array, cast head into ref as uint
    - TODO: allow plug custom compressor instead of 7bit encoding like (huffman Unity FPSSample in learning and ready alphabet encodings)
- Collections.Concurrent
  - ArrayQueue is Single-producer single-consumer first-in-first-out non-blocking queue
  - ConcurrentBuffer is Multi-producer multi-consumer first-in-first-out non-blocking queue
  - ConcurrentPool is Self-stabilizing semi-lockless circular buffer
- Collections (TODO)
  - CyclicSequence (from Gaffer on Games)
  - CyclicSequenceBuffer (from Gaffer on Games)
  - CyclicIdPool (from Gaffer on Games)

NetStack utilized [1](https://vimeo.com/292969981) and [2](https://forum.unity.com/threads/showcase-enet-unity-ecs-5000-real-time-player-simulation.605656/) 

# Usage

##### Concurrent objects pool:
```csharp
// Define a message object
class MessageObject {
	public uint id;
	public byte[] data;
}

// Create a new objects pool with 8 objects in the head
var messages = new ConcurrentPool<MessageObject>(8, () => new MessageObject());

// Acquire an object in the pool
MessageObject message = messages.Acquire();

// Do some stuff
message.id = 1;
message.data = buffers.Rent(64);

byte data = 0;

for (int i = 0; i < buffer.Length; i++) {
	message.data[i] = data++;
}

buffers.Return(message.data);

// Release pooled object
messages.Release(message);
```

##### Concurrent objects buffer:
```csharp
// Create a new concurrent buffer limited to 8192 cells
var conveyor = new ConcurrentBuffer(8192);

// Enqueue an object
conveyor.Enqueue(message);

// Dequeue object
MessageObject message = (MessageObject)conveyor.Dequeue();
```

##### Compress float:
```c#
// Compress data
ushort compressedSpeed = HalfPrecision.Compress(speed);

// Decompress data
float speed = HalfPrecision.Decompress(compressedSpeed);
```

##### Compress vector:
```csharp
// Create a new BoundedRange array for Vector3 position, each entry has bounds and precision
BoundedRange[] worldBounds = new BoundedRange[3];

worldBounds[0] = new BoundedRange(-50f, 50f, 0.05f); // X axis
worldBounds[1] = new BoundedRange(0f, 25f, 0.05f); // Y axis
worldBounds[2] = new BoundedRange(-50f, 50f, 0.05f); // Z axis

// Compress position data
CompressedVector3 compressedPosition = BoundedRange.Compress(position, worldBounds);

// Read compressed data
Console.WriteLine("Compressed position - X: " + compressedPosition.x + ", Y:" + compressedPosition.y + ", Z:" + compressedPosition.z);

// Decompress position data
Vector3 decompressedPosition = BoundedRange.Decompress(compressedPosition, worldBounds);
```

##### Compress quaternion:
```csharp
// Compress rotation data
CompressedQuaternion compressedRotation = SmallestThree.Compress(rotation);

// Read compressed data
Console.WriteLine("Compressed rotation - M: " + compressedRotation.m + ", A:" + compressedRotation.a + ", B:" + compressedRotation.b + ", C:" + compressedRotation.c);

// Decompress rotation data
Quaternion rotation = SmallestThree.Decompress(compressedRotation);
```

##### Serialize/deserialize data:
```csharp
// Create a new bit buffer with 1024 chunks, the buffer can grow automatically if required
var data = new BitBuffer(1024);

// Fill bit buffer and serialize data to a byte array
data.AddUInt(peer)
.AddString(name)
.AddBool(accelerated)
.AddUShort(speed)
.AddUInt(compressedPosition.x)
.AddUInt(compressedPosition.y)
.AddUInt(compressedPosition.z)
.AddByte(compressedRotation.m)
.AddShort(compressedRotation.a)
.AddShort(compressedRotation.b)
.AddShort(compressedRotation.c)
.ToArray(buffer);

// Get a length of actual data in bit buffer for sending through the network
Console.WriteLine("Data length: " + data.Length);

// Reset bit buffer for further reusing
data.Clear();

// Deserialize data from a byte array
data.FromArray(buffer, length);

// Unload bit buffer in the same order
uint peer = data.ReadUInt();
string name = data.ReadString();
bool accelerated = data.ReadBool();
ushort speed = data.ReadUShort();
CompressedVector3 position = new CompressedVector3(data.ReadUInt(), data.ReadUInt(), data.ReadUInt());
CompressedQuaternion rotation = new CompressedQuaternion(data.ReadByte(), data.ReadShort(), data.ReadShort(), data.ReadShort());

// Check if bit buffer is fully unloaded
Console.WriteLine("Bit buffer is empty: " + data.IsFinished);
```

##### Abstract data serialization with Span:
```csharp
// Create a one-time allocation buffer pool
static class BufferPool {
	[ThreadStatic]
	private static BitBuffer bitBuffer;

	public static BitBuffer GetBitBuffer() {
		if (bitBuffer == null)
			bitBuffer = new BitBuffer(1024);

		return bitBuffer;
	}
}

// Define a networking message
struct MessageObject {
	public const ushort id = 1; // Used to identify the message, can be packed or sent as packet header
	public uint peer;
	public byte race;
	public ushort skin;

	public void Serialize(ref Span<byte> packet) {
		BitBuffer data = BufferPool.GetBitBuffer();

		data.AddUInt(peer)
		.AddByte(race)
		.AddUShort(skin)
		.ToSpan(ref packet);

		data.Clear();
	}

	public void Deserialize(ref ReadOnlySpan<byte> packet, int length) {
		BitBuffer data = BufferPool.GetBitBuffer();

		data.FromSpan(ref packet, length);

		peer = data.ReadUInt();
		race = data.ReadByte();
		skin = data.ReadUShort();

		data.Clear();
	}
}
```
