using NoSqlMigrator.Infrastructure;

namespace NoSqlMigrator.Tests;

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

[Migration(3)]
public class TestMigration_3 : Migrate
{
    public override void Up()
    {
        Create.Index("ix_myIndex")
            .OnScope("myScope")
            .OnCollection("myCollection1")
            .OnField("foo1").Ascending()
            .OnField("foo2").Descending()
            .OnFieldRaw("ALL ARRAY FLATTEN_KEYS(s.day DESC, s.flight) FOR s IN schedule END")
            .Where("geo.alt > 1000")
            .UsingGsi()
            .WithNodes("127.0.0.1:8091")
            .WithDeferBuild()
            .WithNumReplicas(0);
    }

    public override void Down()
    {
        Delete.Index("ix_myIndex")
            .FromScope("myScope")
            .FromCollection("myCollection1");
    }
}

[Migration(4)]
public class TestMigration_4 : Migrate
{
    public override void Up()
    {
        Create.PrimaryIndex()
            .OnScope("myScope")
            .OnCollection("myCollection1")
            .UsingGsi()
            .WithNodes("127.0.0.1:8091")
            .WithNumReplicas(0);
    }

    public override void Down()
    {
        Delete.PrimaryIndex()
            .FromScope("myScope")
            .FromCollection("myCollection1");
    }
}

[Migration(5)]
public class TestMigration_5 : Migrate
{
    public override void Up()
    {
        Insert.Into
            .Scope("myScope")
            .Collection("myCollection2")
            .Document("doc1", new { foo = "bar", baz = "qux" })
            .Document<DateTime>("doc2", DateTime.Now);
    }

    public override void Down()
    {
        Delete.From
            .Scope("myScope")
            .Collection("myCollection2")
            .Document("doc1")
            .Document("doc2");
    }
}

[Migration(6)]
public class TestMigration_6 : Migrate
{
    public override void Up()
    {
        Execute.Sql(@"
            INSERT INTO `testmigrator`.`myScope`.`myCollection1` (KEY, VALUE)
            VALUES (""doc3"", { ""foo"" : ""bar"", ""baz"":""qux""})");
    }

    public override void Down()
    {
        Execute.Sql(@"
            DELETE FROM `testmigrator`.`myScope`.`myCollection1` USE KEYS ""doc3"";");
    }
}

[Migration(7)]
public class TestMigration_7 : Migrate
{
    public override void Up()
    {
        Update.Collection("myCollection1")
            .InScope("myScope")
            .UpsertFieldWithValue("bing", "bong")
            .RemoveField("foo");
    }

    public override void Down()
    {
        // can't really "down" this migration
        // since upsert and remove can be destructive
        Update.Collection("myCollection1")
            .InScope("myScope")
            .RemoveField("bing")
            .UpsertFieldWithValue("foo", "fooIsBack");
    }
}

[Migration(8)]
public class TestMigration_8 : Migrate
{
    public override void Up()
    {
        Update.Document("doc3")
            .InCollection("myCollection1")
            .InScope("myScope")
            .UpsertFieldWithValue("shoeSize", 13)
            .RemoveField("baz");
    }

    public override void Down()
    {
        Update.Document("doc3")
            .InCollection("myCollection1")
            .InScope("myScope")
            .RemoveField("shoeSize")
            .UpsertFieldWithValue("baz", "qux");
    }
}

public abstract class MigrateBase : Migrate
{

}

[Migration(9)]
public class TestMigration_9 : MigrateBase
{
    public override void Up()
    {
        Create.Collection("testingbaseclass")
            .InScope("_default");
    }

    public override void Down()
    {
        Delete.Collection("testingbaseclass")
            .FromScope("_default");
    }
}