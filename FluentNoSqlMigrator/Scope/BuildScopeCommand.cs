using Couchbase;
using FluentNoSqlMigrator.Infrastructure;

namespace FluentNoSqlMigrator.Scope;

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