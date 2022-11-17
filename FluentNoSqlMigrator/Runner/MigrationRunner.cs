using System.Reflection;
using Couchbase;
using Couchbase.Core.Exceptions.KeyValue;
using Couchbase.KeyValue;
using FluentNoSqlMigrator.Infrastructure;

namespace FluentNoSqlMigrator.Runner;

public class MigrationRunner
{
    private RunSettings _settings;
    private List<Type> _migrateClasses;

    public async Task Run(Assembly assembly, RunSettings settings)
    {
        _settings = settings;
        _migrateClasses = assembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(Migrate)))
            .ToList();

        ValidateTypes();

        if (_settings.Direction == DirectionEnum.Up)
            await RunUp();
        else
            await RunDown();
    }

    // detect if there's a missing attribute and produce an appropriate error message
    private void ValidateTypes()
    {
        var typesWithoutAttributes = _migrateClasses
            .Where(t => (Attribute.GetCustomAttribute(t, typeof(Migration)) is null));

        if (typesWithoutAttributes.Any())
            throw new Exception("Migration attributes are required. These migration classes do not have attributes: "
                                + string.Join(",", typesWithoutAttributes.Select(t => t.FullName)));
    }

    public async Task Run(List<Type> migrateClasses, RunSettings settings)
    {
        _settings = settings;
        _migrateClasses = migrateClasses;
        
        ValidateTypes();

        if (_settings.Direction == DirectionEnum.Up)
            await RunUp();
        else
            await RunDown();
    }

    private async Task RunUp()
    {
        // get all the migrations, ordered by attribute
        var migrations = _migrateClasses
            .OrderBy(t => ((Migration)Attribute.GetCustomAttribute(t, typeof(Migration))).MigrationNumber)
            .ToList();

        // instantiate each one and run up on down on it
        foreach (var migrate in migrations)
        {
            MigrationContext.SetMigrationName(migrate.FullName);
            
            // has this already been run?
            var migrationNumber = ((Migration)Attribute.GetCustomAttribute(migrate, typeof(Migration))).MigrationNumber;
            if (await IsMigrationAlreadyRun(migrationNumber))
                continue;

            // run everything, it will put commands into MigrationContext
            var migration = Activator.CreateInstance(migrate) as Migrate;
            migration.Up();

            // execute the commands
            await MigrationContext.RunCommands(_settings.Bucket);

            // for UP: keep a record of successful migration
            await AddMigrationToHistory(migrationNumber);

            // clear context for next migration
            MigrationContext.Clear();
        }
    }

    private async Task RunDown()
    {
        // get all the migrations, ordered by attribute DESCENDING
        var migrations = _migrateClasses
            .OrderByDescending(t => ((Migration)Attribute.GetCustomAttribute(t, typeof(Migration))).MigrationNumber)
            .ToList();

        // instantiate each one and run up on down on it
        foreach (var migrate in migrations)
        {
            MigrationContext.SetMigrationName(migrate.FullName);
            
            // has this already been run?
            var migrationNumber = ((Migration)Attribute.GetCustomAttribute(migrate, typeof(Migration))).MigrationNumber;
            var alreadyRun = await IsMigrationAlreadyRun(migrationNumber);
            if (!alreadyRun)
                continue;

            // run everything in context
            var migration = Activator.CreateInstance(migrate) as Migrate;
            migration.Down();

            // execute the commands
            await MigrationContext.RunCommands(_settings.Bucket);

            // for UP: keep a record of successful migration
            await RollbackMigrationFromHistory(migrationNumber);
            
            MigrationContext.Clear();
        }
    }

    private async Task RollbackMigrationFromHistory(int migrationNumber)
    {
        var collection = await _settings.Bucket.CollectionAsync("_default");
        try
        {
            // get the doc
            var doc = await collection.GetAsync("FluentMigrationHistory");
            var fluentMigrationHistory = doc.ContentAs<FluentMigrationHistory>();

            // remove the number from history
            fluentMigrationHistory?.History.Remove(migrationNumber);

            // save the doc (replace the whole thing, since there's no subdoc operation to remove from an array
            await collection.ReplaceAsync("FluentMigrationHistory", fluentMigrationHistory);
        }
        catch (DocumentNotFoundException)
        {
            // if there's no doc, that's fine
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
    /// <summary>
    /// Limit is the HIGHEST that the migration will run UP to
    /// Or the LOWEST that the migration will run DOWN to
    /// If it's not set, ALL migrations will be run
    /// </summary>
    public int? Limit { get; set; }
}

public enum DirectionEnum
{
    Up, Down
}