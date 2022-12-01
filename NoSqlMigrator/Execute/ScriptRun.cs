using NoSqlMigrator.Infrastructure;

namespace NoSqlMigrator.Execute;

public interface IScriptBuild
{
    
}

internal class ScriptRun : IScriptBuild, IBuildCommands
{
    private readonly string _sql;

    internal ScriptRun(string sql)
    {
        _sql = sql;
        MigrationContext.AddCommands(BuildCommands);
    }

    public List<IMigrateCommand> BuildCommands()
    {
        return new List<IMigrateCommand>
        {
            new ScriptRunCommand(_sql)
        };
    }
}