// Copyright (c) 2025 Tiago Ferreira Alves. Licensed under the MIT License.
using Microsoft.Extensions.Options;
using Snowspeck.Metadata;

namespace Snowspeck.Services;

public class UnsignedSnowflakeService : SnowflakeServiceBase<ulong>
{
    public UnsignedSnowflakeService(IOptions<SnowflakeOptions> options)
        : base(options.Value) { }

    public UnsignedSnowflakeService(SnowflakeOptions options)
        : base(options) { }

    protected override ulong ComposeId(long timestamp, long workerId, long sequence)
    {
        return ((ulong)timestamp << 22) | ((ulong)workerId << 12) | (ulong)sequence;
    }

    protected override SnowflakeMetadata ParseId(ulong id, long epoch)
    {
        const ulong sequenceMask = (1UL << 12) - 1;
        const ulong workerIdMask = (1UL << 10) - 1;

        long sequence = (long)(id & sequenceMask);
        long workerId = (long)((id >> 12) & workerIdMask);
        long timestamp = (long)(id >> 22) + epoch;

        return new SnowflakeMetadata(timestamp, workerId, sequence);
    }
}
