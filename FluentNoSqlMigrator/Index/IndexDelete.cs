using FluentNoSqlMigrator.Infrastructure;

namespace FluentNoSqlMigrator.Index;

public interface IIndexDelete
{
    /// <summary>
    /// Scope to delete an index from (required)
    /// </summary>
    /// <param name="scopeName">Scope name</param>
    /// <returns></returns>
    IIndexDeleteScope FromScope(string scopeName);
}

public interface IIndexDeleteScope
{
    /// <summary>
    /// Collection to delete an index from (required)
    /// </summary>
    /// <param name="collectionName">Collection name</param>
    /// <returns></returns>
    IIndexDeleteCollection FromCollection(string collectionName);
}

public interface IIndexDeleteCollection
{
    
}

internal class IndexDelete : IIndexDelete, IIndexDeleteScope, IIndexDeleteCollection, IBuildCommands
{
    private readonly string _indexName;
    private string _scopeName;
    private string _collectionName;

    internal IndexDelete(string indexName)
    {
        _indexName = indexName;
        MigrationContext.AddCommands(BuildCommands);
    }
    
    public IIndexDeleteScope FromScope(string scopeName)
    {
        _scopeName = scopeName;
        return this;
    }

    public IIndexDeleteCollection FromCollection(string collectionName)
    {
        _collectionName = collectionName;
        return this;
    }
    
    public List<IMigrateCommand> BuildCommands()
    {
        return new List<IMigrateCommand> { new IndexDeleteCommand(_indexName, _scopeName, _collectionName) };
    }
}