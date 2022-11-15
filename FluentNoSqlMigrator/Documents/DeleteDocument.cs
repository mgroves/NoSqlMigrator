using Couchbase;
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

public class DeleteDocument : IDocumentDelete, IDocumentDeleteScope, IDocumentDeleteCollection, IBuildCommands
{
    private string _scopeName;
    private string _collectionName;
    private List<string> _keys;

    internal DeleteDocument()
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
            new DeleteDocumentsCommand(_scopeName, _collectionName, _keys)
        };
    }
}

public class DeleteDocumentsCommand : IMigrateCommand
{
    private readonly string _scopeName;
    private readonly string _collectionName;
    private readonly List<string> _keys;

    public DeleteDocumentsCommand(string scopeName, string collectionName, List<string> keys)
    {
        _scopeName = scopeName;
        _collectionName = collectionName;
        _keys = keys;
    }

    public async Task Execute(IBucket bucket)
    {
        var scope = await bucket.ScopeAsync(_scopeName);
        var coll = await scope.CollectionAsync(_collectionName);
        foreach (var key in _keys)
        {
            await coll.RemoveAsync(key);
        }
    }

    public bool IsValid(List<string> errorMessages)
    {
        var isValid = true;
        if (string.IsNullOrEmpty(_scopeName))
        {
            errorMessages.Add("Scope name must be specified");
            isValid = false;
        } 
        if (string.IsNullOrEmpty(_collectionName))
        {
            errorMessages.Add("Collection name must be specified");
            isValid = false;
        }

        if (!_keys.Any())
        {
            errorMessages.Add("At least one document key must be specified");
            isValid = false;
        }

        return isValid;
    }
}