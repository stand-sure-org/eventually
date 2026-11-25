namespace StandSure.Eventually;

/// <summary>
/// The tenant a change belongs to. Callers say <see cref="Of"/> or <see cref="None"/> on purpose, so a forgotten tenant
/// can't look the same as an entity that has none. No tenant reaches the wire as an unset <c>tenantid</c>.
/// </summary>
public readonly record struct Tenant
{
    private Tenant(Option<string> id)
    {
        this.Id = id;
    }

    /// <summary>The entity has no tenant.</summary>
    public static Tenant None => default;

    /// <summary>The tenant's id, when there is one.</summary>
    public Option<string> Id { get; }

    /// <summary>A tenant with the given id. An empty id fails when the event is created.</summary>
    public static Tenant Of(string id) => new(Option.Some(id));
}
