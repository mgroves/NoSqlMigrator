using Couchbase;

namespace FluentNoSqlMigrator.Infrastructure;

public class MigrationContext
{
    private readonly Dictionary<Guid, IMigrateCommand> _commands;

    public MigrationContext()
    {
        _commands = new Dictionary<Guid, IMigrateCommand>();
    }
    
    public async Task RunCommands(IBucket bucket)
    {
        foreach (var command in _commands)
        {
            await command.Value.Execute(bucket);
        }
    }

    public void SetCommand(Guid guid, IMigrateCommand command)
    {
        if (_commands.ContainsKey(guid))
            _commands[guid] = command;
        else
            _commands.Add(guid, command);
    }
}

public interface IMigrateCommand
{
    Task Execute(IBucket bucket);
}