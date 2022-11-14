using Couchbase;
using FluentNoSqlMigrator.Collection;
using FluentNoSqlMigrator.Infrastructure;

namespace FluentNoSqlMigrator.Scope;

public interface IScopeSettingsBuild
{
    IScopeSettingsBuild WithCollection(string collectionName);
}

internal class ScopeBuild : IScopeSettingsBuild
{
    private readonly string _scopeName;
    private readonly MigrationContext _context;
    private List<string> _collections;
    private readonly Guid _guid;

    public ScopeBuild(string scopeName, MigrationContext context)
    {
        _scopeName = scopeName;
        _context = context;
        _guid = Guid.NewGuid();
        _context.SetCommand(_guid, new BuildScopeCommand() { ScopeName = _scopeName});
        _collections = new List<string>();
    }

    public IScopeSettingsBuild WithCollection(string collectionName)
    {
        _collections.Add(collectionName);
        _context.SetCommand(Guid.NewGuid(), new BuildCollectionCommand( _scopeName,collectionName));
        return this;
    }
}

internal class BuildScopeCommand : IMigrateCommand
{
    public async Task Execute(IBucket bucket)
    {
        var coll = bucket.Collections;
        await coll.CreateScopeAsync(ScopeName);
        return;
    }

    public string ScopeName { get; set; }
}