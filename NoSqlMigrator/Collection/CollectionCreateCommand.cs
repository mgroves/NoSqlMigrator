using Couchbase;
using Couchbase.Management.Collections;
using NoSqlMigrator.Infrastructure;
using Polly;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using NoSqlMigrator.Extensions;

namespace NoSqlMigrator.Collection;

internal class CollectionCreateCommand : IMigrateCommand
{
    private readonly string _scopeName;
    private readonly string _collectionName;

    internal CollectionCreateCommand(string scopeName, string collectionName)
    {
        _scopeName = scopeName;
        _collectionName = collectionName;
    }

    public async Task Execute(IBucket bucket)
    {
        var coll = bucket.Collections;
        await coll.CreateCollectionAsync(new CollectionSpec(_scopeName, _collectionName));

        var result = await bucket.DoesCollectionExist(_scopeName, _collectionName);

        if (!result)
            throw new Exception($"Creation of collection `{_collectionName}` in scope `{_scopeName}` could not be verified.");
    }

    /// <summary>
    /// verify collection exists/is ready
    /// this policy will retry 5 times, with an exponential backoff wait
    /// after each attempt
    /// until collection is found (or retry limit exceeded)    /// </summary>
    /// <returns></returns>
    // private async Task<bool> VerifyCollectionCreation(IBucket bucket)
    // {
    //     var policy = Policy
    //         .HandleResult<bool>(r => r == false)
    //         .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
    //             async (result, timeSpan, retryCount, context) =>
    //             {
    //                 Console.WriteLine("Retry attempt: " + retryCount + ", Retrying in " + timeSpan.TotalSeconds +
    //                                   " seconds.");
    //             });
    //     var result = await policy.ExecuteAsync(async () =>
    //     {
    //         var collManager = bucket.Collections;
    //         var allScopes = await collManager.GetAllScopesAsync();
    //         var doesCollectionExist = allScopes
    //             .Any(s => s.Collections
    //                 .Any(c => c.Name == _collectionName && c.ScopeName == _scopeName));
    //         return doesCollectionExist;
    //     });
    //     return result;
    // }

    public bool IsValid(List<string> errorMessages)
    {
        var isValid = true;
        if (string.IsNullOrEmpty(_collectionName))
        {
            errorMessages.Add("Collection name must be specified when creating a collection");
            isValid = false;
        }

        if (string.IsNullOrEmpty(_scopeName))
        {
            errorMessages.Add("Scope name must be specified when creating a collection");
            isValid = false;
        }

        return isValid;
    }
}