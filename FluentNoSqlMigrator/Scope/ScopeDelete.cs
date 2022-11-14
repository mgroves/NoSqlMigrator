using Couchbase;
using FluentNoSqlMigrator.Infrastructure;

namespace FluentNoSqlMigrator.Scope;

public interface IScopeSettingsDelete
{
    // no settings that I can think of (yet)
    // but this is the pattern to follow to move scope deletion
    // into another state
}

public class ScopeDelete : IScopeSettingsDelete
{
    private readonly string _name;
    private readonly MigrationContext _context;
    private readonly Guid _guid;

    public ScopeDelete(string name, MigrationContext context)
    {
        _name = name;
        _context = context;
        _guid = Guid.NewGuid();
        _context.SetCommand(_guid, new DeleteScopeCommand() { Name = _name });
    }
}

public class DeleteScopeCommand : IMigrateCommand
{
    public async Task Execute(IBucket bucket)
    {
        var coll = bucket.Collections;
        await coll.DropScopeAsync(Name);
    }

    public string Name { get; set; }
}