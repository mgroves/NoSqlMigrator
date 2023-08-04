using Couchbase;
using Couchbase.KeyValue;
using NoSqlMigrator.Infrastructure;
using NoSqlMigrator.Runner;

namespace NoSqlMigrator.Tests.DocumentCreate;

public class MultipleDocumentSameKeyspace
{
    private ICluster _cluster;
    private MigrationRunner _runner;
    private RunSettings _settings;
    private ICouchbaseCollection _collection;

    [SetUp]
    public async Task Setup()
    {
        _cluster = await Cluster.ConnectAsync("couchbase://localhost", "Administrator", "password");
        await _cluster.WaitUntilReadyAsync(TimeSpan.FromSeconds(30));

        _runner = new MigrationRunner();
        _settings = new RunSettings();
        _settings.Bucket = await _cluster.BucketAsync("testmigrator");
        _collection = await _settings.Bucket.DefaultCollectionAsync();
    }

    [Test]
    public async Task Multiple_Document_Create()
    {
        // arrange
        _settings.Direction = DirectionEnum.Up;
        var dockey1 = Path.GetRandomFileName();
        var dockey2 = Path.GetRandomFileName();
        MultipleDocumentSameKeyspace_Migrate.Keys = new List<string>
        {
            dockey1, dockey2
        };

        // act
        await _runner.Run(new List<Type> { typeof(MultipleDocumentSameKeyspace_Migrate) }, _settings);

        // assert
        var exists1 = await _collection.ExistsAsync(dockey1);
        var exists2 = await _collection.ExistsAsync(dockey2);
        Assert.That(exists1.Exists, Is.EqualTo(true));
        Assert.That(exists2.Exists, Is.EqualTo(true));

        // cleanup
        _settings.Direction = DirectionEnum.Down;
        await _runner.Run(new List<Type> { typeof(MultipleDocumentSameKeyspace_Migrate) }, _settings);
    }

}

[Migration(100)]
public class MultipleDocumentSameKeyspace_Migrate : Migrate
{
    public static List<string> Keys { get; set; } = new List<string>();

    public override void Up()
    {
        foreach (var key in Keys)
        {
            Insert.Into
                .Scope("_default")
                .Collection("_default")
                .Document(key, new
                {
                    foo = "bar-" + Path.GetRandomFileName()
                });
        }
    }

    public override void Down()
    {
        foreach (var key in Keys)
        {
            Delete.From
                .Scope("_default")
                .Collection("_default")
                .Document(key);
        }
    }
}