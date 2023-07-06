using System.Text;
using Couchbase;
using Couchbase.Management.Query;
using NoSqlMigrator.Infrastructure;
using Polly;

namespace NoSqlMigrator.Index;

internal class PrimaryIndexCreateCommand : IMigrateCommand
{
    private readonly string _indexName;
    private readonly string _scopeName;
    private readonly string _collectionName;
    private readonly bool _useGsi;
    private readonly List<string> _withNodes;
    private readonly bool _deferBuild;
    private readonly uint? _numReplicas;

    internal PrimaryIndexCreateCommand(string indexName, string scopeName, string collectionName, bool useGsi, List<string> withNodes, bool deferBuild, uint? numReplicas)
    {
        _indexName = indexName;
        _scopeName = scopeName;
        _collectionName = collectionName;
        _useGsi = useGsi;
        _withNodes = withNodes;
        _deferBuild = deferBuild;
        _numReplicas = numReplicas;
    }

    public async Task Execute(IBucket bucket)
    {
        // verify collection exists / is ready
        var verifyCollection = await VerifyCollectionCreation(bucket);
        if (!verifyCollection)
            throw new Exception($"Unable to verify `{_collectionName}` collection exists in `{_scopeName}` scope.");

        var sqlIndex = $"CREATE PRIMARY INDEX ";

        if(!string.IsNullOrEmpty(_indexName))
            sqlIndex += $"`{_indexName}`";
        
        sqlIndex += $" ON `{bucket.Name}`.`{_scopeName}`.`{_collectionName}` ";
        
        // USING GSI is optional
        if (_useGsi)
            sqlIndex += " USING GSI ";
        
        // WITH nodes is optional
        var hasWith = _withNodes.Any() || _deferBuild || _numReplicas.HasValue;
        if (hasWith)
        {
            sqlIndex += " WITH {";
            var withTokens = new List<string>();
            if (_withNodes.Any())
                withTokens.Add($" \"nodes\" : [\"{string.Join("\"",_withNodes)}\"] ");
            if (_deferBuild)
                withTokens.Add(" \"defer_build\":true ");
            if (_numReplicas.HasValue)
                withTokens.Add($" \"num_replica\" : {_numReplicas.Value}");
            sqlIndex += string.Join(", ", withTokens);
            sqlIndex += "}";
        }
        
        var cluster = bucket.Cluster;
        // execute query, retry if necessary
        var verifyQuery = await VerifyCreateIndex(cluster, sqlIndex);

        if (!verifyQuery)
            throw new Exception($"Unable to create primary index on `{_collectionName}` collection");
    }

    private async Task<bool> VerifyCreateIndex(ICluster cluster, string sqlIndex)
    {
        var policy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                async (result, timeSpan, retryCount, context) =>
                {
                    Console.WriteLine("Retry attempt: " + retryCount + ", Retrying in " + timeSpan.TotalSeconds +
                                      " seconds.");
                });
        var result = false;
        try
        {
            await policy.ExecuteAsync(async () =>
            {
                await cluster.QueryAsync<dynamic>(sqlIndex);
                result = true;
            });
        }
        catch (Exception e)
        {
            Console.WriteLine("The last retry failed, setting result to false");
            result = false;
        }
        return result;
    }

    /// <summary>
    /// verify collection exists/is ready
    /// this policy will retry 5 times, with an exponential backoff wait
    /// after each attempt
    /// until collection is found (or retry limit exceeded)    /// </summary>
    /// <returns></returns>
    private async Task<bool> VerifyCollectionCreation(IBucket bucket)
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
            var doesCollectionExist = allScopes
                .Any(s => s.Collections
                    .Any(c => c.Name == _collectionName && c.ScopeName == _scopeName));
            return doesCollectionExist;
        });
        return result;
    }

    public bool IsValid(List<string> errorMessages)
    {
        var isValid = true;

        if (string.IsNullOrEmpty(_scopeName))
        {
            errorMessages.Add("Scope name must be specified.");
            isValid = false;
        }

        if (string.IsNullOrEmpty(_collectionName))
        {
            errorMessages.Add("Collection name must be specified.");
            isValid = false;
        }

        return isValid;
    }
}