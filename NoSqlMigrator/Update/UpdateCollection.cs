using NoSqlMigrator.Infrastructure;

namespace NoSqlMigrator.Update;

public interface IUpdateCollection
{
    /// <summary>
    /// Scope that collection is in (required)
    /// </summary>
    /// <param name="scopeName">Scope name</param>
    /// <returns></returns>
    IUpdateCollectionScope InScope(string scopeName);
}

public interface IUpdateCollectionScope
{
    /// <summary>
    /// Field to add/update (upsert) to every document and the value to update it with.
    /// This is destructive: if a value is updated (replaced), it cannot be restored
    /// </summary>
    /// <param name="fieldName">Field name</param>
    /// <param name="value">Field value (if null or empty string, the field will still be added)</param>
    /// <returns></returns>
    IUpdateCollectionFields UpsertFieldWithValue(string fieldName, object value);
    /// <summary>
    /// Field to remove from every document
    /// </summary>
    /// <param name="fieldName">Field name</param>
    /// <returns></returns>
    IUpdateCollectionFields RemoveField(string fieldName);
}

public interface IUpdateCollectionFields
{
    /// <summary>
    /// Field to add/update (upsert) to every document and the value to update it with 
    /// </summary>
    /// <param name="fieldName">Field name</param>
    /// <param name="value">Field value (if null or empty string, the field will still be added)</param>
    /// <returns></returns>    
    IUpdateCollectionFields UpsertFieldWithValue(string fieldName, object value);
    /// <summary>
    /// Field to remove from every document.
    /// This is destructive: once the data is removed, it cannot be restored.
    /// </summary>
    /// <param name="fieldName">Field name</param>
    /// <returns></returns>
    IUpdateCollectionFields RemoveField(string fieldName);
}

internal class UpdateCollection : IUpdateCollection, IUpdateCollectionScope, IUpdateCollectionFields, IBuildCommands
{
    private readonly string _collectionName;
    private readonly Dictionary<string,object> _fieldsToUpsert;
    private readonly List<string> _fieldsToRemove;
    private string _scopeName;

    public UpdateCollection(string collectionName)
    {
        _collectionName = collectionName;
        _fieldsToUpsert = new Dictionary<string, object>();
        _fieldsToRemove = new List<string>();
        MigrationContext.AddCommands(BuildCommands);
    }

    public List<IMigrateCommand> BuildCommands()
    {
        return new List<IMigrateCommand>
        {
            new UpdateCollectionCommand(_scopeName, _collectionName, _fieldsToUpsert, _fieldsToRemove)
        };
    }

    IUpdateCollectionFields IUpdateCollectionScope.UpsertFieldWithValue(string fieldName, object value)
    {
        _fieldsToUpsert.Add(fieldName, value);
        return this;
    }

    IUpdateCollectionFields IUpdateCollectionFields.RemoveField(string fieldName)
    {
        _fieldsToRemove.Add(fieldName);
        return this;
    }

    IUpdateCollectionFields IUpdateCollectionScope.RemoveField(string fieldName)
    {
        _fieldsToRemove.Add(fieldName);
        return this;
    }

    IUpdateCollectionFields IUpdateCollectionFields.UpsertFieldWithValue(string fieldName, object value)
    {
        _fieldsToUpsert.Add(fieldName, value);
        return this;
    }

    public IUpdateCollectionScope InScope(string scopeName)
    {
        _scopeName = scopeName;
        return this;
    }
}