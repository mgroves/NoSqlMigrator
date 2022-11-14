using Couchbase;
using Couchbase.Management.Collections;
using FluentNoSqlMigrator.Infrastructure;

namespace FluentNoSqlMigrator.Collection;

public interface ICollectionSettingsBuild
{
    ICollectionSettingsBuild InScope(string scopeName);
}

internal class CollectionBuild : ICollectionSettingsBuild
{
    private readonly string _collectionName;
    private readonly MigrationContext _context;
    private string _scopeName;
    private readonly Guid _guid;

    public CollectionBuild(string collectionName, MigrationContext context)
    {
        _guid = Guid.NewGuid();
        _collectionName = collectionName;
        _context = context;
        _context.SetCommand(_guid, new FluentSyntaxErrorCommand("You must specify the scope for this collection using InScope()"));
    }

    public ICollectionSettingsBuild InScope(string scopeName)
    {
        _scopeName = scopeName;
        _context.SetCommand(_guid, new BuildCollectionCommand(scopeName, _collectionName));
        return this;
    }
}

internal class BuildCollectionCommand : IMigrateCommand
{
    private readonly string _scopeName;
    private readonly string _collectionName;

    public BuildCollectionCommand(string scopeName, string collectionName)
    {
        _scopeName = scopeName;
        _collectionName = collectionName;
    }

    public async Task Execute(IBucket bucket)
    {
        var coll = bucket.Collections;
        await coll.CreateCollectionAsync(new CollectionSpec(_scopeName, _collectionName));
    }
}