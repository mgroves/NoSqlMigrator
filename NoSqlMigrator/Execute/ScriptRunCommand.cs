using Couchbase;
using NoSqlMigrator.Infrastructure;

namespace NoSqlMigrator.Execute;

internal class ScriptRunCommand : IMigrateCommand
{
    private readonly string _sql;

    internal ScriptRunCommand(string sql)
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