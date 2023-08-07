using NoSqlMigrator.Extensions;
using NoSqlMigrator.Infrastructure;
using NoSqlMigrator.Tests.Helpers;

namespace NoSqlMigrator.Tests.Collection;

[TestFixture]
public class CreateCollectionInDefaultScopeTests : MigrationTestBase<CreateCollectionInDefaultScope_Migrate>
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
        var collectionName = _random.String(10);
        CreateCollectionInDefaultScope_Migrate.CollectionName = collectionName;

        // act
        await RunUp();

        // assert
        Assert.That(await _bucket.DoesCollectionExist("_default", collectionName), Is.True);
    }
}

[Migration(1000)]
public class CreateCollectionInDefaultScope_Migrate : Migrate
{
    // these only exist to inject names into this class for testing
    public static string CollectionName { get; set; }

    public override void Up()
    {
        Create.Collection(CollectionName)
            .InDefaultScope();
    }

    public override void Down()
    {
        Delete.Collection(CollectionName)
            .FromDefaultScope();
    }
}