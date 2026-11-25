namespace StandSure.Eventually;

/// <summary>Red-phase placeholder: returns a failure so the tests compile and fail. Delete once nothing uses it.</summary>
internal static class NotYet
{
    internal static Result<T> Implemented<T>(string member) =>
        Result.Failure<T>(Error.Unexpected("eventually.not_implemented", $"{member} is not implemented yet."));
}
