using FluentNoSqlMigrator.Scope;

namespace FluentNoSqlMigrator.Infrastructure;

public class DeleteBuilder
{
    private readonly MigrationContext _context;

    internal DeleteBuilder(MigrationContext context)
    {
        _context = context;
    }

    public IScopeSettingsDelete Scope(string name)
    {
        return new ScopeDelete(name, _context);
    }
}
