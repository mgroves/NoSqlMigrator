using NoSqlMigrator.Infrastructure;

namespace NoSqlMigrator.Scope;

public interface IScopeSettingsDelete
{
    // no settings that I can think of (yet)
    // but this is the pattern to follow to move scope deletion
    // into another state
}

internal class ScopeDelete : IScopeSettingsDelete, IBuildCommands
{
    private readonly string _scopeName;

    internal ScopeDelete(string scopeName)
    {
        _scopeName = scopeName;
        MigrationContext.AddCommands(BuildCommands);
    }

    public List<IMigrateCommand> BuildCommands()
    {
        return new List<IMigrateCommand>
        {
            new ScopeDeleteCommand(_scopeName)
        };
    }
}