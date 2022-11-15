using FluentNoSqlMigrator.Execute;

namespace FluentNoSqlMigrator.Infrastructure;

public class ExecuteBuilder
{
    /// <summary>
    /// Execute an arbitrary SQL++ script (anything returned is discarded)
    /// </summary>
    /// <param name="sql">SQL++ script</param>
    /// <returns></returns>
    public IScriptBuild Script(string sql)
    {
        return new ScriptRun(sql);
    }
}