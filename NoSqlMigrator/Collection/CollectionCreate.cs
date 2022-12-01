using NoSqlMigrator.Infrastructure;

namespace NoSqlMigrator.Collection;

public interface ICollectionCreateSettings
{
    /// <summary>
    /// The scope to create the collection in (required).
    /// Alternatively, use Create.Scope("...").WithCollection("...")
    /// </summary>
    /// <param name="scopeName">Scope Name</param>
    /// <returns></returns>
    ICollectionCreateSettings InScope(string scopeName);
}

internal class CollectionCreate : ICollectionCreateSettings, IBuildCommands
{
    private readonly string _collectionName;
    private string _scopeName;

    internal CollectionCreate(string collectionName)
    {
        _collectionName = collectionName;
        MigrationContext.AddCommands(BuildCommands);
    }

    public ICollectionCreateSettings InScope(string scopeName)
    {
        _scopeName = scopeName;
        return this;
    }

    public List<IMigrateCommand> BuildCommands()
    {
        return new List<IMigrateCommand>
        {
            new CollectionCreateCommand(_scopeName, _collectionName)
        };
    }
}