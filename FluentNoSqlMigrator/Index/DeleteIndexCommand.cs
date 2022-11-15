using Couchbase;
using FluentNoSqlMigrator.Infrastructure;

namespace FluentNoSqlMigrator.Index;

public class DeleteIndexCommand : IMigrateCommand
{
    private readonly string _indexName;
    private readonly string _scopeName;
    private readonly string _collectionName;

    public DeleteIndexCommand(string indexName, string scopeName, string collectionName)
    {
        _indexName = indexName;
        _scopeName = scopeName;
        _collectionName = collectionName;
    }

    public async Task Execute(IBucket bucket)
    {
        await bucket.Cluster.QueryAsync<dynamic>($"DROP INDEX `{_indexName}` ON `{bucket.Name}`.`{_scopeName}`.`{_collectionName}`");
    }
}