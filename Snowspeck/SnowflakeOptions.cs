// Copyright (c) 2025 Tiago Ferreira Alves. Licensed under the MIT License.
namespace Snowspeck;

/// <summary>
/// Configuration options for the Snowflake ID generator.
/// </summary>
public class SnowflakeOptions
{
    /// <summary>
    /// The custom epoch (in Unix milliseconds) to start ID generation from.
    /// For example, 1735689600000 = Jan 1, 2025 UTC.
    /// </summary>
    public long Epoch { get; init; } = 1735689600000L;

    /// <summary>
    /// A unique worker/machine identifier (0-1023).
    /// Ensure no two instances share the same WorkerId.
    /// </summary>
    public long WorkerId { get; init; }
}
