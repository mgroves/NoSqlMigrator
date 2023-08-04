using NoSqlMigrator.Infrastructure;

namespace NoSqlMigrator.Documents;

public interface IInsertDocumentInitial
{
    IInsertDocumentsScopeSettings Into { get; }
}

public interface IInsertDocumentsScopeSettings
{
    /// <summary>
    /// The scope to insert document(s) into (required)
    /// </summary>
    /// <param name="scopeName">Scope name</param>
    /// <returns></returns>
    IInsertDocumentsCollectionSettings Scope(string scopeName);
}

public interface IInsertDocumentsCollectionSettings
{
    /// <summary>
    /// The collection to insert document(s) into (required)
    /// </summary>
    /// <param name="collectionName">Collection name</param>
    /// <returns></returns>
    IInsertDocumentsBuild Collection(string collectionName);
}

public interface IInsertDocumentsBuild
{
    /// <summary>
    /// Document to insert
    /// Not designed for bulk import of data
    /// </summary>
    /// <param name="key">Document key (id)</param>
    /// <param name="document">Any C# object (will be serialized to JSON)</param>
    /// <returns></returns>
    IInsertDocumentsBuild Document(string key, dynamic document);
    
    /// <summary>
    /// Document to insert
    /// Not designed for bulk import of data
    /// </summary>
    /// <param name="key">Document key (id)</param>
    /// <param name="document">Any C# object (will be serialized to JSON)</param>
    /// <returns></returns>    
    IInsertDocumentsBuild Document<T>(string key, T document);
}

public class KeySpaceAndDocument
{
    public string ScopeName { get; set; }
    public string CollectionName { get; set; }
    public string Key { get; set; }
    public object Document { get; set; }
}

public class DocumentCreate: IInsertDocumentInitial, IInsertDocumentsScopeSettings, IInsertDocumentsCollectionSettings, IInsertDocumentsBuild, IBuildCommands
{
    private string _collectionName;
    private List<KeySpaceAndDocument> _documents;
    private string _scopeName;

    /// <summary>
    /// Prepare to insert documents into a scope/collection
    /// </summary>
    public IInsertDocumentsScopeSettings Into
    {
        get
        {
            // since this command can build up multiple documents
            // make sure to only create this dictionary once
            // and only add one command to the context
            if (_documents == null)
            {
                _documents = new List<KeySpaceAndDocument>();
                MigrationContext.AddCommands(BuildCommands);
            }
            return this;
        }
    }

    public List<IMigrateCommand> BuildCommands()
    {
        return new List<IMigrateCommand>
        {
            new DocumentCreateCommand(_documents)
        };
    }

    public IInsertDocumentsBuild Document(string key, dynamic document)
    {
        _documents.Add(new KeySpaceAndDocument { ScopeName = _scopeName, CollectionName = _collectionName, Key = key, Document = document});
        return this;
    }

    public IInsertDocumentsBuild Document<T>(string key, T document)
    {
        _documents.Add(new KeySpaceAndDocument { ScopeName = _scopeName, CollectionName = _collectionName, Key = key, Document = document });
        return this;
    }

    public IInsertDocumentsCollectionSettings Scope(string scopeName)
    {
        _scopeName = scopeName;
        return this;
    }

    public IInsertDocumentsBuild Collection(string collectionName)
    {
        _collectionName = collectionName;
        return this;
    }
}