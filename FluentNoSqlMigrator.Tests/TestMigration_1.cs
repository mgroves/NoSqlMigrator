using Couchbase.KeyValue;
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
            .OnFieldRaw("ALL ARRAY FLATTEN_KEYS(s.day DESC, s.flight) FOR s IN schedule END");
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
        Insert.Into
            .Scope("myScope")
            .Collection("myCollection2")
            .Document("doc1", new { foo = "bar", baz = "qux" })
            .Document<DateTime>("doc2", DateTime.Now);
    }

    public override void Down()
    {
        throw new NotImplementedException();
    }
}
