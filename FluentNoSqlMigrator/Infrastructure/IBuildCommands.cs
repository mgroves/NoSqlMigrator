namespace FluentNoSqlMigrator.Infrastructure;

public interface IBuildCommands
{
    List<IMigrateCommand> BuildCommands();
}