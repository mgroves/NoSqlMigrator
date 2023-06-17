using NoSqlMigrator.Runner;
using Oakton;

#if DEBUG
Console.WriteLine("Press any key once you've attached the debugger...");
Console.ReadKey();
#endif

return CommandExecutor.ExecuteCommand<MigrationCommand>(args);