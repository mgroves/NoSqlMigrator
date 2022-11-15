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
    
    /// <summary>
    /// Create index in default scope (_default)
    /// </summary>
    /// <returns></returns>
    IIndexBuildScope OnDefaultScope();
    
    /// <summary>
    /// Add WHERE clause to index
    /// </summary>
    /// <param name="whereClause">WHERE clause (don't include "WHERE")</param>
    /// <returns></returns>
    IIndexBuild Where(string whereClause);
    /// <summary>
    /// Explicitly add USING GSI to index
    /// </summary>
    /// <returns></returns>
    IIndexBuild UsingGsi();

    IIndexBuild WithNodes(params string[] nodes);
    IIndexBuild WithDeferBuild();
    IIndexBuild WithNumReplicas(int numReplicas);
}

public interface IIndexBuildScope
{
    /// <summary>
    /// The collection to create the index in (required)
    /// </summary>
    /// <param name="collectionName">Collection name</param>
    /// <returns></returns>
    IIndexBuildCollection OnCollection(string collectionName);
    /// <summary>
    /// Create index in default collection (_default)
    /// </summary>
    /// <returns></returns>
    IIndexBuildCollection OnDefaultCollection();
    /// <summary>
    /// Add WHERE clause to index
    /// </summary>
    /// <param name="whereClause">WHERE clause (don't include "WHERE")</param>
    /// <returns></returns>
    IIndexBuildScope Where(string whereClause);
    /// <summary>
    /// Explicitly add USING GSI to index
    /// </summary>
    /// <returns></returns>
    IIndexBuildScope UsingGsi();
    IIndexBuildScope WithNodes(params string[] nodes);
    IIndexBuildScope WithDeferBuild();
    IIndexBuildScope WithNumReplicas(int numReplicas);    
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

    /// <summary>
    /// Add WHERE clause to index
    /// </summary>
    /// <param name="whereClause">WHERE clause (don't include "WHERE")</param>
    /// <returns></returns>
    IIndexBuildCollection Where(string whereClause);
    /// <summary>
    /// Explicitly add USING GSI to index
    /// </summary>
    /// <returns></returns>
    IIndexBuildCollection UsingGsi();
    
    IIndexBuildCollection WithNodes(params string[] nodes);
    IIndexBuildCollection WithDeferBuild();
    IIndexBuildCollection WithNumReplicas(int numReplicas);     
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
    
    /// <summary>
    /// Add WHERE clause to index
    /// </summary>
    /// <param name="whereClause">WHERE clause (don't include "WHERE")</param>
    /// <returns></returns>
    IIndexFieldSettings Where(string whereClause);
    /// <summary>
    /// Explicitly add USING GSI to index
    /// </summary>
    /// <returns></returns>
    IIndexFieldSettings UsingGsi();
    
    IIndexFieldSettings WithNodes(params string[] nodes);
    IIndexFieldSettings WithDeferBuild();
    IIndexFieldSettings WithNumReplicas(int numReplicas);  
}

// TODO: add "WHERE" clause
// TODO: USING GSI
// TODO: with nodes
// TODO: deferred
public class IndexBuild : IIndexBuild, IIndexBuildScope, IIndexBuildCollection, IIndexFieldSettings, IBuildCommands
{
    private readonly string _indexName;
    private string _collectionName;
    private List<BuildIndexCommandField> _fields;
    private string _scopeName;
    private string _whereClause;
    private bool _useGsi;
    private List<string> _withNodes;
    private bool _deferBuild;
    private int? _numReplicas;

    public IndexBuild(string indexName)
    {
        _indexName = indexName;
        _fields = new List<BuildIndexCommandField>();
        _withNodes = new List<string>();
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
        _fields.Add(new BuildIndexCommandField(fieldName, "", false));
        return this;
    }

    public IIndexBuildCollection OnFieldRaw(string rawField)
    {
        _fields.Add(new BuildIndexCommandField(rawField, "", true));
        return this;
    }

    public IIndexBuildCollection Ascending()
    {
        _fields.Last().AscOrDesc = "ASC";
        return this;
    }

    public IIndexBuildCollection Descending()
    {
        _fields.Last().AscOrDesc = "DESC";
        return this;
    }
    
    // I'm not sure the best place to put "WHERE" clause
    // It could be only at top leve
    // But SQL++ syntax has it at the end
    // Both make sense to me for fluent syntax, so I put them everywhere
    // same is true for USING GSI and WITH
    #region WHERE
    
    IIndexBuild IIndexBuild.Where(string whereClause)
    {
        _whereClause = whereClause;
        return this;
    }

    IIndexBuildScope IIndexBuildScope.Where(string whereClause)
    {
        _whereClause = whereClause;
        return this;
    }
    
    IIndexBuildCollection IIndexBuildCollection.Where(string whereClause)
    {
        _whereClause = whereClause;
        return this;
    }
    
    IIndexFieldSettings IIndexFieldSettings.Where(string whereClause)
    {
        _whereClause = whereClause;
        return this;
    }
    #endregion

    // I think this is a completely optional keyword
    // Since GSI is the only index option (for now)
    // Adding it for completeness
    #region USING GSI
    IIndexFieldSettings IIndexFieldSettings.UsingGsi()
    {
        _useGsi = true;
        return this;
    }

    IIndexBuildCollection IIndexBuildCollection.UsingGsi()
    {
        _useGsi = true;
        return this;
    }
    IIndexBuildScope IIndexBuildScope.UsingGsi()
    {
        _useGsi = true;
        return this;
    }
    IIndexBuild IIndexBuild.UsingGsi()
    {
        _useGsi = true;
        return this;
    }
    
    #endregion

    #region WITH
    
    IIndexFieldSettings IIndexFieldSettings.WithNodes(params string[] nodes)
    {
        _withNodes.AddRange(nodes);
        return this;
    }
    IIndexBuildCollection IIndexBuildCollection.WithNodes(params string[] nodes)
    {
        _withNodes.AddRange(nodes);
        return this;
    }    
    IIndexBuildScope IIndexBuildScope.WithNodes(params string[] nodes)
    {
        _withNodes.AddRange(nodes);
        return this;
    }
    IIndexBuild IIndexBuild.WithNodes(params string[] nodes)
    {
        _withNodes.AddRange(nodes);
        return this;
    }    
    
    IIndexFieldSettings IIndexFieldSettings.WithDeferBuild()
    {
        _deferBuild = true;
        return this;
    }
    IIndexBuildCollection IIndexBuildCollection.WithDeferBuild()
    {
        _deferBuild = true;
        return this;
    }    
    IIndexBuildScope IIndexBuildScope.WithDeferBuild()
    {
        _deferBuild = true;
        return this;
    }
    IIndexBuild IIndexBuild.WithDeferBuild()
    {
        _deferBuild = true;
        return this;
    }

    IIndexFieldSettings IIndexFieldSettings.WithNumReplicas(int numReplicas)
    {
        _numReplicas = numReplicas;
        return this;
    }
    IIndexBuildCollection IIndexBuildCollection.WithNumReplicas(int numReplicas)
    {
        _numReplicas = numReplicas;
        return this;
    }
    IIndexBuildScope IIndexBuildScope.WithNumReplicas(int numReplicas)
    {
        _numReplicas = numReplicas;
        return this;
    }
    IIndexBuild IIndexBuild.WithNumReplicas(int numReplicas)
    {
        _numReplicas = numReplicas;
        return this;
    }    

    #endregion
    
    public List<IMigrateCommand> BuildCommands()
    {
        return new List<IMigrateCommand>
        {
            new BuildIndexCommand(_indexName, _scopeName, _collectionName, _fields, _whereClause, _useGsi, _withNodes, _deferBuild, _numReplicas)
        };
    }
}