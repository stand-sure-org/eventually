using AwesomeAssertions;

using Xunit;

using static StandSure.Eventually.Tests.Fixtures;

namespace StandSure.Eventually.Tests;

// T004
public sealed class EntityNameTests
{
    [Theory]
    [InlineData("invoice")]
    [InlineData("StandSure.Billing.Invoice")]
    [InlineData("billing.invoice.line-item")]
    [InlineData("Café.Ünïcode")]
    [InlineData("a b*c>d#e%f")]
    public void EntityType_AnyNonEmptyStringWithoutColon_IsAccepted(string entityType)
    {
        Ok(ChangeEvents.Create(Change(entityType: entityType), Context())).EntityType.Should().Be(entityType);
    }

    [Theory]
    [InlineData("42")]
    [InlineData("0193b5c2-7d1e-7c3a-9f00-1a2b3c4d5e6f")]
    [InlineData("a.b")]
    [InlineData("chris@example.com")]
    [InlineData("urn:isbn:0451450523")]
    [InlineData("café-№7")]
    public void EntityId_AnyNonEmptyString_IsAccepted(string entityId)
    {
        Ok(ChangeEvents.Create(Change(entityId: entityId), Context())).EntityId.Should().Be(entityId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void EntityType_Empty_IsRequired(string entityType)
    {
        var error = Failed(ChangeEvents.Create(Change(entityType: entityType), Context()));

        error.Errors.Should().ContainSingle(e => e.Code == "event.entity_type.required");
    }

    [Fact]
    public void EntityType_WithColon_IsRejected()
    {
        var error = Failed(ChangeEvents.Create(Change(entityType: "billing:invoice"), Context()));

        error.Errors.Should().ContainSingle(e => e.Code == "event.entity_type.colon");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void EntityId_Empty_IsRequired(string entityId)
    {
        var error = Failed(ChangeEvents.Create(Change(entityId: entityId), Context()));

        error.Errors.Should().ContainSingle(e => e.Code == "event.entity_id.required");
    }
}
