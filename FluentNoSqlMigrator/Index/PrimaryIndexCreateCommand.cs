using System.Text;
using Couchbase;
using Couchbase.Management.Query;
using FluentNoSqlMigrator.Infrastructure;

namespace FluentNoSqlMigrator.Index;

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
        await cluster.QueryAsync<dynamic>(sqlIndex);
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