using Couchbase;
using FluentNoSqlMigrator.Runner;
using NUnit.Framework;

namespace FluentNoSqlMigrator.Tests;

[TestFixture]
public class TestRunner
{
    [Test]
    public async Task Run_UP_on_test_migrations()
    {
        // arrange
        var runner = new MigrationRunner();
        var settings = new RunSettings();
        settings.Direction = DirectionEnum.Up;

        var cluster = await Cluster.ConnectAsync("couchbase://localhost", "Administrator", "password");
        settings.Bucket = await cluster.BucketAsync("testmigrator");

        // act
        await runner.Run(typeof(TestMigration_1).Assembly, settings);

        // assert
        // TODO

        // cleanup
        await cluster.DisposeAsync();
    }
}