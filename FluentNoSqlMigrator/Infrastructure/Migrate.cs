namespace FluentNoSqlMigrator.Infrastructure;

public abstract class Migrate
{
    public MigrationContext Context { get; set; }
    
    public abstract void Up();
    public abstract void Down();

    private CreateBuilder _create;
    protected CreateBuilder Create => (_create ??= new CreateBuilder(Context));

    private DeleteBuilder _delete;
    protected DeleteBuilder Delete => (_delete ??= new DeleteBuilder(Context));
}