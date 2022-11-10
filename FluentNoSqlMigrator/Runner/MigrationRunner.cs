using System.Reflection;
using Couchbase;
using Couchbase.Core.Exceptions.KeyValue;
using Couchbase.KeyValue;
using FluentNoSqlMigrator.Infrastructure;

namespace FluentNoSqlMigrator.Runner;

public class MigrationRunner
{
    private RunSettings _settings;

    public async Task Run(Assembly assembly, RunSettings settings)
    {
        _settings = settings;

        // get all the migrations, ordered by attribute
        var migrations = assembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(Migrate)))
            .OrderBy(t => ((Migration)Attribute.GetCustomAttribute(t,typeof(Migration))).MigrationNumber)
            .ToList();
        
        // instantiate each one and run up on down on it
        foreach (var migrate in migrations)
        {
            // has this already been run?
            var migrationNumber = ((Migration)Attribute.GetCustomAttribute(migrate, typeof(Migration))).MigrationNumber;
            if (await IsMigrationAlreadyRun(migrationNumber))
                continue;

            // run everything in context
            var context = new MigrationContext();
            var migration = Activator.CreateInstance(migrate) as Migrate;
            migration.Context = context;
            migration.Up();

            // execute the commands
            await context.RunCommands(settings.Bucket);
            
            // keep a record of successful migration
            await AddMigrationToHistory(migrationNumber);
        }
        
    }

    private async Task AddMigrationToHistory(int migrationNumber)
    {
        var collection = await _settings.Bucket.CollectionAsync("_default");
        try
        {
            await collection.MutateInAsync("FluentMigrationHistory", specs =>
            {
                specs.ArrayAppend("history", migrationNumber, true);
            });
        }
        catch (DocumentNotFoundException)
        {
            await collection.InsertAsync("FluentMigrationHistory",
                new FluentMigrationHistory { History = new List<int> { migrationNumber } });
        }
    }

    private async Task<bool> IsMigrationAlreadyRun(int migrationNumber)
    {
        var collection = await _settings.Bucket.CollectionAsync("_default");
        try
        {
            var migrationDoc = await collection.GetAsync("FluentMigrationHistory");
            var migration = migrationDoc.ContentAs<FluentMigrationHistory>();
            return migration.History.Contains(migrationNumber);
        }
        catch (DocumentNotFoundException)
        {
            return false;
        }
    }
}

public class FluentMigrationHistory
{
    public List<int> History { get; set; }
}

public class RunSettings
{
    public DirectionEnum Direction { get; set; }
    public IBucket Bucket { get; set; }
}

public enum DirectionEnum
{
    Up, Down
}