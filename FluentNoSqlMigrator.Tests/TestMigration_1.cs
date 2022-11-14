using FluentNoSqlMigrator.Infrastructure;

namespace FluentNoSqlMigrator.Tests;

[Migration(1)]
public class TestMigration_1 : Migrate
{
    public override void Up()
    {
        Create.Scope("myScope")
            .WithCollection("myCollection1");
    }

    public override void Down()
    {
        Delete.Scope("myScope");
    }
}

[Migration(2)]
public class TestMigration_2 : Migrate
{
    public override void Up()
    {
        Create.Collection("myCollection2")
            .InScope("myScope");
    }

    public override void Down()
    {
        Delete.Collection("myCollection2")
            .FromScope("myScope");
    }
}