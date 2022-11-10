using FluentNoSqlMigrator.Infrastructure;

namespace FluentNoSqlMigrator.Tests;

[Migration(1)]
public class TestMigration_1 : Migrate
{
    public override void Up()
    {
        Create.Scope("myScope");
    }

    public override void Down()
    {
        Delete.Scope("myScope");
    }
}

