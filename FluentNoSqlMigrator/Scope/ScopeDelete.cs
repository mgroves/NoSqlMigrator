using FluentNoSqlMigrator.Infrastructure;

namespace FluentNoSqlMigrator.Scope;

public interface IScopeSettingsDelete
{
    // no settings that I can think of (yet)
    // but this is the pattern to follow to move scope deletion
    // into another state
}

public class ScopeDelete : IScopeSettingsDelete, IBuildCommands
{
    private readonly string _scopeName;

    public ScopeDelete(string scopeName)
    {
        _scopeName = scopeName;
        MigrationContext.AddCommands(BuildCommands);
    }

    public List<IMigrateCommand> BuildCommands()
    {
        return new List<IMigrateCommand>
        {
            new DeleteScopeCommand(_scopeName)
        };
    }
}