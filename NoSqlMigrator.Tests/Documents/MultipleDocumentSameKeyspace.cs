using NoSqlMigrator.Infrastructure;
using NoSqlMigrator.Runner;

namespace NoSqlMigrator.Tests.Documents;

public class MultipleDocumentSameKeyspace : MigrationTestBase<MultipleDocumentSameKeyspace_Migrate>
{
    [SetUp]
    public override async Task Setup()
    {
        await base.Setup();

        _collection = await _settings.Bucket.DefaultCollectionAsync();
    }

    [Test]
    public async Task Multiple_Document_Create()
    {
        // arrange
        _settings.Direction = DirectionEnum.Up;
        var dockey1 = Path.GetRandomFileName();
        var dockey2 = Path.GetRandomFileName();
        MultipleDocumentSameKeyspace_Migrate.Keys = new List<string>
        {
            dockey1, dockey2
        };

        // act
        await RunUp();

        // assert
        var exists1 = await _collection.ExistsAsync(dockey1);
        var exists2 = await _collection.ExistsAsync(dockey2);
        Assert.That(exists1.Exists, Is.EqualTo(true));
        Assert.That(exists2.Exists, Is.EqualTo(true));
    }
}

[Migration(1000)]
public class MultipleDocumentSameKeyspace_Migrate : Migrate
{
    public static List<string> Keys { get; set; } = new List<string>();

    public override void Up()
    {
        foreach (var key in Keys)
        {
            Insert.Into
                .Scope("_default")
                .Collection("_default")
                .Document(key, new
                {
                    foo = "bar-" + Path.GetRandomFileName()
                });
        }
    }

    public override void Down()
    {
        foreach (var key in Keys)
        {
            Delete.From
                .Scope("_default")
                .Collection("_default")
                .Document(key);
        }
    }
}