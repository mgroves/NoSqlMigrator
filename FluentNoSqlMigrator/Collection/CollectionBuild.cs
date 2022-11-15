using FluentNoSqlMigrator.Infrastructure;

namespace FluentNoSqlMigrator.Collection;

public interface ICollectionSettingsBuild
{
    /// <summary>
    /// The scope to create the collection in (required).
    /// Alternatively, use Create.Scope("...").WithCollection("...")
    /// </summary>
    /// <param name="scopeName">Scope Name</param>
    /// <returns></returns>
    ICollectionSettingsBuild InScope(string scopeName);
}

internal class CollectionBuild : ICollectionSettingsBuild, IBuildCommands
{
    private readonly string _collectionName;
    private string _scopeName;

    public CollectionBuild(string collectionName)
    {
        _collectionName = collectionName;
        MigrationContext.AddCommands(BuildCommands);
    }

    public ICollectionSettingsBuild InScope(string scopeName)
    {
        _scopeName = scopeName;
        return this;
    }

    public List<IMigrateCommand> BuildCommands()
    {
        return new List<IMigrateCommand>
        {
            new BuildCollectionCommand(_scopeName, _collectionName)
        };
    }
}