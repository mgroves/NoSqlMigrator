using FluentNoSqlMigrator.Infrastructure;

namespace FluentNoSqlMigrator.Insert;

public interface IInsertDocumentsScopeSettings
{
    IInsertDocumentsCollectionSettings Scope(string scopeName);
}

public interface IInsertDocumentsCollectionSettings
{
    IInsertDocumentsBuild Collection(string collectionName);
}

public interface IInsertDocumentsBuild
{
    IInsertDocumentsBuild Document(string key, dynamic document);
    IInsertDocumentsBuild Document<T>(string key, T document);
}

public class InsertBuilder: IInsertDocumentsScopeSettings, IInsertDocumentsCollectionSettings, IInsertDocumentsBuild, IBuildCommands
{
    private string _collectionName;
    private Dictionary<string, object> _documents;
    private string _scopeName;

    public IInsertDocumentsScopeSettings Into
    {
        get
        {
            _documents = new Dictionary<string, object>();
            MigrationContext.AddCommands(BuildCommands);
            return this;
        }
    }

    public List<IMigrateCommand> BuildCommands()
    {
        return new List<IMigrateCommand>
        {
            new BuildDocumentsCommand(_scopeName, _collectionName, _documents)
        };
    }

    public IInsertDocumentsBuild Document(string key, dynamic document)
    {
        _documents.Add(key, document);
        return this;
    }

    public IInsertDocumentsBuild Document<T>(string key, T document)
    {
        _documents.Add(key, document);
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