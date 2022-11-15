using System.Text;
using Couchbase;
using FluentNoSqlMigrator.Infrastructure;

namespace FluentNoSqlMigrator.Index;

internal class BuildIndexCommandField
{
    private readonly string _fieldName;
    private string _ascOrDesc;
    private readonly bool _isRaw;

    public BuildIndexCommandField(string fieldName, string ascOrDesc, bool isRaw)
    {
        _fieldName = fieldName;
        _ascOrDesc = ascOrDesc;
        _isRaw = isRaw;
    }

    public string Value => _isRaw ? _fieldName : $"`{_fieldName}` {_ascOrDesc}";

    public string AscOrDesc
    {
        set => _ascOrDesc = value;
    }
}

internal class IndexCreateCommand : IMigrateCommand
{
    private readonly string _indexName;
    private readonly string _scopeName;
    private readonly string _collectionName;
    private readonly List<BuildIndexCommandField> _fields;
    private readonly string _whereClause;
    private readonly bool _useGsi;
    private readonly List<string> _withNodes;
    private readonly bool _deferBuild;
    private readonly int? _numReplicas;

    public IndexCreateCommand(string indexName, string scopeName, string collectionName,
        List<BuildIndexCommandField> fields, string whereClause, bool useGsi, List<string> withNodes, bool deferBuild, int? numReplicas)
    {
        _indexName = indexName;
        _scopeName = scopeName;
        _collectionName = collectionName;
        _fields = fields;
        _whereClause = whereClause;
        _useGsi = useGsi;
        _withNodes = withNodes;
        _deferBuild = deferBuild;
        _numReplicas = numReplicas;
    }

    public async Task Execute(IBucket bucket)
    {
        var sqlIndex = $"CREATE INDEX `{_indexName}` ON `{bucket.Name}`.`{_scopeName}`.`{_collectionName}` ({GetFields()})";
        
        // WHERE clause is optional
        if (!string.IsNullOrEmpty(_whereClause))
            sqlIndex += $" WHERE {_whereClause} ";
        
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
        if (string.IsNullOrEmpty(_indexName))
        {
            errorMessages.Add("Index name must be specified.");
            isValid = false;
        }

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

        if (!_fields.Any())
        {
            errorMessages.Add("At least one field is required for an index.");
            isValid = false;
        }

        return isValid;
    }

    private string GetFields()
    {
        var sb = new StringBuilder();
        foreach (var field in _fields)
            sb.Append($"{field.Value},");
        return sb.ToString().Trim(',');
    }
}