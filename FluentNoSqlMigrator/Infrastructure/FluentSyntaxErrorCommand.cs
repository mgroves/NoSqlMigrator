using Couchbase;

namespace FluentNoSqlMigrator.Infrastructure;

public class FluentSyntaxErrorCommand : IMigrateCommand
{
    private readonly string _message;

    public FluentSyntaxErrorCommand(string message)
    {
        _message = message;
    }

    public Task Execute(IBucket bucket)
    {
        throw new Exception(_message);
    }
}