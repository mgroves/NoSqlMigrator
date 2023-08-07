using NoSqlMigrator.Extensions;
using NoSqlMigrator.Infrastructure;
using NoSqlMigrator.Tests.Helpers;

namespace NoSqlMigrator.Tests.Collection;

[TestFixture]
public class CreateScopeAndCollectionTests : MigrationTestBase<CreateScopeAndCollection_Migrate>
{
    [SetUp]
    public async Task Setup()
    {
        await base.Setup();
    }

    [Test]
    public async Task ScopeAndCollectionShouldBeCreated()
    {
        // arrange
        var scopeName = _random.String(10);
        var collectionName = _random.String(10);
        CreateScopeAndCollection_Migrate.ScopeName = scopeName;
        CreateScopeAndCollection_Migrate.CollectionName = collectionName;

        // act
        await RunUp();

        // assert
        Assert.That(await _bucket.DoesCollectionExist(scopeName, collectionName), Is.True);
    }
}

[Migration(1000)]
public class CreateScopeAndCollection_Migrate : Migrate
{
    // these only exist to inject names into this class for testing
    public static string CollectionName { get; set; }
    public static string ScopeName { get; set; }

    public override void Up()
    {
        Create.Scope(ScopeName)
            .WithCollection(CollectionName);
    }

    public override void Down()
    {
        Delete.Scope(ScopeName);
    }
}