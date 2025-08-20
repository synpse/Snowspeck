# Snowspeck
**A lightweight, thread-safe, extensible, and decodable Snowflake ID generator for .NET.**

![NuGet](https://img.shields.io/nuget/v/Snowspeck.svg)
![License: MIT](https://img.shields.io/badge/license-MIT-yellow.svg)

---

## Features

- 64-bit unique, time-ordered ID generation
- Fully thread-safe
- Decodable (timestamp, worker ID, sequence)
- Minimal dependencies (`Microsoft.Extensions.Options`)
- Great for distributed systems and high-throughput workloads

---

## What is a Snowflake ID?

A Snowflake ID is a 64-bit integer **originally developed by Twitter** to generate unique, time-ordered identifiers in distributed systems.

It typically encodes:
- A timestamp (usually in milliseconds)
- A machine or worker ID (identifying the node that generated the ID)
- A sequence number (to prevent collisions when generating multiple IDs in the same millisecond)

This structure ensures:
- Globally unique identifiers without coordination between nodes
- IDs that increase over time (sortable)
- High-performance generation, suitable for distributed environments

### Disclaimer

Snowflake IDs are intentionally decodable and expose metadata like generation time and worker identity.  
They are **not cryptographically secure** and should **not** be used to encode sensitive or private information.

Use them only for **unique, ordered identifiers**, not for hiding or securing data.

---

## Installation

Using the CLI:

```bash
dotnet add package Snowspeck
```

Or via the NuGet Package Manager:

```powershell
Install-Package Snowspeck
```

---

## Implementations

Snowspeck offers **two implementations** of the generator, depending on whether you prefer signed or unsigned IDs:

| Class                       | Return Type | ID Range           | Notes                                       |
|----------------------------|-------------|--------------------|---------------------------------------------|
| `SignedSnowflakeService`   | `long`      | -2⁶³ to 2⁶³−1      | Default in DI setup, fits in signed columns |
| `UnsignedSnowflakeService` | `ulong`     | 0 to 2⁶⁴−1         | Full 64-bit range, use when unsigned is OK  |

Both use the same structure:
- 42 bits for timestamp
- 10 bits for worker ID
- 12 bits for sequence number

Choose the implementation that best fits your database, serialization, or sorting needs.

---

## Quick Start

### Manual Instantiation

```csharp
using Snowspeck;
using Snowspeck.Interfaces;
using Snowspeck.Services;

var options = new SnowflakeOptions
{
    WorkerId = 1,
    Epoch = 1735689600000L // (optional) Default is 1735689600000L -> Jan 1, 2025
};

ISnowflakeGenerator generator = new SignedSnowflakeService(options);
long id = generator.NextId();
```

---

### Dependency Injection (ASP.NET Core)

```csharp
builder.Services.AddSnowspeck(options =>
{
    options.WorkerId = 1;
    options.Epoch = 1735689600000L; // (optional) Default is 1735689600000L -> Jan 1, 2025
});
```

```csharp
builder.Services.AddSnowspeckUnsigned(options =>
{
    options.WorkerId = 1;
    options.Epoch = 1735689600000L; // (optional) Default is 1735689600000L -> Jan 1, 2025
});
```

Once registered, resolve `ISnowflakeGenerator` via constructor injection:

```csharp
public class MyService
{
    private readonly ISnowflakeGenerator _generator;

    public MyService(ISnowflakeGenerator generator)
    {
        _generator = generator;
    }

    public void DoSomething()
    {
        long id = _generator.NextId();
    }
}
```

---

### Decoding Snowflake Metadata

The generator supports decoding a Snowflake ID back into:

- `Timestamp` (UTC time the ID was generated)
- `WorkerId`
- `Sequence`

```csharp
long id = generator.NextId();
var metadata = generator.Decode(id);

Console.WriteLine($"Timestamp: {metadata.Timestamp}");
Console.WriteLine($"WorkerId: {metadata.WorkerId}");
Console.WriteLine($"Sequence: {metadata.Sequence}");
```

---

## Planned Features

- **Lock-free ID generation**  
  Explore a non-blocking alternative using atomics or semaphores for high-concurrency environments.

### Have an idea or feature request? Feel free to open an issue.

---

## License

Licensed under the [MIT License](LICENSE).  
© 2025 Tiago Ferreira Alves
