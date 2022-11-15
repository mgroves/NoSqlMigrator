using FluentNoSqlMigrator.Infrastructure;

namespace FluentNoSqlMigrator.Collection;

public interface ICollectionSettingsDelete
{
    /// <summary>
    /// The scope to delete the collection from (required)
    /// </summary>
    /// <param name="scopeName">Scope name</param>
    /// <returns></returns>
    ICollectionSettingsDelete FromScope(string scopeName);
    ICollectionSettingsDelete FromDefaultScope();
}

internal class CollectionDelete : ICollectionSettingsDelete, IBuildCommands
{
    private readonly string _collectionName;
    private string _scopeName;

    internal CollectionDelete(string collectionName)
    {
        _collectionName = collectionName;
        MigrationContext.AddCommands(BuildCommands);
    }

    public ICollectionSettingsDelete FromScope(string scopeName)
    {
        _scopeName = scopeName;
        return this;
    }

    public ICollectionSettingsDelete FromDefaultScope()
    {
        _scopeName = "_default";
        return this;
    }

    public List<IMigrateCommand> BuildCommands()
    {
        return new List<IMigrateCommand>
        {
            new CollectionDeleteCommand(_scopeName, _collectionName)
        };
    }
}