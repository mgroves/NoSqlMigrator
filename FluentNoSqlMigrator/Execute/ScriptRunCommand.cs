using Couchbase;
using FluentNoSqlMigrator.Infrastructure;

namespace FluentNoSqlMigrator.Execute;

public class ScriptRunCommand : IMigrateCommand
{
    private readonly string _sql;

    public ScriptRunCommand(string sql)
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