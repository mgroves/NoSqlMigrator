using FluentNoSqlMigrator.Infrastructure;

namespace FluentNoSqlMigrator.Index;

public interface IPrimaryIndexCreate
{
    /// <summary>
    /// The scope to create the primary index in (required)
    /// </summary>
    /// <param name="scopeName">Scope name</param>
    /// <returns></returns>
    IPrimaryIndexCreateScope OnScope(string scopeName);
    
    /// <summary>
    /// Create primary index in default scope (_default)
    /// </summary>
    /// <returns></returns>
    IPrimaryIndexCreateScope OnDefaultScope();
    
    /// <summary>
    /// Explicitly add USING GSI to primary index
    /// </summary>
    /// <returns></returns>
    IPrimaryIndexCreate UsingGsi();

    /// <summary>
    /// Add nodes to WITH for primary index
    /// </summary>
    /// <param name="nodes">Multiple nodes to distribute replicas. Port number is required</param>
    /// <returns></returns>
    IPrimaryIndexCreate WithNodes(params string[] nodes);
    
    /// <summary>
    /// Defer the primary index being built
    /// </summary>
    /// <returns></returns>
    IPrimaryIndexCreate WithDeferBuild();
    
    /// <summary>
    /// Number of replicas for load-balancing/HA
    /// </summary>
    /// <param name="numReplicas">Number, If the value of this property is not less than the number of index nodes in the cluster, then the index creation will fail.</param>
    /// <returns></returns>
    IPrimaryIndexCreate WithNumReplicas(uint numReplicas);
}

public interface IPrimaryIndexCreateScope
{
    /// <summary>
    /// The collection to create the index in (required)
    /// </summary>
    /// <param name="collectionName">Collection name</param>
    /// <returns></returns>
    IPrimaryIndexCreateCollection OnCollection(string collectionName);
    /// <summary>
    /// Create index in default collection (_default)
    /// </summary>
    /// <returns></returns>
    IPrimaryIndexCreateCollection OnDefaultCollection();
    /// <summary>
    /// Explicitly add USING GSI to primary index
    /// </summary>
    /// <returns></returns>
    IPrimaryIndexCreateScope UsingGsi();
    /// <summary>
    /// Add nodes to WITH for primary index
    /// </summary>
    /// <param name="nodes">Multiple nodes to distribute replicas. Port number is required</param>
    /// <returns></returns>    
    IPrimaryIndexCreateScope WithNodes(params string[] nodes);
    /// <summary>
    /// Defer the primary index being built
    /// </summary>
    /// <returns></returns>
    IPrimaryIndexCreateScope WithDeferBuild();
    /// <summary>
    /// Number of replicas for load-balancing/HA
    /// </summary>
    /// <param name="numReplicas">Number, If the value of this property is not less than the number of index nodes in the cluster, then the index creation will fail.</param>
    /// <returns></returns>
    IPrimaryIndexCreateScope WithNumReplicas(uint numReplicas);    
}

public interface IPrimaryIndexCreateCollection
{
    /// <summary>
    /// Explicitly add USING GSI to primary index
    /// </summary>
    /// <returns></returns>
    IPrimaryIndexCreateCollection UsingGsi();

    /// <summary>
    /// Add nodes to WITH for primary index
    /// </summary>
    /// <param name="nodes">Multiple nodes to distribute replicas. Port number is required</param>
    /// <returns></returns>
    IPrimaryIndexCreateCollection WithNodes(params string[] nodes);
    /// <summary>
    /// Defer the primary index being built
    /// </summary>
    /// <returns></returns>
    IPrimaryIndexCreateCollection WithDeferBuild();
    /// <summary>
    /// Number of replicas for load-balancing/HA
    /// </summary>
    /// <param name="numReplicas">Number, If the value of this property is not less than the number of index nodes in the cluster, then the index creation will fail.</param>
    /// <returns></returns>
    IPrimaryIndexCreateCollection WithNumReplicas(uint numReplicas);     
}

// TODO: add "WHERE" clause
// TODO: USING GSI
// TODO: with nodes
// TODO: deferred
public class PrimaryIndexCreate : IPrimaryIndexCreate, IPrimaryIndexCreateScope, IPrimaryIndexCreateCollection, IBuildCommands
{
    private readonly string _indexName;
    private string _collectionName;
    private string _scopeName;
    private bool _useGsi;
    private List<string> _withNodes;
    private bool _deferBuild;
    private uint? _numReplicas;

    public PrimaryIndexCreate(string indexName)
    {
        _indexName = indexName;
        _withNodes = new List<string>();
        MigrationContext.AddCommands(BuildCommands);
    }

    public IPrimaryIndexCreateScope OnScope(string scopeName)
    {
        _scopeName = scopeName;
        return this;
    }

    public IPrimaryIndexCreateScope OnDefaultScope()
    {
        _scopeName = "_default";
        return this;
    }

    public IPrimaryIndexCreateCollection OnCollection(string collectionName)
    {
        _collectionName = collectionName;
        return this;
    }

    public IPrimaryIndexCreateCollection OnDefaultCollection()
    {
        _collectionName = "_default";
        return this;
    }
    
    // I'm not sure the best place to put "WITH" clause
    // It could be only at top leve
    // But SQL++ syntax has it at the end
    // Both make sense to me for fluent syntax, so I put them everywhere
    // same is true for USING GSI
    #region WITH

    IPrimaryIndexCreateCollection IPrimaryIndexCreateCollection.WithNodes(params string[] nodes)
    {
        _withNodes.AddRange(nodes);
        return this;
    }
    IPrimaryIndexCreateScope IPrimaryIndexCreateScope.WithNodes(params string[] nodes)
    {
        _withNodes.AddRange(nodes);
        return this;
    }
    IPrimaryIndexCreate IPrimaryIndexCreate.WithNodes(params string[] nodes)
    {
        _withNodes.AddRange(nodes);
        return this;
    }
    
    
    
    
    IPrimaryIndexCreateCollection IPrimaryIndexCreateCollection.WithDeferBuild()
    {
        _deferBuild = true;
        return this;
    }
    IPrimaryIndexCreateScope IPrimaryIndexCreateScope.WithDeferBuild()
    {
        _deferBuild = true;
        return this;
    }
    IPrimaryIndexCreate IPrimaryIndexCreate.WithDeferBuild()
    {
        _deferBuild = true;
        return this;
    }
    
    
    

    IPrimaryIndexCreateCollection IPrimaryIndexCreateCollection.WithNumReplicas(uint numReplicas)
    {
        _numReplicas = numReplicas;
        return this;
    }
    IPrimaryIndexCreateScope IPrimaryIndexCreateScope.WithNumReplicas(uint numReplicas)
    {
        _numReplicas = numReplicas;
        return this;
    }
    IPrimaryIndexCreate IPrimaryIndexCreate.WithNumReplicas(uint numReplicas)
    {
        _numReplicas = numReplicas;
        return this;
    }

    #endregion

    
    // I think this is a completely optional keyword
    // Since GSI is the only index option (for now)
    // Adding it for completeness
    #region USING GSI

    IPrimaryIndexCreateCollection IPrimaryIndexCreateCollection.UsingGsi()
    {
        _useGsi = true;
        return this;
    }
    IPrimaryIndexCreate IPrimaryIndexCreate.UsingGsi()
    {
        _useGsi = true;
        return this;
    }
    IPrimaryIndexCreateScope IPrimaryIndexCreateScope.UsingGsi()
    {
        _useGsi = true;
        return this;
    }
    #endregion


    public List<IMigrateCommand> BuildCommands()
    {
        return new List<IMigrateCommand>
        {
            new PrimaryIndexCreateCommand(_indexName, _scopeName, _collectionName, _useGsi, _withNodes, _deferBuild, _numReplicas)
        };
    }
}

