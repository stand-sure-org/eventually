namespace StandSure.Eventually;

/// <summary>Maps <see cref="ChangeVerb"/> to and from its lowercase wire token.</summary>
public static class ChangeVerbs
{
    /// <summary>The lowercase token for a defined verb, such as <c>update</c>.</summary>
    public static Result<string> ToToken(ChangeVerb verb) => verb switch
    {
        ChangeVerb.None => "none",
        ChangeVerb.Create => "create",
        ChangeVerb.Update => "update",
        ChangeVerb.Patch => "patch",
        ChangeVerb.Archive => "archive",
        ChangeVerb.Delete => "delete",
        ChangeVerb.Purge => "purge",
        _ => Unknown<string>($"{(int)verb} is not a defined verb."),
    };

    /// <summary>Parses an exact lowercase token. Anything else is Validation <c>event.verb.unknown</c>.</summary>
    public static Result<ChangeVerb> FromToken(string token) => token switch
    {
        "none" => ChangeVerb.None,
        "create" => ChangeVerb.Create,
        "update" => ChangeVerb.Update,
        "patch" => ChangeVerb.Patch,
        "archive" => ChangeVerb.Archive,
        "delete" => ChangeVerb.Delete,
        "purge" => ChangeVerb.Purge,
        _ => Unknown<ChangeVerb>($"'{token}' is not a verb token."),
    };

    private static Result<T> Unknown<T>(string message) =>
        Result.Failure<T>(Error.Validation("event.verb.unknown", message));
}
