using FluentNoSqlMigrator.Infrastructure;

namespace FluentNoSqlMigrator.Collection;

public interface ICollectionSettingsDelete
{
    ICollectionSettingsDelete FromScope(string scopeName);
}

internal class CollectionDelete : ICollectionSettingsDelete, IBuildCommands
{
    private readonly string _collectionName;
    private string _scopeName;

    public CollectionDelete(string collectionName)
    {
        _collectionName = collectionName;
        MigrationContext.AddCommands(BuildCommands);
    }

    public ICollectionSettingsDelete FromScope(string scopeName)
    {
        _scopeName = scopeName;
        return this;
    }

    public List<IMigrateCommand> BuildCommands()
    {
        if (string.IsNullOrEmpty(_scopeName))
            throw new Exception("Scope name must be specified when deleting a collection");
        if (string.IsNullOrEmpty(_collectionName))
            throw new Exception("Collection name must be specified when deleting a collection");
        return new List<IMigrateCommand>
        {
            new DeleteCollectionCommand(_scopeName, _collectionName)
        };
    }
}