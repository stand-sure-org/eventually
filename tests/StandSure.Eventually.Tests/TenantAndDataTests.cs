using AwesomeAssertions;

using Xunit;

using static StandSure.Eventually.Tests.Fixtures;

namespace StandSure.Eventually.Tests;

// T007
public sealed class TenantAndDataTests
{
    [Fact]
    public void TenantNone_HasNoId()
    {
        Tenant.None.Id.IsNone.Should().BeTrue();
    }

    [Fact]
    public void TenantOf_HoldsTheId()
    {
        Tenant.Of("t1").Id.Should().Be(Option.Some("t1"));
    }

    [Fact]
    public void TenantOf_IsNotTenantNone()
    {
        Tenant.Of("t1").Should().NotBe(Tenant.None);
    }

    [Fact]
    public void Create_TenantNone_LeavesTenantIdUnset()
    {
        Ok(ChangeEvents.Create(Change(tenant: Tenant.None), Context())).TenantId.IsNone.Should().BeTrue();
    }

    [Fact]
    public void Create_TenantOfEmpty_IsTenantIdRequired()
    {
        var error = Failed(ChangeEvents.Create(Change(tenant: Tenant.Of("")), Context()));

        error.Errors.Should().ContainSingle(e => e.Code == "event.tenant_id.required");
    }

    [Fact]
    public void Create_WithData_KeepsIt()
    {
        var data = Json("""{"total":12.5,"currency":"USD"}""");

        var e = Ok(ChangeEvents.Create(Change(data: Option.Some(data)), Context()));

        e.Data.Match(onNone: () => string.Empty, onSome: d => d.GetRawText()).Should().Be(data.GetRawText());
    }

    [Fact]
    public void Create_DataAtTheCap_IsAccepted()
    {
        var data = Json("\"" + new string('x', 14) + "\""); // 16 UTF-8 bytes with the quotes

        Ok(ChangeEvents.Create(Change(data: Option.Some(data)), Context() with { MaxDataBytes = 16 }));
    }

    [Fact]
    public void Create_DataOverTheCap_IsTooLarge()
    {
        var data = Json("\"" + new string('x', 15) + "\""); // 17 UTF-8 bytes

        var error = Failed(ChangeEvents.Create(Change(data: Option.Some(data)), Context() with { MaxDataBytes = 16 }));

        error.Errors.Should().ContainSingle(e => e.Code == "event.data.too_large");
    }
}
