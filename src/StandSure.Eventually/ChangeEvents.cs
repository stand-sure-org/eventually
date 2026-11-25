using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace StandSure.Eventually;

/// <summary>Pure functions over change events.</summary>
public static class ChangeEvents
{
    /// <summary>
    /// Builds and validates an event. Every problem is reported at once: one Validation error <c>event.invalid</c>
    /// whose <see cref="Error.Errors"/> hold a child per problem.
    /// </summary>
    public static Result<ChangeEvent> Create(EntityChange change, EventContext context)
    {
        List<Error> problems = [.. Problems(change, context)];
        if (problems.Count > 0)
        {
            return Result.Failure<ChangeEvent>(
                Error.Validation("event.invalid", "The change can't be published as an event.", problems));
        }

        return ChangeVerbs.ToToken(change.Verb).Map(token => Build(change, context, token));
    }

    private static ChangeEvent Build(EntityChange change, EventContext context, string token) =>
        new(
            Id: context.Id.ToString(),
            Source: context.Source,
            Subject: $"{change.EntityType}:{token}:{change.EntityId}",
            Time: context.Now.ToUniversalTime(),
            EntityType: change.EntityType,
            EntityId: change.EntityId,
            Action: change.Verb,
            TenantId: change.Tenant.Id,
            Data: change.Data);

    private static IEnumerable<Error> Problems(EntityChange change, EventContext context)
    {
        if (string.IsNullOrWhiteSpace(change.EntityType))
        {
            yield return Error.Validation("event.entity_type.required", "The entity type is required.");
        }
        else if (change.EntityType.Contains(':', StringComparison.Ordinal))
        {
            yield return Error.Validation("event.entity_type.colon", "The entity type can't contain a colon.");
        }

        if (string.IsNullOrWhiteSpace(change.EntityId))
        {
            yield return Error.Validation("event.entity_id.required", "The entity id is required.");
        }

        if (ChangeVerbs.ToToken(change.Verb).Match<Error?>(onFailure: e => e, onSuccess: _ => null) is { } verbError)
        {
            yield return verbError;
        }

        if (change.Tenant.Id.Match(onNone: () => false, onSome: string.IsNullOrWhiteSpace))
        {
            yield return Error.Validation("event.tenant_id.required", "A tenant's id is required; use Tenant.None for no tenant.");
        }

        if (change.Data.Match(onNone: () => 0, onSome: DataBytes) > context.MaxDataBytes)
        {
            yield return Error.Validation("event.data.too_large", $"The audit data is over {context.MaxDataBytes} bytes.");
        }
    }

    private static int DataBytes(JsonElement data) => Encoding.UTF8.GetByteCount(data.GetRawText());
}
