using AwesomeAssertions;

using Xunit;

using static StandSure.Eventually.Tests.Fixtures;

namespace StandSure.Eventually.Tests;

// T003
public sealed class ChangeVerbTests
{
    public static TheoryData<ChangeVerb, string> Tokens => new()
    {
        { ChangeVerb.None, "none" },
        { ChangeVerb.Create, "create" },
        { ChangeVerb.Update, "update" },
        { ChangeVerb.Patch, "patch" },
        { ChangeVerb.Archive, "archive" },
        { ChangeVerb.Delete, "delete" },
        { ChangeVerb.Purge, "purge" },
    };

    [Theory]
    [MemberData(nameof(Tokens))]
    public void ToToken_DefinedVerb_IsLowercaseName(ChangeVerb verb, string token)
    {
        Ok(ChangeVerbs.ToToken(verb)).Should().Be(token);
    }

    [Theory]
    [MemberData(nameof(Tokens))]
    public void FromToken_Token_RoundTrips(ChangeVerb verb, string token)
    {
        Ok(ChangeVerbs.FromToken(token)).Should().Be(verb);
    }

    [Theory]
    [InlineData("Update")]
    [InlineData("UPDATE")]
    [InlineData("updated")]
    [InlineData("")]
    [InlineData(" update")]
    public void FromToken_AnythingElse_IsVerbUnknown(string token)
    {
        var error = Failed(ChangeVerbs.FromToken(token));

        error.Kind.Should().Be(ErrorKind.Validation);
        error.Code.Should().Be("event.verb.unknown");
    }

    [Fact]
    public void ToToken_UndefinedVerb_IsVerbUnknown()
    {
        var error = Failed(ChangeVerbs.ToToken((ChangeVerb)99));

        error.Kind.Should().Be(ErrorKind.Validation);
        error.Code.Should().Be("event.verb.unknown");
    }
}
