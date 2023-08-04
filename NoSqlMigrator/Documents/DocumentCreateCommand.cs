using Couchbase;
using NoSqlMigrator.Infrastructure;

namespace NoSqlMigrator.Documents;

internal class DocumentCreateCommand : IMigrateCommand
{
    private readonly List<KeySpaceAndDocument> _documents;

    public DocumentCreateCommand(List<KeySpaceAndDocument> documents)
    {
        _documents = documents;
    }

    public async Task Execute(IBucket bucket)
    {
        foreach (var doc in _documents)
        {
            var scope = await bucket.ScopeAsync(doc.ScopeName);
            var coll = await scope.CollectionAsync(doc.CollectionName);
            await coll.InsertAsync(doc.Key, doc.Document);
        }
    }

    public bool IsValid(List<string> errorMessages)
    {
        var isValid = true;
        if (_documents.Any(d => string.IsNullOrEmpty(d.ScopeName)))
        {
            errorMessages.Add("Scope name(s) must be specified");
            isValid = false;
        }

        if (_documents.Any(d => string.IsNullOrEmpty(d.CollectionName)))
        {
            errorMessages.Add("Collection name(s) must be specified");
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