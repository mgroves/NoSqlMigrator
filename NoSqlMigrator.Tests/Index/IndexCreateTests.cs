using NoSqlMigrator.Infrastructure;
using NoSqlMigrator.Tests.Helpers;

namespace NoSqlMigrator.Tests.Index;

[TestFixture]
public class IndexCreateTests : MigrationTestBase<IndexCreateTests_Migrate>
{
    [SetUp]
    public async Task Setup()
    {
        await base.Setup();
    }

    [Test]
    public async Task Can_Create_Index()
    {
        // arrange
        var indexName = "ix_" + _random.String(10);
        IndexCreateTests_Migrate.IndexName = indexName;

        // act
        await RunUp();

        // assert
        Assert.That(await _bucket.DoesIndexExist(indexName), Is.True);
    }
}

[Migration(1000)]
public class IndexCreateTests_Migrate : Migrate
{
    // these only exist to inject names into this class for testing
    public static string IndexName { get; set; }

    public override void Up()
    {
        Create.Index(IndexName)
            .OnDefaultScope()
            .OnDefaultCollection()
            .OnField("foo")
            .OnField("bar");
    }

    public override void Down()
    {
        Delete.Index(IndexName)
            .FromDefaultScope()
            .FromDefaultCollection();
    }
}