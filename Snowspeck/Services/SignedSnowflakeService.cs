// Copyright (c) 2025 Tiago Ferreira Alves. Licensed under the MIT License.
using Microsoft.Extensions.Options;
using Snowspeck.Metadata;

namespace Snowspeck.Services;

public class SignedSnowflakeService : SnowflakeServiceBase<long>
{
    public SignedSnowflakeService(IOptions<SnowflakeOptions> options)
        : base(options.Value) { }

    public SignedSnowflakeService(SnowflakeOptions options)
        : base(options) { }

    protected override long ComposeId(long timestamp, long workerId, long sequence)
    {
        return (timestamp << 22) | (workerId << 12) | sequence;
    }

    protected override SnowflakeMetadata ParseId(long id, long epoch)
    {
        long sequence = id & ((1L << 12) - 1L);
        long workerId = (id >> 12) & ((1L << 10) - 1L);
        long timestamp = (id >> 22) + epoch;

        return new SnowflakeMetadata(timestamp, workerId, sequence);
    }
}
