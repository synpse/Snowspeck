// Copyright (c) 2025 Tiago Ferreira Alves. Licensed under the MIT License.

using Snowspeck.Metadata;

namespace Snowspeck.Interfaces;

public interface ISnowflakeGenerator<T>
{
    T NextId();
    SnowflakeMetadata Decode(T id);
}
