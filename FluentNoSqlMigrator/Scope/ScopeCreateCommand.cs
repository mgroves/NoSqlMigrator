using Couchbase;
using FluentNoSqlMigrator.Infrastructure;

namespace FluentNoSqlMigrator.Scope;

internal class ScopeCreateCommand : IMigrateCommand
{
    private readonly string _scopeName;

    internal ScopeCreateCommand(string scopeName)
    {
        _scopeName = scopeName;
    }

    public async Task Execute(IBucket bucket)
    {
        var coll = bucket.Collections;
        await coll.CreateScopeAsync(_scopeName);
        return;
    }

    public bool IsValid(List<string> errorMessages)
    {
        if (string.IsNullOrEmpty(_scopeName))
        {
            errorMessages.Add("Scope name must be specified");
            return false;
        }
        return true;
    }
}