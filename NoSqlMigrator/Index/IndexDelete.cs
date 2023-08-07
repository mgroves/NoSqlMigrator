using NoSqlMigrator.Infrastructure;

namespace NoSqlMigrator.Index;

public interface IIndexDelete
{
    /// <summary>
    /// Scope to delete an index from (required)
    /// </summary>
    /// <param name="scopeName">Scope name</param>
    IIndexDeleteScope FromScope(string scopeName);
    /// <summary>
    /// Delete from _default scope
    /// </summary>
    IIndexDeleteScope FromDefaultScope();
}

public interface IIndexDeleteScope
{
    /// <summary>
    /// Collection to delete an index from (required)
    /// </summary>
    /// <param name="collectionName">Collection name</param>
    IIndexDeleteCollection FromCollection(string collectionName);
    /// <summary>
    /// Delete from _default collection
    /// </summary>
    IIndexDeleteCollection FromDefaultCollection();
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
    public IIndexDeleteScope FromDefaultScope()
    {
        _scopeName = "_default";
        return this;
    }

    public IIndexDeleteCollection FromCollection(string collectionName)
    {
        _collectionName = collectionName;
        return this;
    } 
    
    public IIndexDeleteCollection FromDefaultCollection()
    {
        _collectionName = "_default";
        return this;
    }
    
    public List<IMigrateCommand> BuildCommands()
    {
        return new List<IMigrateCommand> { new IndexDeleteCommand(_indexName, _scopeName, _collectionName) };
    }
}