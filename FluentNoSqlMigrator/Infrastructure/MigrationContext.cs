using Couchbase;

namespace FluentNoSqlMigrator.Infrastructure;

public class MigrationContext
{
    private readonly List<IMigrateCommand> _commands;

    public MigrationContext()
    {
        _commands = new List<IMigrateCommand>();
    }
    
    public void AddCommand(IMigrateCommand migrateCommand)
    {
        _commands.Add(migrateCommand);
    }

    public async Task RunCommands(IBucket bucket)
    {
        foreach (var command in _commands)
        {
            await command.Execute(bucket);
        }
    }
}

public interface IMigrateCommand
{
    Task Execute(IBucket bucket);
}