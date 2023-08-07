using NoSqlMigrator.Infrastructure;
using NoSqlMigrator.Tests.Helpers;

namespace NoSqlMigrator.Tests.Collection;

[TestFixture]
public class CreateCollectionNoScopeSpecifiedTests : MigrationTestBase<CreateCollectionNoScopeSpecified_Migrate>
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
        CreateCollectionNoScopeSpecified_Migrate.CollectionName = _random.String(10);

        // act / assert
        var ex = Assert.ThrowsAsync<Exception>(async () => 
            await RunUp()
        );
        Assert.That(ex.Message.Contains("Invalid migration"));
        Assert.That(ex.Message.Contains("Scope name must be specified"));
    }
}

[Migration(1000)]
public class CreateCollectionNoScopeSpecified_Migrate : Migrate
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