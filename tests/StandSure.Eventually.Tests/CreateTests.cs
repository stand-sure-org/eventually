using System;

using AwesomeAssertions;

using Xunit;

using static StandSure.Eventually.Tests.Fixtures;

namespace StandSure.Eventually.Tests;

// T005
public sealed class CreateTests
{
    [Fact]
    public void Create_ValidChange_BuildsEveryAttribute()
    {
        var e = Ok(ChangeEvents.Create(Change(), Context()));

        e.Id.Should().Be("0193b5c2-7d1e-7c3a-9f00-1a2b3c4d5e6f");
        e.Source.Should().Be(Source);
        e.Subject.Should().Be("StandSure.Billing.Invoice:update:42");
        e.Time.Should().Be(Now);
        e.Time.Offset.Should().Be(TimeSpan.Zero);
        e.EntityType.Should().Be("StandSure.Billing.Invoice");
        e.EntityId.Should().Be("42");
        e.Action.Should().Be(ChangeVerb.Update);
        e.TenantId.Should().Be(Option.Some("t1"));
        e.Data.IsNone.Should().BeTrue();
    }

    [Fact]
    public void Create_IdWithColons_KeepsTheIdWholeAtTheEndOfTheSubject()
    {
        var e = Ok(ChangeEvents.Create(Change(entityType: "Book", entityId: "urn:isbn:0451450523", verb: ChangeVerb.Delete), Context()));

        e.Subject.Should().Be("Book:delete:urn:isbn:0451450523");
    }

    [Fact]
    public void Create_NonUtcNow_IsStoredAsUtc()
    {
        var local = new DateTimeOffset(2026, 11, 25, 9, 0, 0, TimeSpan.FromHours(-5));

        var e = Ok(ChangeEvents.Create(Change(), Context() with { Now = local }));

        e.Time.Offset.Should().Be(TimeSpan.Zero);
        e.Time.Should().Be(local);
    }

    [Fact]
    public void Create_SameInputs_SameEvent()
    {
        Ok(ChangeEvents.Create(Change(), Context())).Should().Be(Ok(ChangeEvents.Create(Change(), Context())));
    }

    [Fact]
    public void Type_IsTheFixedConstant()
    {
        ChangeEvent.Type.Should().Be("standsure.eventually");
        ChangeEvent.SpecVersion.Should().Be("1.0");
    }
}
