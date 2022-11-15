using FluentNoSqlMigrator.Infrastructure;

namespace FluentNoSqlMigrator.Documents;

public interface IDocumentDelete
{
    /// <summary>
    /// The scope to delete document(s) from (required)
    /// </summary>
    /// <param name="scopeName">Scope name</param>
    /// <returns></returns>
    IDocumentDeleteScope Scope(string scopeName);
}

public interface IDocumentDeleteScope
{
    /// <summary>
    /// The collection to delete document(s) from (required)
    /// </summary>
    /// <param name="collectionName">Collection</param>
    /// <returns></returns>
    IDocumentDeleteCollection Collection(string collectionName);
}

public interface IDocumentDeleteCollection
{
    /// <summary>
    /// Document(s) to delete
    /// </summary>
    /// <param name="key">Document key (id)</param>
    /// <returns></returns>
    IDocumentDeleteCollection Document(string key);
}

internal class DocumentDelete : IDocumentDelete, IDocumentDeleteScope, IDocumentDeleteCollection, IBuildCommands
{
    private string _scopeName;
    private string _collectionName;
    private List<string> _keys;

    internal DocumentDelete()
    {
        _keys = new List<string>();
        MigrationContext.AddCommands(BuildCommands);
    }

    public IDocumentDeleteScope Scope(string scopeName)
    {
        _scopeName = scopeName;
        return this;
    }

    public IDocumentDeleteCollection Collection(string collectionName)
    {
        _collectionName = collectionName;
        return this;
    }

    public IDocumentDeleteCollection Document(string key)
    {
        _keys.Add(key);
        return this;
    }

    public List<IMigrateCommand> BuildCommands()
    {
        return new List<IMigrateCommand>
        {
            new DocumentDeleteCommand(_scopeName, _collectionName, _keys)
        };
    }
}