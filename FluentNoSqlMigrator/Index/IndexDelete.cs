using FluentNoSqlMigrator.Infrastructure;

namespace FluentNoSqlMigrator.Index;

public interface IIndexDelete
{
    IIndexDeleteScope InScope(string scopeName);
}

public interface IIndexDeleteScope
{
    IIndexDeleteCollection InCollection(string collectionName);
}

public interface IIndexDeleteCollection
{
    
}

public class IndexDelete : IIndexDelete, IIndexDeleteScope, IIndexDeleteCollection, IBuildCommands
{
    private readonly string _indexName;
    private string _scopeName;
    private string _collectionName;

    public IndexDelete(string indexName)
    {
        _indexName = indexName;
        MigrationContext.AddCommands(BuildCommands);
    }
    
    public IIndexDeleteScope InScope(string scopeName)
    {
        _scopeName = scopeName;
        return this;
    }

    public IIndexDeleteCollection InCollection(string collectionName)
    {
        _collectionName = collectionName;
        return this;
    }
    
    public List<IMigrateCommand> BuildCommands()
    {
        return new List<IMigrateCommand> { new DeleteIndexCommand(_indexName, _scopeName, _collectionName) };
    }
}