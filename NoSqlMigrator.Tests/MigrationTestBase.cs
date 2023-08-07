using Couchbase;
using Couchbase.KeyValue;
using Couchbase.Management.Collections;
using NoSqlMigrator.Infrastructure;
using NoSqlMigrator.Runner;

namespace NoSqlMigrator.Tests;

public abstract class MigrationTestBase<T> where T : Migrate
{
    protected ICluster _cluster;
    protected MigrationRunner _runner;
    protected RunSettings _settings;
    protected ICouchbaseCollection _collection;
    protected ICouchbaseCollectionManager _collMananger;
    protected Random _random;
    protected IBucket _bucket;

    [SetUp]
    public virtual async Task Setup()
    {
        _cluster = await Cluster.ConnectAsync("couchbase://localhost", "Administrator", "password");
        await _cluster.WaitUntilReadyAsync(TimeSpan.FromSeconds(30));

        _runner = new MigrationRunner();
        _settings = new RunSettings();
        _settings.Bucket = await _cluster.BucketAsync("testmigrator");
        _bucket = _settings.Bucket;
        _collMananger = _settings.Bucket.Collections;

        _random = new Random();
    }

    [TearDown]
    public async Task Cleanup()
    {
        await RunDown();

        await _cluster.DisposeAsync();
    }

    protected async Task RunUp()
    {
        _settings.Direction = DirectionEnum.Up;
        await _runner.Run(new List<Type>
        {
            typeof(T)
        }, _settings);

    }

    protected async Task RunDown() 
    {
        _settings.Direction = DirectionEnum.Down;
        await _runner.Run(new List<Type>
        {
            typeof(T)
        }, _settings);
    }
}