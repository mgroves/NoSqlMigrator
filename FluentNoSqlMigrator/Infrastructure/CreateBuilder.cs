using FluentNoSqlMigrator.Collection;
using FluentNoSqlMigrator.Index;
using FluentNoSqlMigrator.Scope;

namespace FluentNoSqlMigrator.Infrastructure;

public class CreateBuilder
{
    public IScopeSettingsBuild Scope(string name)
    {
        return new ScopeBuild(name);
    }

    public ICollectionSettingsBuild Collection(string collectionName)
    {
        return new CollectionBuild(collectionName);
    }

    public IIndexBuild Index(string indexName)
    {
        return new IndexBuild(indexName);
    }
}