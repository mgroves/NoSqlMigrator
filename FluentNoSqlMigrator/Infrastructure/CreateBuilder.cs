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
    public IScopeSettingsBuild Scope(string scopeName)
    {
        return new ScopeBuild(scopeName);
    }

    /// <summary>
    /// Create a new collection
    /// </summary>
    /// <param name="collectionName">Collection name</param>
    /// <returns></returns>
    public ICollectionSettingsBuild Collection(string collectionName)
    {
        return new CollectionBuild(collectionName);
    }

    /// <summary>
    /// Create a new SQL++ index
    /// </summary>
    /// <param name="indexName">Index name</param>
    /// <returns></returns>
    public IIndexBuild Index(string indexName)
    {
        return new IndexBuild(indexName);
    }

    /// <summary>
    /// Create a new SQL++ primary index
    /// </summary>
    /// <param name="indexName">Index name (optional)</param>
    /// <returns></returns>
    public IPrimaryIndexBuild PrimaryIndex(string indexName = "")
    {
        return new PrimaryIndexBuild(indexName);
    }
}