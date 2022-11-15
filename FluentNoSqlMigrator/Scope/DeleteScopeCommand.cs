using Couchbase;
using FluentNoSqlMigrator.Infrastructure;

namespace FluentNoSqlMigrator.Scope;

public class DeleteScopeCommand : IMigrateCommand
{
    public async Task Execute(IBucket bucket)
    {
        var coll = bucket.Collections;
        await coll.DropScopeAsync(Name);
    }

    public string Name { get; set; }
}