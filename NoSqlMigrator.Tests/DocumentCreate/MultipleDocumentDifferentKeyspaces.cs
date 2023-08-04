using Couchbase.KeyValue;
using Couchbase;
using Couchbase.Management.Buckets;
using Couchbase.Management.Collections;
using NoSqlMigrator.Runner;
using NoSqlMigrator.Infrastructure;

namespace NoSqlMigrator.Tests.DocumentCreate;

[TestFixture]
public class MultipleDocumentDifferentKeyspaces
{
    private ICluster _cluster;
    private MigrationRunner _runner;
    private RunSettings _settings;
    private ICouchbaseCollection _collection;
    private ICouchbaseCollectionManager _collMananger;

    [SetUp]
    public async Task Setup()
    {
        _cluster = await Cluster.ConnectAsync("couchbase://localhost", "Administrator", "password");
        await _cluster.WaitUntilReadyAsync(TimeSpan.FromSeconds(30));

        _runner = new MigrationRunner();
        _settings = new RunSettings();
        _settings.Bucket = await _cluster.BucketAsync("testmigrator");
        _collMananger = _settings.Bucket.Collections;
    }

    [Test]
    public async Task CanInsertIntoMultipleKeyspaces()
    {
        // arrange
        // create two separate keyspaces
        var keyspace1Scope = GenerateRandomString(10);
        var keyspace1Collection = GenerateRandomString(10);
        var keyspace2Scope = GenerateRandomString(10);
        var keyspace2Collection = GenerateRandomString(10);
        await _collMananger.CreateScopeAsync(keyspace1Scope);
        await _collMananger.CreateScopeAsync(keyspace2Scope);
        await _collMananger.CreateCollectionAsync(new CollectionSpec(keyspace1Scope, keyspace1Collection));
        await _collMananger.CreateCollectionAsync(new CollectionSpec(keyspace2Scope, keyspace2Collection));
        var keyspace1Key = GenerateRandomString(10);
        var keyspace2Key = GenerateRandomString(10);
        MultipleDocumentDifferentKeyspaces_Migrate.KeySpace1 = (keyspace1Scope, keyspace1Collection, keyspace1Key);
        MultipleDocumentDifferentKeyspaces_Migrate.KeySpace2 = (keyspace2Scope, keyspace2Collection, keyspace2Key);

        // create the SDK objects for assertion
        var scope1 = await _settings.Bucket.ScopeAsync(keyspace1Scope);
        var scope2 = await _settings.Bucket.ScopeAsync(keyspace2Scope);
        var collection1 = await scope1.CollectionAsync(keyspace1Collection);
        var collection2 = await scope2.CollectionAsync(keyspace2Collection);

        // act
        await _runner.Run(new List<Type>
        {
            typeof(MultipleDocumentDifferentKeyspaces_Migrate)
        }, _settings);

        // assert
        var exists1 = await collection1.ExistsAsync(keyspace1Key);
        var exists2 = await collection2.ExistsAsync(keyspace2Key);
        Assert.That(exists1.Exists, Is.True);
        Assert.That(exists2.Exists, Is.True);

        // cleanup
        _settings.Direction = DirectionEnum.Down;
        await _runner.Run(new List<Type>
        {
            typeof(MultipleDocumentDifferentKeyspaces_Migrate)
        }, _settings);

    }


    public static string GenerateRandomString(int size)
    {
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        return new string(Enumerable.Range(0, size)
            .Select(_ => chars[random.Next(chars.Length)]).ToArray());
    }
}

[Migration(1000)]
public class MultipleDocumentDifferentKeyspaces_Migrate : Migrate
{
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

