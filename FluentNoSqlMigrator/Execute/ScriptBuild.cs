using Couchbase;
using FluentNoSqlMigrator.Infrastructure;

namespace FluentNoSqlMigrator.Execute;

public interface IScriptBuild
{
    
}

public class ScriptBuild : IScriptBuild, IBuildCommands
{
    private readonly string _sql;

    public ScriptBuild(string sql)
    {
        _sql = sql;
        MigrationContext.AddCommands(BuildCommands);
    }

    public List<IMigrateCommand> BuildCommands()
    {
        return new List<IMigrateCommand>
        {
            new BuildScriptCommand(_sql)
        };
    }
}

public class BuildScriptCommand : IMigrateCommand
{
    private readonly string _sql;

    public BuildScriptCommand(string sql)
    {
        _sql = sql;
    }

    public async Task Execute(IBucket bucket)
    {
        await bucket.Cluster.QueryAsync<dynamic>(_sql);
    }

    public bool IsValid(List<string> errorMessages)
    {
        if (string.IsNullOrEmpty(_sql))
        {
            errorMessages.Add("Script required");
            return false;
        }

        return true;
    }
}