using Couchbase;

namespace NoSqlMigrator.Tests.Helpers;

public static class ClusterHelpers
{
    public static async Task<bool> DoesIndexExist(this IBucket @this, string indexName)
    {
        var cluster = @this.Cluster;
        var indexManager = cluster.QueryIndexes;
        var allIndexes = await indexManager.GetAllIndexesAsync(@this.Name);
        var exists = allIndexes.Any(i => i.Name == indexName);
        return exists;
    }
}