using System;
using Projects.Domain;
using ES.Infrastructure.Repository;
using Identity.Domain;
using Xunit;

namespace ES.Infrastructure.Test;

[TestCaseOrderer("ES.Infrastructure.Test.AlphabeticalOrderer", "ES.Infrastructure.Test")]
public class IntegrationTests : IClassFixture<IntegrationTestFixture>
{
    private readonly IntegrationTestFixture _fixture;

    public IntegrationTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }
    //
    // [Fact]
    // public async void Test_001_Hydrate()
    // {
    //     var cer = _fixture.Repository;
    //     var u = User.Initialize(_fixture.UserId, _fixture.CurrentId, "John", "Doe", "js@example.com",
    //         "New description", "");
    //     await cer.AppendAsync(u);
    // }

    [Fact]
    public async void Test_002_Rehydrate()
    {
        var cer = _fixture.Repository;
        var p = await cer.RehydrateAsync(_fixture.CurrentId);
    }

    [Fact]
    public async void Test_003_AppendToExistingProject()
    {
        var cer = _fixture.Repository;
        var user = await cer.RehydrateAsync(_fixture.CurrentId);
        user.UpdateEmail(_fixture.UserId, "johnny.doe@company.com");
        await cer.AppendAsync(user);
    }
}