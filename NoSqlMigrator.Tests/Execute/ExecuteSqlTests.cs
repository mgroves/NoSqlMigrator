using NoSqlMigrator.Extensions;
using NoSqlMigrator.Infrastructure;
using NoSqlMigrator.Tests.Helpers;

namespace NoSqlMigrator.Tests.Collection;

[TestFixture]
public class ExecuteSqlTests : MigrationTestBase<ExecuteSqlTests_Migrate>
{
    [SetUp]
    public async Task Setup()
    {
        await base.Setup();
    }

    [Test]
    public async Task Can_Run_Inline_Sql()
    {
        // arrange
        var fooValue = _random.String(64);
        var key = _random.String(10);
        ExecuteSqlTests_Migrate.BucketName = _bucket.Name;
        ExecuteSqlTests_Migrate.Key = key;
        ExecuteSqlTests_Migrate.ValueToInsert = fooValue;
        var defaultCollection = await _bucket.DefaultCollectionAsync();

        // act
        await RunUp();
        var exists = await defaultCollection.ExistsAsync(key);

        // assert
        Assert.That(exists.Exists, Is.True);
    }
}

[Migration(1000)]
public class ExecuteSqlTests_Migrate : Migrate
{
    // these only exist to inject names into this class for testing
    public static string BucketName { get; set; }
    public static string Key { get; set; }
    public static string ValueToInsert { get; set; }

    public override void Up()
    {
        Execute.Sql($@"INSERT INTO `{BucketName}`.`_default`.`_default` (KEY, VALUE)
            VALUES
            (
                ""{Key}"",
                {{ ""foo"" :""{ValueToInsert}"" }}
            );");
    }

    public override void Down()
    {
        Execute.Sql($"DELETE FROM `{BucketName}`.`_default`.`_default` USE KEYS [\"{Key}\"];");
    }
}