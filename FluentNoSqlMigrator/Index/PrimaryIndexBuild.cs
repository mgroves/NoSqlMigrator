using FluentNoSqlMigrator.Infrastructure;

namespace FluentNoSqlMigrator.Index;

public interface IPrimaryIndexBuild
{
    /// <summary>
    /// The scope to create the primary index in (required)
    /// </summary>
    /// <param name="scopeName">Scope name</param>
    /// <returns></returns>
    IPrimaryIndexBuildScope OnScope(string scopeName);
    
    /// <summary>
    /// Create primary index in default scope (_default)
    /// </summary>
    /// <returns></returns>
    IPrimaryIndexBuildScope OnDefaultScope();
    
    /// <summary>
    /// Explicitly add USING GSI to primary index
    /// </summary>
    /// <returns></returns>
    IPrimaryIndexBuild UsingGsi();

    /// <summary>
    /// Add nodes to WITH for primary index
    /// </summary>
    /// <param name="nodes">Multiple nodes to distribute replicas. Port number is required</param>
    /// <returns></returns>
    IPrimaryIndexBuild WithNodes(params string[] nodes);
    
    /// <summary>
    /// Defer the primary index being built
    /// </summary>
    /// <returns></returns>
    IPrimaryIndexBuild WithDeferBuild();
    
    /// <summary>
    /// Number of replicas for load-balancing/HA
    /// </summary>
    /// <param name="numReplicas">Number, If the value of this property is not less than the number of index nodes in the cluster, then the index creation will fail.</param>
    /// <returns></returns>
    IPrimaryIndexBuild WithNumReplicas(uint numReplicas);
}

public interface IPrimaryIndexBuildScope
{
    /// <summary>
    /// The collection to create the index in (required)
    /// </summary>
    /// <param name="collectionName">Collection name</param>
    /// <returns></returns>
    IPrimaryIndexBuildCollection OnCollection(string collectionName);
    /// <summary>
    /// Create index in default collection (_default)
    /// </summary>
    /// <returns></returns>
    IPrimaryIndexBuildCollection OnDefaultCollection();
    /// <summary>
    /// Explicitly add USING GSI to primary index
    /// </summary>
    /// <returns></returns>
    IPrimaryIndexBuildScope UsingGsi();
    /// <summary>
    /// Add nodes to WITH for primary index
    /// </summary>
    /// <param name="nodes">Multiple nodes to distribute replicas. Port number is required</param>
    /// <returns></returns>    
    IPrimaryIndexBuildScope WithNodes(params string[] nodes);
    /// <summary>
    /// Defer the primary index being built
    /// </summary>
    /// <returns></returns>
    IPrimaryIndexBuildScope WithDeferBuild();
    /// <summary>
    /// Number of replicas for load-balancing/HA
    /// </summary>
    /// <param name="numReplicas">Number, If the value of this property is not less than the number of index nodes in the cluster, then the index creation will fail.</param>
    /// <returns></returns>
    IPrimaryIndexBuildScope WithNumReplicas(uint numReplicas);    
}

public interface IPrimaryIndexBuildCollection
{
    /// <summary>
    /// Explicitly add USING GSI to primary index
    /// </summary>
    /// <returns></returns>
    IPrimaryIndexBuildCollection UsingGsi();

    /// <summary>
    /// Add nodes to WITH for primary index
    /// </summary>
    /// <param name="nodes">Multiple nodes to distribute replicas. Port number is required</param>
    /// <returns></returns>
    IPrimaryIndexBuildCollection WithNodes(params string[] nodes);
    /// <summary>
    /// Defer the primary index being built
    /// </summary>
    /// <returns></returns>
    IPrimaryIndexBuildCollection WithDeferBuild();
    /// <summary>
    /// Number of replicas for load-balancing/HA
    /// </summary>
    /// <param name="numReplicas">Number, If the value of this property is not less than the number of index nodes in the cluster, then the index creation will fail.</param>
    /// <returns></returns>
    IPrimaryIndexBuildCollection WithNumReplicas(uint numReplicas);     
}

// TODO: add "WHERE" clause
// TODO: USING GSI
// TODO: with nodes
// TODO: deferred
public class PrimaryIndexBuild : IPrimaryIndexBuild, IPrimaryIndexBuildScope, IPrimaryIndexBuildCollection, IBuildCommands
{
    private readonly string _indexName;
    private string _collectionName;
    private string _scopeName;
    private bool _useGsi;
    private List<string> _withNodes;
    private bool _deferBuild;
    private uint? _numReplicas;

    public PrimaryIndexBuild(string indexName)
    {
        _indexName = indexName;
        _withNodes = new List<string>();
        MigrationContext.AddCommands(BuildCommands);
    }

    public IPrimaryIndexBuildScope OnScope(string scopeName)
    {
        _scopeName = scopeName;
        return this;
    }

    public IPrimaryIndexBuildScope OnDefaultScope()
    {
        _scopeName = "_default";
        return this;
    }

    public IPrimaryIndexBuildCollection OnCollection(string collectionName)
    {
        _collectionName = collectionName;
        return this;
    }

    public IPrimaryIndexBuildCollection OnDefaultCollection()
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

    IPrimaryIndexBuildCollection IPrimaryIndexBuildCollection.WithNodes(params string[] nodes)
    {
        _withNodes.AddRange(nodes);
        return this;
    }
    IPrimaryIndexBuildScope IPrimaryIndexBuildScope.WithNodes(params string[] nodes)
    {
        _withNodes.AddRange(nodes);
        return this;
    }
    IPrimaryIndexBuild IPrimaryIndexBuild.WithNodes(params string[] nodes)
    {
        _withNodes.AddRange(nodes);
        return this;
    }
    
    
    
    
    IPrimaryIndexBuildCollection IPrimaryIndexBuildCollection.WithDeferBuild()
    {
        _deferBuild = true;
        return this;
    }
    IPrimaryIndexBuildScope IPrimaryIndexBuildScope.WithDeferBuild()
    {
        _deferBuild = true;
        return this;
    }
    IPrimaryIndexBuild IPrimaryIndexBuild.WithDeferBuild()
    {
        _deferBuild = true;
        return this;
    }
    
    
    

    IPrimaryIndexBuildCollection IPrimaryIndexBuildCollection.WithNumReplicas(uint numReplicas)
    {
        _numReplicas = numReplicas;
        return this;
    }
    IPrimaryIndexBuildScope IPrimaryIndexBuildScope.WithNumReplicas(uint numReplicas)
    {
        _numReplicas = numReplicas;
        return this;
    }
    IPrimaryIndexBuild IPrimaryIndexBuild.WithNumReplicas(uint numReplicas)
    {
        _numReplicas = numReplicas;
        return this;
    }

    #endregion

    
    // I think this is a completely optional keyword
    // Since GSI is the only index option (for now)
    // Adding it for completeness
    #region USING GSI

    IPrimaryIndexBuildCollection IPrimaryIndexBuildCollection.UsingGsi()
    {
        _useGsi = true;
        return this;
    }
    IPrimaryIndexBuild IPrimaryIndexBuild.UsingGsi()
    {
        _useGsi = true;
        return this;
    }
    IPrimaryIndexBuildScope IPrimaryIndexBuildScope.UsingGsi()
    {
        _useGsi = true;
        return this;
    }
    #endregion


    public List<IMigrateCommand> BuildCommands()
    {
        return new List<IMigrateCommand>
        {
            new BuildPrimaryIndexCommand(_indexName, _scopeName, _collectionName, _useGsi, _withNodes, _deferBuild, _numReplicas)
        };
    }
}

