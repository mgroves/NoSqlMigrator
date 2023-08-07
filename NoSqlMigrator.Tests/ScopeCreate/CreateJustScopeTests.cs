using NoSqlMigrator.Extensions;
using NoSqlMigrator.Infrastructure;
using NoSqlMigrator.Tests.Helpers;

namespace NoSqlMigrator.Tests.ScopeCreate;

[TestFixture]
public class CreateJustScopeTests : MigrationTestBase<CreateOnlyScope_Migrate>
{
    [SetUp]
    public override async Task Setup()
    {
        await base.Setup();
    }

    [Test]
    public async Task CreateOnlyScope()
    {
        // arrange
        var scopeName = _random.String(10);
        CreateOnlyScope_Migrate.ScopeName = scopeName;

        // act
        await RunUp();

        // assert
        Assert.That(await _bucket.DoesScopeExist(scopeName), Is.True);
    }
}

[Migration(1000)]
public class CreateOnlyScope_Migrate : Migrate
{
    public static string ScopeName { get; set; }

    public override void Up()
    {
        Create.Scope(ScopeName);
    }

    public override void Down()
    {
        Delete.Scope(ScopeName);
    }
}