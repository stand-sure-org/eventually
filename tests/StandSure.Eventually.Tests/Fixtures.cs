using System;
using System.Text.Json;

using Xunit.Sdk;

namespace StandSure.Eventually.Tests;

internal static class Fixtures
{
    internal static readonly Uri Source = new("urn:example:billing");

    internal static readonly DateTimeOffset Now = new(2026, 11, 25, 14, 0, 0, TimeSpan.Zero);

    internal static readonly Guid Id = Guid.Parse("0193b5c2-7d1e-7c3a-9f00-1a2b3c4d5e6f");

    internal static EventContext Context() => new(Source, Now, Id);

    internal static EntityChange Change(
        string entityType = "StandSure.Billing.Invoice",
        string entityId = "42",
        ChangeVerb verb = ChangeVerb.Update,
        Tenant? tenant = null,
        Option<JsonElement> data = default) =>
        new(entityType, entityId, verb, tenant ?? Tenant.Of("t1"), data);

    internal static JsonElement Json(string json)
    {
        using var document = JsonDocument.Parse(json);
        return document.RootElement.Clone();
    }

    internal static T Ok<T>(Result<T> result) =>
        result.Match<T>(
            onFailure: e => throw new XunitException($"Expected success, got {e.Kind} {e.Code}: {e.Message}"),
            onSuccess: v => v);

    internal static Error Failed<T>(Result<T> result) =>
        result.Match<Error>(
            onFailure: e => e,
            onSuccess: v => throw new XunitException($"Expected failure, got {v}"));
}
