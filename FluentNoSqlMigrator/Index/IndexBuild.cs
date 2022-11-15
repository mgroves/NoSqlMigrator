using FluentNoSqlMigrator.Infrastructure;

namespace FluentNoSqlMigrator.Index;

public interface IIndexBuild
{
    IIndexBuildScope OnScope(string collectionName);
}

public interface IIndexBuildScope
{
    IIndexBuildCollection OnCollection(string collectionName);
}

public interface IIndexBuildCollection
{
    IIndexFieldSettings OnField(string fieldName);
}

public interface IIndexFieldSettings
{
    IIndexBuildCollection Ascending();
    IIndexBuildCollection Descending();
}

// TODO: add "WHERE" clause
// TODO: USING GSI
// TODO: with nodes
// TODO: deferred
public class IndexBuild : IIndexBuild, IIndexBuildScope, IIndexBuildCollection, IIndexFieldSettings, IBuildCommands
{
    private readonly string _indexName;
    private string _collectionName;
    private Dictionary<string, string> _fields;
    private string _scopeName;

    public IndexBuild(string indexName)
    {
        _indexName = indexName;
        _fields = new Dictionary<string, string>();
        MigrationContext.AddCommands(BuildCommands);
    }

    public IIndexBuildScope OnScope(string scopeName)
    {
        _scopeName = scopeName;
        return this;
    }
    
    public IIndexBuildCollection OnCollection(string collectionName)
    {
        _collectionName = collectionName;
        return this;
    }

    public IIndexFieldSettings OnField(string fieldName)
    {
        _fields.Add(fieldName, "");
        return this;
    }

    public IIndexBuildCollection Ascending()
    {
        _fields[_fields.Last().Key] = "ASC";
        return this;
    }

    public IIndexBuildCollection Descending()
    {
        _fields[_fields.Last().Key] = "DESC";
        return this;
    }

    public List<IMigrateCommand> BuildCommands()
    {
        return new List<IMigrateCommand>
        {
            new BuildIndexCommand(_indexName, _scopeName, _collectionName, _fields)
        };
    }
}