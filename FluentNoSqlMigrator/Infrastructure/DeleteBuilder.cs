using FluentNoSqlMigrator.Collection;
using FluentNoSqlMigrator.Index;
using FluentNoSqlMigrator.Scope;

namespace FluentNoSqlMigrator.Infrastructure;

public class DeleteBuilder
{
    /// <summary>
    /// Delete a scope from the bucket
    /// </summary>
    /// <param name="scopeName">Scope name</param>
    /// <returns></returns>
    public IScopeSettingsDelete Scope(string scopeName)
    {
        return new ScopeDelete(scopeName);
    }

    /// <summary>
    /// Delete a collection (from a scope)
    /// </summary>
    /// <param name="collectionName">Collection name</param>
    /// <returns></returns>
    public ICollectionSettingsDelete Collection(string collectionName)
    {
        return new CollectionDelete(collectionName);
    }

    /// <summary>
    /// Delete an index (from a scope and collection)
    /// </summary>
    /// <param name="indexName">Index name</param>
    /// <returns></returns>
    public IIndexDelete Index(string indexName)
    {
        return new IndexDelete(indexName);
    }
}
