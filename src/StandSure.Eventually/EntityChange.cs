using System.Text.Json;

namespace StandSure.Eventually;

/// <summary>A persisted mutation to announce: which entity, what happened, whose it is, and optional audit data.</summary>
/// <param name="EntityType">The fully qualified entity type name. Dots are fine; a colon is not.</param>
/// <param name="EntityId">The entity's id, an opaque non-empty string.</param>
/// <param name="Verb">What happened.</param>
/// <param name="Tenant">The tenant, or <see cref="Tenant.None"/>.</param>
/// <param name="Data">Optional JSON for audit consumers. Consumers otherwise fetch fresh data themselves.</param>
public sealed record EntityChange(string EntityType, string EntityId, ChangeVerb Verb, Tenant Tenant, Option<JsonElement> Data);
