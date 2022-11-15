using FluentNoSqlMigrator.Documents;

namespace FluentNoSqlMigrator.Infrastructure;

public abstract class Migrate
{
    public abstract void Up();
    public abstract void Down();

    private CreateBuilder _create;
    protected CreateBuilder Create => (_create ??= new CreateBuilder());

    private DeleteBuilder _delete;
    protected DeleteBuilder Delete => (_delete ??= new DeleteBuilder());

    private DocumentCreate _insert;
    protected DocumentCreate Insert => (_insert ??= new DocumentCreate());

    private ExecuteBuilder _execute;
    protected ExecuteBuilder Execute => (_execute ??= new ExecuteBuilder());
}