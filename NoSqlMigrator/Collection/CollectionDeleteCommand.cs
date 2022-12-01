using Couchbase;
using Couchbase.Management.Collections;
using NoSqlMigrator.Infrastructure;

namespace NoSqlMigrator.Collection;

internal class CollectionDeleteCommand : IMigrateCommand
{
    private readonly string _scopeName;
    private readonly string _collectionName;

    internal CollectionDeleteCommand(string scopeName, string collectionName)
    {
        _scopeName = scopeName;
        _collectionName = collectionName;
    }

    public async Task Execute(IBucket bucket)
    {
        var coll = bucket.Collections;
        await coll.DropCollectionAsync(new CollectionSpec(_scopeName, _collectionName));
    }

    public bool IsValid(List<string> errorMessages)
    {
        var isValid = true;
        if (string.IsNullOrEmpty(_scopeName))
        {
            errorMessages.Add("Scope name must be specified when deleting a collection");
            isValid = false;
        }

        if (string.IsNullOrEmpty(_collectionName))
        {
            errorMessages.Add("Collection name must be specified when deleting a collection");
            isValid = false;
        }
        return isValid;
    }
}