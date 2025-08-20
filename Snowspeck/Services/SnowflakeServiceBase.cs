// Copyright (c) 2025 Tiago Ferreira Alves. Licensed under the MIT License.
using Snowspeck.Interfaces;
using Snowspeck.Metadata;

namespace Snowspeck.Services;

public abstract class SnowflakeServiceBase<TId> : ISnowflakeGenerator<TId>
{
    private readonly object _lock = new();

    private readonly long _epoch;
    private readonly long _workerId;
    private readonly long _maxSequence;

    private long _lastTimestamp = -1L;
    private long _sequence;

    protected SnowflakeServiceBase(SnowflakeOptions options)
    {
        if (options is null)
            throw new InvalidOperationException("Snowflake options must be provided.");

        _epoch = options.Epoch;
        _workerId = options.WorkerId;
        _maxSequence = ~(-1L << 12); // SequenceBits

        long maxWorkerId = ~(-1L << 10); // WorkerIdBits
        if (_workerId < 0 || _workerId > maxWorkerId)
            throw new ArgumentException($"Worker id must be between 0 and {maxWorkerId}");
    }

    public TId NextId()
    {
        lock (_lock)
        {
            long timestamp = GetCurrentTimestamp();

            if (timestamp < _lastTimestamp)
                throw new InvalidOperationException("Invalid timestamp");

            if (timestamp == _lastTimestamp)
            {
                _sequence = (_sequence + 1) & _maxSequence;
                if (_sequence == 0)
                {
                    timestamp = WaitForNextMillis(timestamp);
                }
            }
            else
            {
                _sequence = 0;
            }

            _lastTimestamp = timestamp;

            return ComposeId(timestamp - _epoch, _workerId, _sequence);
        }
    }

    public SnowflakeMetadata Decode(TId id)
    {
        return ParseId(id, _epoch);
    }

    protected abstract TId ComposeId(long timestamp, long workerId, long sequence);
    protected abstract SnowflakeMetadata ParseId(TId id, long epoch);

    private long GetCurrentTimestamp() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    private long WaitForNextMillis(long current)
    {
        long timestamp = GetCurrentTimestamp();
        while (timestamp <= current)
        {
            timestamp = GetCurrentTimestamp();
        }
        return timestamp;
    }
}
