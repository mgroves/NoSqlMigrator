using FluentNoSqlMigrator.Infrastructure;

namespace FluentNoSqlMigrator.Execute;

public interface IScriptBuild
{
    
}

public class ScriptRun : IScriptBuild, IBuildCommands
{
    private readonly string _sql;

    public ScriptRun(string sql)
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