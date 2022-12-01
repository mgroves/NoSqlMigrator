using Couchbase;
using Couchbase.Query;
using NoSqlMigrator.Infrastructure;

namespace NoSqlMigrator.Update;

internal class UpdateCollectionCommand : IMigrateCommand
{
    private readonly string _scopeName;
    private readonly string _collectionName;
    private readonly Dictionary<string, object> _fieldsToUpsert;
    private readonly List<string> _fieldsToRemove;

    public UpdateCollectionCommand(string scopeName, string collectionName, Dictionary<string, object> fieldsToUpsert, List<string> fieldsToRemove)
    {
        _scopeName = scopeName;
        _collectionName = collectionName;
        _fieldsToUpsert = fieldsToUpsert;
        _fieldsToRemove = fieldsToRemove;
    }

    public async Task Execute(IBucket bucket)
    {
        var cluster = bucket.Cluster;

        // TODO: I wonder if it would be faster/better to query for the IDs and use them to perform a bunch of subdocument updates?
        // (would still require a primary index, but might be less pressure on query/index services)
        
        var sql = $"UPDATE `{bucket.Name}`.`{_scopeName}`.`{_collectionName}` ";
        var opts = new QueryOptions();
        var paramNumber = 1;
        if (_fieldsToUpsert.Any())
        {
            sql += " SET ";
            foreach (var set in _fieldsToUpsert)
            {
                sql += $" `{set.Key}` = $param{paramNumber},";
                opts.Parameter("$param" + paramNumber, set.Value);
                paramNumber++;
            }

            sql = sql.TrimEnd(',');
        }

        if (_fieldsToRemove.Any())
            sql += " UNSET `" + string.Join("`,`", _fieldsToRemove) + "` ";

        await cluster.QueryAsync<dynamic>(sql, opts);
    }

    public bool IsValid(List<string> errorMessages)
    {
        var isValid = true;
        if (string.IsNullOrEmpty(_scopeName))
        {
            errorMessages.Add("Scope name is required");
            isValid = false;
        }
        if (string.IsNullOrEmpty(_collectionName))
        {
            errorMessages.Add("Collection name is required");
            isValid = false;
        }

        var isFieldsToUpsert = _fieldsToUpsert.Any();
        var isFieldsToRemove = _fieldsToRemove.Any();
        if (!isFieldsToUpsert && !isFieldsToRemove)
        {
            errorMessages.Add("Fields to upsert and/or remove are required");
            isValid = false;
        }
        
        return isValid;
    }
}