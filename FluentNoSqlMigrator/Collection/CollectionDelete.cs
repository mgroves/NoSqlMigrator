using Couchbase;
using Couchbase.Management.Collections;
using FluentNoSqlMigrator.Infrastructure;

namespace FluentNoSqlMigrator.Collection;

public interface ICollectionSettingsDelete
{
    ICollectionSettingsDelete FromScope(string scopeName);
}

internal class CollectionDelete : ICollectionSettingsDelete
{
    private readonly string _collectionName;
    private readonly MigrationContext _context;
    private readonly Guid _guid;
    private string _scopeName;

    public CollectionDelete(string collectionName, MigrationContext context)
    {
        _collectionName = collectionName;
        _context = context;
        _guid = Guid.NewGuid();
        _context.SetCommand(_guid, new FluentSyntaxErrorCommand("You must specify the scope for this collection using FromScope()"));
    }

    public ICollectionSettingsDelete FromScope(string scopeName)
    {
        _scopeName = scopeName;
        _context.SetCommand(_guid, new DeleteCollectionCommand(_scopeName, _collectionName));
        return this;
    }
}

internal class DeleteCollectionCommand : IMigrateCommand
{
    private readonly string _scopeName;
    private readonly string _collectionName;

    public DeleteCollectionCommand(string scopeName, string collectionName)
    {
        _scopeName = scopeName;
        _collectionName = collectionName;
    }

    public async Task Execute(IBucket bucket)
    {
        var coll = bucket.Collections;
        await coll.DropCollectionAsync(new CollectionSpec(_scopeName, _collectionName));
    }
}