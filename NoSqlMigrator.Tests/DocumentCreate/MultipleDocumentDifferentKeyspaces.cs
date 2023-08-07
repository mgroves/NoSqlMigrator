using Couchbase.Management.Collections;
using NoSqlMigrator.Runner;
using NoSqlMigrator.Infrastructure;
using NoSqlMigrator.Tests.Helpers;

namespace NoSqlMigrator.Tests.DocumentCreate;

[TestFixture]
public class MultipleDocumentDifferentKeyspaces : MigrationTestBase<MultipleDocumentDifferentKeyspaces_Migrate>
{
    [SetUp]
    public override async Task Setup()
    {
        await base.Setup();
    }

    [Test]
    public async Task CanInsertIntoMultipleKeyspaces()
    {
        // arrange
        // create two separate keyspaces
        var keyspace1Scope = _random.String(10);
        var keyspace1Collection = _random.String(10);
        var keyspace2Scope = _random.String(10);
        var keyspace2Collection = _random.String(10);
        await _collMananger.CreateScopeAsync(keyspace1Scope);
        await _collMananger.CreateScopeAsync(keyspace2Scope);
        await _collMananger.CreateCollectionAsync(new CollectionSpec(keyspace1Scope, keyspace1Collection));
        await _collMananger.CreateCollectionAsync(new CollectionSpec(keyspace2Scope, keyspace2Collection));
        var keyspace1Key = _random.String(10);
        var keyspace2Key = _random.String(10);
        MultipleDocumentDifferentKeyspaces_Migrate.KeySpace1 = (keyspace1Scope, keyspace1Collection, keyspace1Key);
        MultipleDocumentDifferentKeyspaces_Migrate.KeySpace2 = (keyspace2Scope, keyspace2Collection, keyspace2Key);

        // create the SDK objects for assertion
        var scope1 = await _bucket.ScopeAsync(keyspace1Scope);
        var scope2 = await _bucket.ScopeAsync(keyspace2Scope);
        var collection1 = await scope1.CollectionAsync(keyspace1Collection);
        var collection2 = await scope2.CollectionAsync(keyspace2Collection);

        // act
        await RunUp();

        // assert
        var exists1 = await collection1.ExistsAsync(keyspace1Key);
        var exists2 = await collection2.ExistsAsync(keyspace2Key);
        Assert.That(exists1.Exists, Is.True);
        Assert.That(exists2.Exists, Is.True);
    }
}

[Migration(1000)]
public class MultipleDocumentDifferentKeyspaces_Migrate : Migrate
{
    // these tuples are only to inject values into this migration
    public static (string, string, string) KeySpace1 { get; set; }
    public static (string, string, string) KeySpace2 { get; set; }

    public override void Up()
    {
        Insert.Into
            .Scope(KeySpace1.Item1)
            .Collection(KeySpace1.Item2)
            .Document(KeySpace1.Item3, new { foo = Path.GetRandomFileName() });
        Insert.Into
            .Scope(KeySpace2.Item1)
            .Collection(KeySpace2.Item2)
            .Document(KeySpace2.Item3, new { foo = Path.GetRandomFileName() });

    }

    public override void Down()
    {
        Delete.From
            .Scope(KeySpace1.Item1)
            .Collection(KeySpace1.Item2)
            .Document(KeySpace1.Item3);
        Delete.From
            .Scope(KeySpace2.Item1)
            .Collection(KeySpace2.Item2)
            .Document(KeySpace2.Item3);
    }
}

