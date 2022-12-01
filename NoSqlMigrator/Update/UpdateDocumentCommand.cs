using Couchbase;
using Couchbase.KeyValue;
using NoSqlMigrator.Infrastructure;

namespace NoSqlMigrator.Update;

internal class UpdateDocumentCommand : IMigrateCommand
{
    private readonly string _scopeName;
    private readonly string _collectionName;
    private readonly string _documentId;
    private readonly Dictionary<string, object> _fieldsToUpsert;
    private readonly List<string> _fieldsToRemove;

    public UpdateDocumentCommand(string scopeName, string collectionName, string documentId, Dictionary<string,object> fieldsToUpsert, List<string> fieldsToRemove)
    {
        _scopeName = scopeName;
        _collectionName = collectionName;
        _documentId = documentId;
        _fieldsToUpsert = fieldsToUpsert;
        _fieldsToRemove = fieldsToRemove;
    }

    public async Task Execute(IBucket bucket)
    {
        var scope = await bucket.ScopeAsync(_scopeName);
        var coll = await scope.CollectionAsync(_collectionName);
        
        var specs = new List<MutateInSpec>();
        foreach (var path in _fieldsToRemove)
            specs.Add(MutateInSpec.Remove(path));
        foreach(var pair in _fieldsToUpsert)
            specs.Add(MutateInSpec.Upsert(pair.Key, pair.Value, true));
        
        await coll.MutateInAsync(_documentId, specs);
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
        if (string.IsNullOrEmpty(_documentId))
        {
            errorMessages.Add("Document ID (key) is required");
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