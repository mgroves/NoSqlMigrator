using FluentNoSqlMigrator.Infrastructure;

namespace FluentNoSqlMigrator.Index;

public interface IIndexCreate
{
    /// <summary>
    /// The scope to create the index in (required)
    /// </summary>
    /// <param name="scopeName">Scope name</param>
    /// <returns></returns>
    IIndexCreateScope OnScope(string scopeName);
    
    /// <summary>
    /// Create index in default scope (_default)
    /// </summary>
    /// <returns></returns>
    IIndexCreateScope OnDefaultScope();
    
    /// <summary>
    /// Add WHERE clause to index
    /// </summary>
    /// <param name="whereClause">WHERE clause (don't include "WHERE")</param>
    /// <returns></returns>
    IIndexCreate Where(string whereClause);
    /// <summary>
    /// Explicitly add USING GSI to index
    /// </summary>
    /// <returns></returns>
    IIndexCreate UsingGsi();

    IIndexCreate WithNodes(params string[] nodes);
    IIndexCreate WithDeferBuild();
    IIndexCreate WithNumReplicas(int numReplicas);
}

public interface IIndexCreateScope
{
    /// <summary>
    /// The collection to create the index in (required)
    /// </summary>
    /// <param name="collectionName">Collection name</param>
    /// <returns></returns>
    IIndexCreateCollection OnCollection(string collectionName);
    /// <summary>
    /// Create index in default collection (_default)
    /// </summary>
    /// <returns></returns>
    IIndexCreateCollection OnDefaultCollection();
    /// <summary>
    /// Add WHERE clause to index
    /// </summary>
    /// <param name="whereClause">WHERE clause (don't include "WHERE")</param>
    /// <returns></returns>
    IIndexCreateScope Where(string whereClause);
    /// <summary>
    /// Explicitly add USING GSI to index
    /// </summary>
    /// <returns></returns>
    IIndexCreateScope UsingGsi();
    IIndexCreateScope WithNodes(params string[] nodes);
    IIndexCreateScope WithDeferBuild();
    IIndexCreateScope WithNumReplicas(int numReplicas);    
}

public interface IIndexCreateCollection
{
    /// <summary>
    /// Field(s) to include in the index.
    /// At least one field is required in an index.
    /// </summary>
    /// <param name="fieldName">Field name</param>
    /// <returns></returns>
    IIndexCreateFieldSettings OnField(string fieldName);
    
    /// <summary>
    /// Raw field(s) to include in the index. Use this when
    /// specifying a complex index, like an array index, adaptive index, etc 
    /// At least one field is required in an index.
    /// </summary>
    /// <param name="rawField">Raw field</param>
    /// <returns></returns>
    IIndexCreateCollection OnFieldRaw(string rawField);

    /// <summary>
    /// Add WHERE clause to index
    /// </summary>
    /// <param name="whereClause">WHERE clause (don't include "WHERE")</param>
    /// <returns></returns>
    IIndexCreateCollection Where(string whereClause);
    /// <summary>
    /// Explicitly add USING GSI to index
    /// </summary>
    /// <returns></returns>
    IIndexCreateCollection UsingGsi();
    
    IIndexCreateCollection WithNodes(params string[] nodes);
    IIndexCreateCollection WithDeferBuild();
    IIndexCreateCollection WithNumReplicas(int numReplicas);     
}

public interface IIndexCreateFieldSettings
{
    /// <summary>
    /// Field index is ascending (default if not specified)
    /// </summary>
    /// <returns></returns>
    IIndexCreateCollection Ascending();
    
    /// <summary>
    /// Field index is descending
    /// </summary>
    /// <returns></returns>
    IIndexCreateCollection Descending();
    
    /// <summary>
    /// Add WHERE clause to index
    /// </summary>
    /// <param name="whereClause">WHERE clause (don't include "WHERE")</param>
    /// <returns></returns>
    IIndexCreateFieldSettings Where(string whereClause);
    /// <summary>
    /// Explicitly add USING GSI to index
    /// </summary>
    /// <returns></returns>
    IIndexCreateFieldSettings UsingGsi();
    
    IIndexCreateFieldSettings WithNodes(params string[] nodes);
    IIndexCreateFieldSettings WithDeferBuild();
    IIndexCreateFieldSettings WithNumReplicas(int numReplicas);  
}

// TODO: add "WHERE" clause
// TODO: USING GSI
// TODO: with nodes
// TODO: deferred
internal class IndexCreate : IIndexCreate, IIndexCreateScope, IIndexCreateCollection, IIndexCreateFieldSettings, IBuildCommands
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

    internal IndexCreate(string indexName)
    {
        _indexName = indexName;
        _fields = new List<BuildIndexCommandField>();
        _withNodes = new List<string>();
        MigrationContext.AddCommands(BuildCommands);
    }

    public IIndexCreateScope OnScope(string scopeName)
    {
        _scopeName = scopeName;
        return this;
    }

    public IIndexCreateScope OnDefaultScope()
    {
        _scopeName = "_default";
        return this;
    }

    public IIndexCreateCollection OnCollection(string collectionName)
    {
        _collectionName = collectionName;
        return this;
    }

    public IIndexCreateCollection OnDefaultCollection()
    {
        _collectionName = "_default";
        return this;
    }

    public IIndexCreateFieldSettings OnField(string fieldName)
    {
        _fields.Add(new BuildIndexCommandField(fieldName, "", false));
        return this;
    }

    public IIndexCreateCollection OnFieldRaw(string rawField)
    {
        _fields.Add(new BuildIndexCommandField(rawField, "", true));
        return this;
    }

    public IIndexCreateCollection Ascending()
    {
        _fields.Last().AscOrDesc = "ASC";
        return this;
    }

    public IIndexCreateCollection Descending()
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
    
    IIndexCreate IIndexCreate.Where(string whereClause)
    {
        _whereClause = whereClause;
        return this;
    }

    IIndexCreateScope IIndexCreateScope.Where(string whereClause)
    {
        _whereClause = whereClause;
        return this;
    }
    
    IIndexCreateCollection IIndexCreateCollection.Where(string whereClause)
    {
        _whereClause = whereClause;
        return this;
    }
    
    IIndexCreateFieldSettings IIndexCreateFieldSettings.Where(string whereClause)
    {
        _whereClause = whereClause;
        return this;
    }
    #endregion

    // I think this is a completely optional keyword
    // Since GSI is the only index option (for now)
    // Adding it for completeness
    #region USING GSI
    IIndexCreateFieldSettings IIndexCreateFieldSettings.UsingGsi()
    {
        _useGsi = true;
        return this;
    }

    IIndexCreateCollection IIndexCreateCollection.UsingGsi()
    {
        _useGsi = true;
        return this;
    }
    IIndexCreateScope IIndexCreateScope.UsingGsi()
    {
        _useGsi = true;
        return this;
    }
    IIndexCreate IIndexCreate.UsingGsi()
    {
        _useGsi = true;
        return this;
    }
    
    #endregion

    #region WITH
    
    IIndexCreateFieldSettings IIndexCreateFieldSettings.WithNodes(params string[] nodes)
    {
        _withNodes.AddRange(nodes);
        return this;
    }
    IIndexCreateCollection IIndexCreateCollection.WithNodes(params string[] nodes)
    {
        _withNodes.AddRange(nodes);
        return this;
    }    
    IIndexCreateScope IIndexCreateScope.WithNodes(params string[] nodes)
    {
        _withNodes.AddRange(nodes);
        return this;
    }
    IIndexCreate IIndexCreate.WithNodes(params string[] nodes)
    {
        _withNodes.AddRange(nodes);
        return this;
    }    
    
    IIndexCreateFieldSettings IIndexCreateFieldSettings.WithDeferBuild()
    {
        _deferBuild = true;
        return this;
    }
    IIndexCreateCollection IIndexCreateCollection.WithDeferBuild()
    {
        _deferBuild = true;
        return this;
    }    
    IIndexCreateScope IIndexCreateScope.WithDeferBuild()
    {
        _deferBuild = true;
        return this;
    }
    IIndexCreate IIndexCreate.WithDeferBuild()
    {
        _deferBuild = true;
        return this;
    }

    IIndexCreateFieldSettings IIndexCreateFieldSettings.WithNumReplicas(int numReplicas)
    {
        _numReplicas = numReplicas;
        return this;
    }
    IIndexCreateCollection IIndexCreateCollection.WithNumReplicas(int numReplicas)
    {
        _numReplicas = numReplicas;
        return this;
    }
    IIndexCreateScope IIndexCreateScope.WithNumReplicas(int numReplicas)
    {
        _numReplicas = numReplicas;
        return this;
    }
    IIndexCreate IIndexCreate.WithNumReplicas(int numReplicas)
    {
        _numReplicas = numReplicas;
        return this;
    }    

    #endregion
    
    public List<IMigrateCommand> BuildCommands()
    {
        return new List<IMigrateCommand>
        {
            new IndexCreateCommand(_indexName, _scopeName, _collectionName, _fields, _whereClause, _useGsi, _withNodes, _deferBuild, _numReplicas)
        };
    }
}