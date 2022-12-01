using Couchbase;
using NoSqlMigrator.Infrastructure;

namespace NoSqlMigrator.Documents;

internal class DocumentCreateCommand : IMigrateCommand
{
    private readonly string _scopeName;
    private readonly string _collectionName;
    private readonly Dictionary<string, object> _documents;

    public DocumentCreateCommand(string scopeName, string collectionName, Dictionary<string, object> documents)
    {
        _scopeName = scopeName;
        _collectionName = collectionName;
        _documents = documents;
    }

    public async Task Execute(IBucket bucket)
    {
        var scope = await bucket.ScopeAsync(_scopeName);
        var coll = await scope.CollectionAsync(_collectionName);
        foreach (var doc in _documents)
        {
            await coll.InsertAsync(doc.Key, doc.Value);
        }
    }

    public bool IsValid(List<string> errorMessages)
    {
        var isValid = true;
        if (string.IsNullOrEmpty(_collectionName))
        {
            errorMessages.Add("Collection name must be specified");
            isValid = false;
        }

        if (!_documents.Any())
        {
            errorMessages.Add("At least one document must be specified");
            isValid = false;
        }

        return isValid;
    }
}