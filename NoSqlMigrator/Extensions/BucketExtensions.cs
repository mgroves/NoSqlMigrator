using Couchbase;
using Polly;

namespace NoSqlMigrator.Extensions;

public static class BucketExtensions
{
    /// <summary>
    /// verify scope exists/is ready
    /// this policy will retry 5 times, with an exponential backoff wait
    /// after each attempt
    /// until collection is found (or retry limit exceeded)    /// </summary>
    /// <returns></returns>
    public static async Task<bool> DoesScopeExist(this IBucket @this, string scopeName)
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
            var collManager = @this.Collections;
            var allScopes = await collManager.GetAllScopesAsync();
            var doesScopeExist = allScopes.Any(s => s.Name == scopeName);
            return doesScopeExist;
        });
        return result;

    }

    /// <summary>
    /// verify collection exists/is ready
    /// this policy will retry 5 times, with an exponential backoff wait
    /// after each attempt
    /// until collection is found (or retry limit exceeded)    /// </summary>
    /// <returns></returns>
    public static async Task<bool> DoesCollectionExist(this IBucket @this, string scopeName, string collectionName)
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
            var collManager = @this.Collections;
            var allScopes = await collManager.GetAllScopesAsync();
            var doesCollectionExist = allScopes
                .Any(s => s.Collections
                    .Any(c => c.Name == collectionName && c.ScopeName == scopeName));
            return doesCollectionExist;
        });
        return result;
    }
}