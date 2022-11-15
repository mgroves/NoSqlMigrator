using FluentNoSqlMigrator.Collection;
using FluentNoSqlMigrator.Index;
using FluentNoSqlMigrator.Scope;

namespace FluentNoSqlMigrator.Infrastructure;

public class DeleteBuilder
{
    public IScopeSettingsDelete Scope(string scopeName)
    {
        return new ScopeDelete(scopeName);
    }

    public ICollectionSettingsDelete Collection(string collectionName)
    {
        return new CollectionDelete(collectionName);
    }

    public IIndexDelete Index(string indexName)
    {
        return new IndexDelete(indexName);
    }
}
