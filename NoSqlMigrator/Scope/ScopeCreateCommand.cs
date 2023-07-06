using Couchbase;
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

        var result = await VerifyScopeCreation(bucket);

        if (!result)
            throw new Exception($"Creation of collection scope `{_scopeName}` could not be verified.");
    }

    /// <summary>
    /// verify scope exists/is ready
    /// this policy will retry 5 times, with an exponential backoff wait
    /// after each attempt
    /// until collection is found (or retry limit exceeded)    /// </summary>
    /// <returns></returns>
    private async Task<bool> VerifyScopeCreation(IBucket bucket)
    {
        var policy = Policy
            .HandleResult<bool>(r => r == false)
            .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                async (result, timeSpan, retryCount, context) =>
                {
                    Console.WriteLine("Retry attempt: " + retryCount + ", Retrying in " + timeSpan.TotalSeconds +
                                      " seconds.");
                });
        var result = await policy.ExecuteAsync(async () =>
        {
            var collManager = bucket.Collections;
            var allScopes = await collManager.GetAllScopesAsync();
            var doesScopeExist = allScopes.Any(s => s.Name == _scopeName);
            return doesScopeExist;
        });
        return result;
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