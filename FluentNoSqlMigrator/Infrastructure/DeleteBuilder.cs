using FluentNoSqlMigrator.Collection;
using FluentNoSqlMigrator.Scope;

namespace FluentNoSqlMigrator.Infrastructure;

public class DeleteBuilder
{
    private readonly MigrationContext _context;

    internal DeleteBuilder(MigrationContext context)
    {
        _context = context;
    }

    public IScopeSettingsDelete Scope(string scopeName)
    {
        return new ScopeDelete(scopeName, _context);
    }

    public ICollectionSettingsDelete Collection(string collectionName)
    {
        return new CollectionDelete(collectionName, _context);
    }
}
