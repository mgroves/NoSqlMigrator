using FluentNoSqlMigrator.Collection;
using FluentNoSqlMigrator.Documents;
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

    /// <summary>
    /// Delete a primary index (from a scope and collection)
    /// </summary>
    /// <param name="indexName">Index name (optional)</param>
    /// <returns></returns>
    public IPrimaryIndexDelete PrimaryIndex(string indexName = "")
    {
        return new PrimaryIndexDelete(indexName);
    }

    /// <summary>
    /// Prepare to delete document(s)
    /// </summary>
    public IDocumentDelete From => new DocumentDelete();
}
