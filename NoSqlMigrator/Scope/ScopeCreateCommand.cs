using Couchbase;
using NoSqlMigrator.Extensions;
using NoSqlMigrator.Infrastructure;
using Polly;

namespace NoSqlMigrator.Scope;

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

        var result = await bucket.DoesScopeExist(_scopeName);

        if (!result)
            throw new Exception($"Creation of collection scope `{_scopeName}` could not be verified.");
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