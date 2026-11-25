using System;

using AwesomeAssertions;

using Xunit;

using static StandSure.Eventually.Tests.Fixtures;

namespace StandSure.Eventually.Tests;

// T007a
public sealed class NoneVerbTests
{
    [Fact]
    public void Create_None_BuildsLikeAnyOtherVerb()
    {
        var e = Ok(ChangeEvents.Create(Change(verb: ChangeVerb.None), Context()));

        e.Action.Should().Be(ChangeVerb.None);
        e.Subject.Should().Be("StandSure.Billing.Invoice:none:42");
        e.Id.Should().Be(Id.ToString());
        e.Time.Should().Be(Now);
    }

    [Fact]
    public void Create_TwoNoneEventsForOneEntity_DistinctIdsAndNonDecreasingTimes()
    {
        var first = Ok(ChangeEvents.Create(Change(verb: ChangeVerb.None), Context()));
        var later = Context() with { Id = Guid.Parse("0193b5c2-7d1f-7c3a-9f00-1a2b3c4d5e70"), Now = Now.AddSeconds(1) };

        var second = Ok(ChangeEvents.Create(Change(verb: ChangeVerb.None), later));

        second.Id.Should().NotBe(first.Id);
        second.Time.Should().BeOnOrAfter(first.Time);
    }
}
