using System.Linq;

using AwesomeAssertions;

using Xunit;

using static StandSure.Eventually.Tests.Fixtures;

namespace StandSure.Eventually.Tests;

// T006
public sealed class ValidationTests
{
    [Fact]
    public void Create_SeveralBadFields_OneValidationErrorWithEveryChild()
    {
        var change = Change(entityType: "", entityId: "", verb: (ChangeVerb)99, tenant: Tenant.Of(""));

        var error = Failed(ChangeEvents.Create(change, Context()));

        error.Kind.Should().Be(ErrorKind.Validation);
        error.Code.Should().Be("event.invalid");
        error.Errors.Select(e => e.Code).Should().BeEquivalentTo(
            "event.entity_type.required",
            "event.entity_id.required",
            "event.verb.unknown",
            "event.tenant_id.required");
        error.Errors.Should().OnlyContain(e => e.Kind == ErrorKind.Validation);
    }

    [Fact]
    public void Create_OneBadField_StillWrapsItInEventInvalid()
    {
        var error = Failed(ChangeEvents.Create(Change(entityId: ""), Context()));

        error.Code.Should().Be("event.invalid");
        error.Errors.Should().ContainSingle().Which.Code.Should().Be("event.entity_id.required");
    }
}
