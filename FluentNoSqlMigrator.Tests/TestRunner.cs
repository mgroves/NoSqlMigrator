using Couchbase;
using FluentNoSqlMigrator.Runner;
using NUnit.Framework;

namespace FluentNoSqlMigrator.Tests;

[TestFixture]
public class TestRunner
{
    private ICluster _cluster;

    [SetUp]
    public async Task Setup()
    {
        _cluster = await Cluster.ConnectAsync("couchbase://localhost", "Administrator", "password");
    }
    
    [Test]
    public async Task Run_UP_on_test_migrations()
    {
        // arrange
        var runner = new MigrationRunner();
        var settings = new RunSettings();
        settings.Direction = DirectionEnum.Up;

        settings.Bucket = await _cluster.BucketAsync("testmigrator");
        var coll = await settings.Bucket.DefaultCollectionAsync();

        // act
        await runner.Run(typeof(TestMigration_1).Assembly, settings);

        // assert
        Assert.That((await coll.ExistsAsync("FluentMigrationHistory")).Exists, Is.True);
    }

    [Test]
    public async Task Run_DOWN_on_test_migrations()
    {
        // arrange
        var runner = new MigrationRunner();
        var settings = new RunSettings();
        settings.Direction = DirectionEnum.Down;
        
        settings.Bucket = await _cluster.BucketAsync("testmigrator");
        var coll = await settings.Bucket.DefaultCollectionAsync();

        // act
        await runner.Run(typeof(TestMigration_1).Assembly, settings);

        // assert
        Assert.That((await coll.ExistsAsync("FluentMigrationHistory")).Exists, Is.True);
    }

    [TearDown]
    public async Task TearDown()
    {
        await _cluster.DisposeAsync();
    }
}