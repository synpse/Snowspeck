// Copyright (c) 2025 Tiago Ferreira Alves. Licensed under the MIT License.
namespace Snowspeck.Metadata;

/// <summary>
/// Represents decoded information from a generated Snowflake ID.
/// </summary>
public readonly record struct SnowflakeMetadata(long Timestamp, long WorkerId, long Sequence)
{
    /// <summary>
    /// Returns a simple string representation of the metadata.
    /// </summary>
    public override string ToString() =>
        $"[Timestamp: {Timestamp}, Worker: {WorkerId}, Sequence: {Sequence}]";

    /// <summary>
    /// Returns a user-friendly string including UTC datetime.
    /// </summary>
    public string ToFriendlyString() =>
        $"[{ToDateTimeOffset():u}, Worker: {WorkerId}, Sequence: {Sequence}]";

    /// <summary>
    /// Converts the Unix millisecond timestamp to a <see cref="DateTimeOffset"/>.
    /// </summary>
    public DateTimeOffset ToDateTimeOffset() =>
        DateTimeOffset.FromUnixTimeMilliseconds(Timestamp);

    /// <summary>
    /// Compares two metadata instances by timestamp.
    /// </summary>
    public static bool operator <(SnowflakeMetadata left, SnowflakeMetadata right) =>
        left.Timestamp < right.Timestamp;

    /// <summary>
    /// Compares two metadata instances by timestamp.
    /// </summary>
    public static bool operator >(SnowflakeMetadata left, SnowflakeMetadata right) =>
        left.Timestamp > right.Timestamp;

    /// <summary>
    /// Compares two metadata instances by timestamp.
    /// </summary>
    public static bool operator <=(SnowflakeMetadata left, SnowflakeMetadata right) =>
        left.Timestamp <= right.Timestamp;

    /// <summary>
    /// Compares two metadata instances by timestamp.
    /// </summary>
    public static bool operator >=(SnowflakeMetadata left, SnowflakeMetadata right) =>
        left.Timestamp >= right.Timestamp;
}
