using NoSqlMigrator.Infrastructure;
using NoSqlMigrator.Tests.Helpers;

namespace NoSqlMigrator.Tests.CollectionCreate;

[TestFixture]
public class CreateJustCollectionTests : MigrationTestBase<CreateOnlyCollection_Migrate>
{
    [SetUp]
    public override async Task Setup()
    {
        await base.Setup();
    }

    [Test]
    public async Task ShouldThrowExceptionMustSpecifyScope()
    {
        // arrange
        CreateOnlyCollection_Migrate.CollectionName = _random.String(10);

        // act / assert
        var ex = Assert.ThrowsAsync<Exception>(async () => await RunUp());
        Assert.That(ex.Message.Contains("Invalid migration"));
        Assert.That(ex.Message.Contains("Scope name must be specified"));
    }
}

[Migration(1000)]
public class CreateOnlyCollection_Migrate : Migrate
{
    public static string CollectionName { get; set; }

    public override void Up()
    {
        Create.Collection(CollectionName);
    }

    public override void Down()
    {
        // no need for down, since Create.Collection (without scope) should fail
    }
}