using FluentNoSqlMigrator.Scope;

namespace FluentNoSqlMigrator.Infrastructure;

public class CreateBuilder
{
    private readonly MigrationContext _context;

    internal CreateBuilder(MigrationContext context)
    {
        _context = context;
    }

    public IScopeSettingsBuild Scope(string name)
    {
        return new ScopeBuild(name, _context);
    }
}