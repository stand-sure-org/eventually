namespace StandSure.Eventually;

/// <summary>Maps <see cref="ChangeVerb"/> to and from its lowercase wire token.</summary>
public static class ChangeVerbs
{
    /// <summary>The lowercase token for a defined verb, such as <c>update</c>.</summary>
    public static Result<string> ToToken(ChangeVerb verb) => NotYet.Implemented<string>(nameof(ToToken));

    /// <summary>Parses an exact lowercase token. Anything else is Validation <c>event.verb.unknown</c>.</summary>
    public static Result<ChangeVerb> FromToken(string token) => NotYet.Implemented<ChangeVerb>(nameof(FromToken));
}
