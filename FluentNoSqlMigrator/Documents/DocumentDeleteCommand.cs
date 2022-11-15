using Couchbase;
using FluentNoSqlMigrator.Infrastructure;

namespace FluentNoSqlMigrator.Documents;

public class DocumentDeleteCommand : IMigrateCommand
{
    private readonly string _scopeName;
    private readonly string _collectionName;
    private readonly List<string> _keys;

    public DocumentDeleteCommand(string scopeName, string collectionName, List<string> keys)
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