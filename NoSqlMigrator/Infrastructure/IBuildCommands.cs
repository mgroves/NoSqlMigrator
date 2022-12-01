namespace NoSqlMigrator.Infrastructure;

public interface IBuildCommands
{
    List<IMigrateCommand> BuildCommands();
}