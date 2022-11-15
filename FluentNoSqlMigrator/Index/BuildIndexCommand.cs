using System.Text;
using Couchbase;
using FluentNoSqlMigrator.Infrastructure;

namespace FluentNoSqlMigrator.Index;

public class BuildIndexCommand : IMigrateCommand
{
    private readonly string _indexName;
    private readonly string _scopeName;
    private readonly string _collectionName;
    private readonly Dictionary<string, string> _fields;

    public BuildIndexCommand(string indexName, string scopeName, string collectionName, Dictionary<string,string> fields)
    {
        _indexName = indexName;
        _scopeName = scopeName;
        _collectionName = collectionName;
        _fields = fields;
    }


    public async Task Execute(IBucket bucket)
    {
        var sqlIndex = $"CREATE INDEX `{_indexName}` ON `{bucket.Name}`.`{_scopeName}`.`{_collectionName}` ({GetFields()})";
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

        return isValid;
    }

    private string GetFields()
    {
        var sb = new StringBuilder();
        foreach (var field in _fields)
        {
            sb.Append($"`{field.Key}`");
            if (!string.IsNullOrEmpty(field.Value))
                sb.Append($" {field.Value}");
            sb.Append(",");
        }

        return sb.ToString().Trim(',');
    }
}