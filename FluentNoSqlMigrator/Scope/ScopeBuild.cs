using FluentNoSqlMigrator.Collection;
using FluentNoSqlMigrator.Infrastructure;

namespace FluentNoSqlMigrator.Scope;

public interface IScopeSettingsBuild
{
    IScopeSettingsBuild WithCollection(string collectionName);
}

internal class ScopeBuild : IScopeSettingsBuild, IBuildCommands
{
    private readonly string _scopeName;
    private readonly List<string> _collections;

    public ScopeBuild(string scopeName)
    {
        _scopeName = scopeName;
        _collections = new List<string>();
        MigrationContext.AddCommands(BuildCommands);
    }

    public IScopeSettingsBuild WithCollection(string collectionName)
    {
        _collections.Add(collectionName);
        return this;
    }

    public List<IMigrateCommand> BuildCommands()
    {
        var commands = new List<IMigrateCommand>
        {
            new BuildScopeCommand(_scopeName),
        };
        _collections.ForEach(c => commands.Add(new BuildCollectionCommand( _scopeName,c)));
        return commands;
    }
}