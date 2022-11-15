using FluentNoSqlMigrator.Collection;
using FluentNoSqlMigrator.Infrastructure;

namespace FluentNoSqlMigrator.Scope;

public interface IScopeCreateSettings
{
    /// <summary>
    /// Create collection(s) within the new scope
    /// </summary>
    /// <param name="collectionName">Collection name</param>
    /// <returns></returns>
    IScopeCreateSettings WithCollection(string collectionName);
}

internal class ScopeCreate : IScopeCreateSettings, IBuildCommands
{
    private readonly string _scopeName;
    private readonly List<string> _collections;

    internal ScopeCreate(string scopeName)
    {
        _scopeName = scopeName;
        _collections = new List<string>();
        MigrationContext.AddCommands(BuildCommands);
    }

    public IScopeCreateSettings WithCollection(string collectionName)
    {
        _collections.Add(collectionName);
        return this;
    }

    public List<IMigrateCommand> BuildCommands()
    {
        var commands = new List<IMigrateCommand>
        {
            new ScopeCreateCommand(_scopeName),
        };
        _collections.ForEach(c => commands.Add(new CollectionCreateCommand( _scopeName,c)));
        return commands;
    }
}