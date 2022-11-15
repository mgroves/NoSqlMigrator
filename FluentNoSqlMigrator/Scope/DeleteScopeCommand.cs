using Couchbase;
using FluentNoSqlMigrator.Infrastructure;

namespace FluentNoSqlMigrator.Scope;

public class DeleteScopeCommand : IMigrateCommand
{
    private readonly string _scopeName;

    public DeleteScopeCommand(string scopeName)
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