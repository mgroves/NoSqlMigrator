using Couchbase;

namespace NoSqlMigrator.Infrastructure;

internal static class MigrationContext
{
    private static List<Func<List<IMigrateCommand>>> _commands = new List<Func<List<IMigrateCommand>>>();
    private static string _migrationName = "Migration runner has not started";
    
    public static async Task RunCommands(IBucket bucket)
    {
        var errorMessages = new List<string>();
        foreach (var command in _commands)
        {
            var actions = command();
            foreach (var action in actions)
            {
                if (!action.IsValid(errorMessages))
                {
                    throw new Exception($"Invalid migration in \"{_migrationName}\": {string.Join(",", errorMessages)}");
                }
                await action.Execute(bucket);
            }
        }
    }

    public static void SetMigrationName(string name)
    {
        _migrationName = name;
    }
    
    public static void AddCommands(Func<List<IMigrateCommand>> buildCommands)
    {
        _commands.Add(buildCommands);
    }

    public static void Clear()
    {
        _commands = new List<Func<List<IMigrateCommand>>>();
    }
}

public interface IMigrateCommand
{
    Task Execute(IBucket bucket);
    bool IsValid(List<string> errorMessages);
}