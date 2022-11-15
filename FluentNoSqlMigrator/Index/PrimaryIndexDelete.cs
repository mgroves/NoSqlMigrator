using FluentNoSqlMigrator.Infrastructure;

namespace FluentNoSqlMigrator.Index;

public interface IPrimaryIndexDelete
{
    /// <summary>
    /// Scope to delete primary index from
    /// </summary>
    /// <param name="scopeName">Scope name</param>
    /// <returns></returns>
    IPrimaryIndexDeleteScope FromScope(string scopeName);
}

public interface IPrimaryIndexDeleteScope
{
    /// <summary>
    /// Collection to delete primary index from
    /// </summary>
    /// <param name="collectionName">Collection name</param>
    void FromCollection(string collectionName);
}

public class PrimaryIndexDelete : IPrimaryIndexDelete, IPrimaryIndexDeleteScope, IBuildCommands
{
    private readonly string _indexName;
    private string _scopeName;
    private string _collectionName;

    public PrimaryIndexDelete(string indexName)
    {
        _indexName = indexName;
        MigrationContext.AddCommands(BuildCommands);
    }

    public List<IMigrateCommand> BuildCommands()
    {
        return new List<IMigrateCommand>
        {
            new PrimaryIndexDeleteCommand(_scopeName, _collectionName, _indexName)
        };
    }

    public IPrimaryIndexDeleteScope FromScope(string scopeName)
    {
        _scopeName = scopeName;
        return this;
    }

    public void FromCollection(string collectionName)
    {
        _collectionName = collectionName;
    }
}