using Couchbase;
using NoSqlMigrator.Infrastructure;
using DropQueryIndexOptions = Couchbase.Management.Query.DropQueryIndexOptions;

namespace NoSqlMigrator.Index;

internal class IndexDeleteCommand : IMigrateCommand
{
    private readonly string _indexName;
    private readonly string _scopeName;
    private readonly string _collectionName;

    internal IndexDeleteCommand(string indexName, string scopeName, string collectionName)
    {
        _indexName = indexName;
        _scopeName = scopeName;
        _collectionName = collectionName;
    }

    public async Task Execute(IBucket bucket)
    {
        var ix = bucket.Cluster.QueryIndexes;
        var opts = new DropQueryIndexOptions();
        opts.ScopeName(_scopeName);
        opts.CollectionName(_collectionName);
        await ix.DropIndexAsync(bucket.Name, _indexName, opts);
    }

    public bool IsValid(List<string> errorMessages)
    {
        var isValid = true;
        if (string.IsNullOrEmpty(_indexName))
        {
            errorMessages.Add("Index name must be specified.");
            isValid = false;
        }

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