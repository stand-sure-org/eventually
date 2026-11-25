using System;

namespace StandSure.Eventually;

/// <summary>Everything impure an event needs, read once by the caller so that building the event is a pure function.</summary>
/// <param name="Source">The producing service's CloudEvents source.</param>
/// <param name="Now">The current time, from <see cref="TimeProvider"/>.</param>
/// <param name="Id">A fresh UUIDv7, from <see cref="Guid.CreateVersion7()"/>.</param>
public sealed record EventContext(Uri Source, DateTimeOffset Now, Guid Id)
{
    /// <summary>The default cap on audit data, in UTF-8 bytes.</summary>
    public const int DefaultMaxDataBytes = 64 * 1024;

    /// <summary>The cap on audit data, in UTF-8 bytes. Larger data is Validation <c>event.data.too_large</c>.</summary>
    public int MaxDataBytes { get; init; } = DefaultMaxDataBytes;
}
