using System;
using System.Text.Json;

namespace StandSure.Eventually;

/// <summary>
/// One CloudEvents 1.0 event for one persisted mutation. <see cref="Subject"/> is
/// <c>{entitytype}:{action}:{entityid}</c>; <see cref="EntityType"/>, <see cref="EntityId"/>, <see cref="Action"/> and
/// <see cref="TenantId"/> are the <c>entitytype</c>, <c>entityid</c>, <c>action</c> and <c>tenantid</c> extension attributes.
/// </summary>
public sealed record ChangeEvent(
    string Id,
    Uri Source,
    string Subject,
    DateTimeOffset Time,
    string EntityType,
    string EntityId,
    ChangeVerb Action,
    Option<string> TenantId,
    Option<JsonElement> Data)
{
    /// <summary>The CloudEvents <c>specversion</c>.</summary>
    public const string SpecVersion = "1.0";

    /// <summary>The CloudEvents <c>type</c> of every event. Also the root of every topic.</summary>
    public const string Type = "standsure.eventually";
}
