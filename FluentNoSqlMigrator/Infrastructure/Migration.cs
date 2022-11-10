namespace FluentNoSqlMigrator.Infrastructure;

public class Migration : Attribute
{
    public int MigrationNumber { get; }

    public Migration(int migrationNumber)
    {
        MigrationNumber = migrationNumber;
    }
}