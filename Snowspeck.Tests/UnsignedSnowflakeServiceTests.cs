using System.Collections.Concurrent;
using Snowspeck.Services;
using FluentAssertions;
using Microsoft.Extensions.Options;

namespace Snowspeck.Tests;

public class UnsignedSnowflakeServiceTests
{
    private readonly UnsignedSnowflakeService _service;

    public UnsignedSnowflakeServiceTests()
    {
        IOptions<SnowflakeOptions> options = Options.Create(new SnowflakeOptions
        {
            WorkerId = 1,
            Epoch = 1735689600000L
        });

        _service = new UnsignedSnowflakeService(options);
    }

    [Fact]
    public void NextId_ShouldReturnUniqueIds()
    {
        HashSet<ulong> ids = [];

        for (int i = 0; i < 1000; i++)
        {
            ulong id = _service.NextId();
            ids.Add(id);
        }

        ids.Count.Should().Be(1000);
    }

    [Fact]
    public void NextId_ShouldBeGreaterThanZero()
    {
        ulong id = _service.NextId();
        id.Should().BeGreaterThan(0UL);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenWorkerIdOutOfRange()
    {
        IOptions<SnowflakeOptions> optionsA = Options.Create(new SnowflakeOptions
        {
            WorkerId = 2048,
            Epoch = 1735689600000L
        });

        IOptions<SnowflakeOptions> optionsB = Options.Create(new SnowflakeOptions
        {
            WorkerId = -1,
            Epoch = 1735689600000L
        });

        Action actionA = () => _ = new UnsignedSnowflakeService(optionsA);
        Action actionB = () => _ = new UnsignedSnowflakeService(optionsB);

        actionA.Should().Throw<ArgumentException>();
        actionB.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void NextId_ShouldBeThreadSafeAndUnique()
    {
        const int threadCount = 8;
        const int idsPerThread = 1000;
        List<Thread> threads = [];
        ConcurrentBag<ulong> ids = [];

        for (int i = 0; i < threadCount; i++)
        {
            Thread thread = new(() =>
            {
                for (int j = 0; j < idsPerThread; j++)
                {
                    ulong id = _service.NextId();
                    ids.Add(id);
                }
            });

            threads.Add(thread);
            thread.Start();
        }

        foreach (Thread thread in threads)
        {
            thread.Join();
        }

        ids.Count.Should().Be(threadCount * idsPerThread);
        ids.Distinct().Count().Should().Be(ids.Count);
    }

    [Fact]
    public void NextId_ShouldEncodeCorrectTimestampAndWorkerId()
    {
        ulong id = _service.NextId();

        (long timestamp, long workerId, long sequence) = _service.Decode(id);

        workerId.Should().Be(1L);
        timestamp.Should().BeGreaterThan(1735689600000L);
        sequence.Should().BeGreaterThanOrEqualTo(0L);
    }
}
