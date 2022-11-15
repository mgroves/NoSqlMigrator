using Couchbase;
using Couchbase.Management.Collections;
using FluentNoSqlMigrator.Infrastructure;

namespace FluentNoSqlMigrator.Collection;

internal class DeleteCollectionCommand : IMigrateCommand
{
    private readonly string _scopeName;
    private readonly string _collectionName;

    public DeleteCollectionCommand(string scopeName, string collectionName)
    {
        _scopeName = scopeName;
        _collectionName = collectionName;
    }

    public async Task Execute(IBucket bucket)
    {
        var coll = bucket.Collections;
        await coll.DropCollectionAsync(new CollectionSpec(_scopeName, _collectionName));
    }
}