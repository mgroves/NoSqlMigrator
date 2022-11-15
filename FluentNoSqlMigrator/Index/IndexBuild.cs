using FluentNoSqlMigrator.Infrastructure;

namespace FluentNoSqlMigrator.Index;

public interface IIndexBuild
{
    /// <summary>
    /// The scope to create the index in (required)
    /// </summary>
    /// <param name="scopeName">Scope name</param>
    /// <returns></returns>
    IIndexBuildScope OnScope(string scopeName);
    IIndexBuildScope OnDefaultScope();
}

public interface IIndexBuildScope
{
    /// <summary>
    /// The collection to create the index in (required)
    /// </summary>
    /// <param name="collectionName">Collection name</param>
    /// <returns></returns>
    IIndexBuildCollection OnCollection(string collectionName);
    IIndexBuildCollection OnDefaultCollection();
}

public interface IIndexBuildCollection
{
    /// <summary>
    /// Field(s) to include in the index.
    /// At least one field is required in an index.
    /// </summary>
    /// <param name="fieldName">Field name</param>
    /// <returns></returns>
    IIndexFieldSettings OnField(string fieldName);
    
    /// <summary>
    /// Raw field(s) to include in the index. Use this when
    /// specifying a complex index, like an array index, adaptive index, etc 
    /// At least one field is required in an index.
    /// </summary>
    /// <param name="rawField">Raw field</param>
    /// <returns></returns>
    IIndexBuildCollection OnFieldRaw(string rawField);
}

public interface IIndexFieldSettings
{
    /// <summary>
    /// Field index is ascending (default if not specified)
    /// </summary>
    /// <returns></returns>
    IIndexBuildCollection Ascending();
    
    /// <summary>
    /// Field index is descending
    /// </summary>
    /// <returns></returns>
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

    public IIndexBuildScope OnDefaultScope()
    {
        _scopeName = "_default";
        return this;
    }

    public IIndexBuildCollection OnCollection(string collectionName)
    {
        _collectionName = collectionName;
        return this;
    }

    public IIndexBuildCollection OnDefaultCollection()
    {
        _collectionName = "_default";
        return this;
    }

    public IIndexFieldSettings OnField(string fieldName)
    {
        _fields.Add(fieldName, "");
        return this;
    }

    public IIndexBuildCollection OnFieldRaw(string rawField)
    {
        _fields.Add(rawField, "");
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