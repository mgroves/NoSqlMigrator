using FluentNoSqlMigrator.Collection;
using FluentNoSqlMigrator.Index;
using FluentNoSqlMigrator.Scope;

namespace FluentNoSqlMigrator.Infrastructure;

public class CreateBuilder
{
    /// <summary>
    /// Create a new scope in the bucket
    /// </summary>
    /// <param name="scopeName">Scope name</param>
    /// <returns></returns>
    public IScopeCreateSettings Scope(string scopeName)
    {
        return new ScopeCreate(scopeName);
    }

    /// <summary>
    /// Create a new collection
    /// </summary>
    /// <param name="collectionName">Collection name</param>
    /// <returns></returns>
    public ICollectionCreateSettings Collection(string collectionName)
    {
        return new CollectionCreate(collectionName);
    }

    /// <summary>
    /// Create a new SQL++ index
    /// </summary>
    /// <param name="indexName">Index name</param>
    /// <returns></returns>
    public IIndexCreate Index(string indexName)
    {
        return new IndexCreate(indexName);
    }

    /// <summary>
    /// Create a new SQL++ primary index
    /// Note: Primary index is not recommended for production.
    /// </summary>
    /// <param name="indexName">Index name (optional)</param>
    /// <returns></returns>
    public IPrimaryIndexCreate PrimaryIndex(string indexName = "")
    {
        return new PrimaryIndexCreate(indexName);
    }
}