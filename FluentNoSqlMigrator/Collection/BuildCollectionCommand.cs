using Couchbase;
using Couchbase.Management.Collections;
using FluentNoSqlMigrator.Infrastructure;

namespace FluentNoSqlMigrator.Collection;

internal class BuildCollectionCommand : IMigrateCommand
{
    private readonly string _scopeName;
    private readonly string _collectionName;

    public BuildCollectionCommand(string scopeName, string collectionName)
    {
        _scopeName = scopeName;
        _collectionName = collectionName;
    }

    public async Task Execute(IBucket bucket)
    {
        var coll = bucket.Collections;
        await coll.CreateCollectionAsync(new CollectionSpec(_scopeName, _collectionName));
    }

    public bool IsValid(List<string> errorMessages)
    {
        var isValid = true;
        if (string.IsNullOrEmpty(_collectionName))
        {
            errorMessages.Add("Collection name must be specified when creating a collection");
            isValid = false;
        }

        if (string.IsNullOrEmpty(_scopeName))
        {
            errorMessages.Add("Scope name must be specific when creating a collection");
            isValid = false;
        }

        return isValid;
    }
}