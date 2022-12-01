using Couchbase;
using Couchbase.KeyValue;
using Couchbase.Management.Query;
using NoSqlMigrator.Infrastructure;

namespace NoSqlMigrator.Index;

internal class PrimaryIndexDeleteCommand : IMigrateCommand
{
    private readonly string _scopeName;
    private readonly string _collectionName;
    private readonly string _indexName;

    internal PrimaryIndexDeleteCommand(string scopeName, string collectionName, string indexName)
    {
        _scopeName = scopeName;
        _collectionName = collectionName;
        _indexName = indexName;
    }

    public async Task Execute(IBucket bucket)
    {
        var coll = bucket.Cluster.QueryIndexes;
        var opts = new DropPrimaryQueryIndexOptions();
        if(!string.IsNullOrEmpty(_indexName))
            opts.IndexName(_indexName);
        opts.ScopeName(_scopeName);
        opts.CollectionName(_collectionName);
        await coll.DropPrimaryIndexAsync(bucket.Name, opts);
    }

    public bool IsValid(List<string> errorMessages)
    {
        var isValid = true;

        if (string.IsNullOrEmpty(_scopeName))
        {
            errorMessages.Add("Scope name must be specified.");
            isValid = false;
        }

        if (string.IsNullOrEmpty(_collectionName))
        {
            errorMessages.Add("Collection name must be specified.");
            isValid = false;
        }

        return isValid;
    }
}