using Couchbase;
using NoSqlMigrator.Infrastructure;

namespace NoSqlMigrator.Scope;

internal class ScopeDeleteCommand : IMigrateCommand
{
    private readonly string _scopeName;

    internal ScopeDeleteCommand(string scopeName)
    {
        _scopeName = scopeName;
    }

    public async Task Execute(IBucket bucket)
    {
        var coll = bucket.Collections;
        await coll.DropScopeAsync(_scopeName);
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