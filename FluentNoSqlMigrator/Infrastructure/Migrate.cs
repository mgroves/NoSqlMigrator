namespace FluentNoSqlMigrator.Infrastructure;

public abstract class Migrate
{
    public abstract void Up();
    public abstract void Down();

    private CreateBuilder _create;
    protected CreateBuilder Create => (_create ??= new CreateBuilder());

    private DeleteBuilder _delete;
    protected DeleteBuilder Delete => (_delete ??= new DeleteBuilder());
}