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
        return new List<IMigrateCommand>
        {
            new DeleteCollectionCommand(_scopeName, _collectionName)
        };
    }
}