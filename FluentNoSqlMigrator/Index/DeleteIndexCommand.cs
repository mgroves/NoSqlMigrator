﻿using Couchbase;
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