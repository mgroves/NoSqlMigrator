using Couchbase;

namespace FluentNoSqlMigrator.Infrastructure;

public static class MigrationContext
{
    private static List<Func<List<IMigrateCommand>>> _commands = new List<Func<List<IMigrateCommand>>>();

    public static async Task RunCommands(IBucket bucket)
    {
        foreach (var command in _commands)
        {
            var actions = command();
            foreach (var action in actions)
            {
                await action.Execute(bucket);
            }
        }
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
}