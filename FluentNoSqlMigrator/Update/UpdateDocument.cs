using FluentNoSqlMigrator.Infrastructure;

namespace FluentNoSqlMigrator.Update;

public interface IUpdateDocument
{
    /// <summary>
    /// Collection that the document is in (required)
    /// </summary>
    /// <param name="collectionName">Collection name</param>
    /// <returns></returns>
    IUpdateDocumentCollection InCollection(string collectionName);
}

public interface IUpdateDocumentCollection
{
    /// <summary>
    /// Scope that the document is in (required)
    /// </summary>
    /// <param name="scopeName">Scope name</param>
    /// <returns></returns>
    IUpdateDocumentScope InScope(string scopeName);
}

public interface IUpdateDocumentScope
{
    /// <summary>
    /// Insert/update (upsert) a field with value into the document
    /// Note that this can be destructive, if there's an existing value, it will be lost forever
    /// </summary>
    /// <param name="fieldName">Field name</param>
    /// <param name="value">Value</param>
    /// <returns></returns>
    IUpdateDocumentFields UpsertFieldWithValue(string fieldName, object value);
    /// <summary>
    /// Remove a field from a document (field must exist)
    /// Note that this is destructive, the value will be lost forever
    /// </summary>
    /// <param name="fieldName">Field name</param>
    /// <returns></returns>    
    IUpdateDocumentFields RemoveField(string fieldName);
}

public interface IUpdateDocumentFields
{
    /// <summary>
    /// Insert/update (upsert) a field with value into the document
    /// Note that this can be destructive, if there's an existing value, it will be lost forever
    /// </summary>
    /// <param name="fieldName">Field name</param>
    /// <param name="value">Value</param>
    /// <returns></returns>
    IUpdateDocumentFields UpsertFieldWithValue(string fieldName, object value);
    /// <summary>
    /// Remove a field from a document (field must exist)
    /// Note that this is destructive, the value will be lost forever
    /// </summary>
    /// <param name="fieldName">Field name</param>
    /// <returns></returns>
    IUpdateDocumentFields RemoveField(string fieldName);
}

internal class UpdateDocument : IUpdateDocument, IUpdateDocumentCollection, IUpdateDocumentScope, IUpdateDocumentFields, IBuildCommands
{
    private readonly string _documentId;
    private string _collectionName;
    private string _scopeName;
    private readonly Dictionary<string,object> _fieldsToUpsert;
    private readonly List<string> _fieldsToRemove;

    public UpdateDocument(string documentId)
    {
        _documentId = documentId;
        _fieldsToUpsert = new Dictionary<string, object>();
        _fieldsToRemove = new List<string>();
        MigrationContext.AddCommands(BuildCommands);
    }

    public IUpdateDocumentCollection InCollection(string collectionName)
    {
        _collectionName = collectionName;
        return this;
    }

    public List<IMigrateCommand> BuildCommands()
    {
        return new List<IMigrateCommand>
        {
            new UpdateDocumentCommand(_scopeName, _collectionName, _documentId, _fieldsToUpsert, _fieldsToRemove)
        };
    }

    public IUpdateDocumentScope InScope(string scopeName)
    {
        _scopeName = scopeName;
        return this;
    }

    IUpdateDocumentFields IUpdateDocumentScope.UpsertFieldWithValue(string fieldName, object value)
    {
        _fieldsToUpsert.Add(fieldName, value);
        return this;
    }
    IUpdateDocumentFields IUpdateDocumentFields.UpsertFieldWithValue(string fieldName, object value)
    {
        _fieldsToUpsert.Add(fieldName, value);
        return this;
    }    

    IUpdateDocumentFields IUpdateDocumentFields.RemoveField(string fieldName)
    {
        _fieldsToRemove.Add(fieldName);
        return this;
    }

    IUpdateDocumentFields IUpdateDocumentScope.RemoveField(string fieldName)
    {
        _fieldsToRemove.Add(fieldName);
        return this;
    }


}