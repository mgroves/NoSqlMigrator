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
}