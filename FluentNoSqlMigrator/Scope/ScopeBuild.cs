using Couchbase;
using FluentNoSqlMigrator.Infrastructure;

namespace FluentNoSqlMigrator.Scope;

public interface IScopeSettingsBuild
{
    // no settings yet, this is just the stateful pattern
    // to follow for other builders (or should the need arise)
}

internal class ScopeBuild : IScopeSettingsBuild
{
    private readonly string _name;
    private readonly MigrationContext _context;

    public ScopeBuild(string name, MigrationContext context)
    {
        _name = name;
        _context = context;
        _context.AddCommand(new BuildScopeCommand() { Name = _name});
    }
}

internal class BuildScopeCommand : IMigrateCommand
{
    public async Task Execute(IBucket bucket)
    {
        var coll = bucket.Collections;
        await coll.CreateScopeAsync(Name);
        return;
    }

    public string Name { get; set; }
}