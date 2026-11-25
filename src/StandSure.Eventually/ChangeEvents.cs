namespace StandSure.Eventually;

/// <summary>Pure functions over change events.</summary>
public static class ChangeEvents
{
    /// <summary>
    /// Builds and validates an event. Every problem is reported at once: one Validation error <c>event.invalid</c>
    /// whose <see cref="Error.Errors"/> hold a child per problem.
    /// </summary>
    public static Result<ChangeEvent> Create(EntityChange change, EventContext context) =>
        NotYet.Implemented<ChangeEvent>(nameof(Create));
}
