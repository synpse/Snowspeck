// Copyright (c) 2025 Tiago Ferreira Alves. Licensed under the MIT License.
using Microsoft.Extensions.DependencyInjection;
using Snowspeck.Interfaces;
using Snowspeck.Services;

namespace Snowspeck.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSnowspeck(
        this IServiceCollection services, Action<SnowflakeOptions> configure)
    {
        services.Configure(configure);
        services.AddSingleton<ISnowflakeGenerator<long>, SignedSnowflakeService>();
        return services;
    }
    
    public static IServiceCollection AddSnowspeckUnsigned(
        this IServiceCollection services, Action<SnowflakeOptions> configure)
    {
        services.Configure(configure);
        services.AddSingleton<ISnowflakeGenerator<ulong>, UnsignedSnowflakeService>();
        return services;
    }
}
