using System.Reflection;
using Couchbase;
using NoSqlMigrator.Infrastructure;
using NoSqlMigrator.Runner;

namespace NoSqlMigrator.Tests;

[TestFixture]
public class TestRunner
{
    private ICluster _cluster;
    private List<Type> _migrationsToTest;

    [SetUp]
    public async Task Setup()
    {
        _cluster = await Cluster.ConnectAsync("couchbase://localhost", "Administrator", "password");
        await _cluster.WaitUntilReadyAsync(TimeSpan.FromSeconds(30));

        // only run migrations under 1000 (until I get all of new test structure in place)
        _migrationsToTest = Assembly.GetAssembly(typeof(TestMigration_1)).GetTypes()
            .Where(t => !t.IsAbstract)
            .Where(t => t.IsSubclassOf(typeof(Migrate)))
            .Where(t => {
                var migrationAttribute = t.GetCustomAttribute<Migration>();
                if (migrationAttribute != null)
                {
                    return migrationAttribute.MigrationNumber < 1000;
                }
                // If the Migration attribute is not present, include the type
                return true;
            })
            .ToList();
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
        await runner.Run(_migrationsToTest, settings);

        // assert
        Assert.That((await coll.ExistsAsync("MigrationHistory")).Exists, Is.True);
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
        await runner.Run(_migrationsToTest, settings);

        // assert
        Assert.That((await coll.ExistsAsync("MigrationHistory")).Exists, Is.True);
    }

    [TearDown]
    public async Task TearDown()
    {
        await _cluster.DisposeAsync();
    }
}