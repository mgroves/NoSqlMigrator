using System.Reflection;
using Couchbase;
using Oakton;

namespace FluentNoSqlMigrator.Runner;

public class MigrationCommand : OaktonAsyncCommand<MigrationInput>
{
    public MigrationCommand()
    {
        Usage("Run migrations in the given direction to the given number")
             .Arguments(x => x.AssemblyName, x => x.ConnectionString, x=> x.Username, x=> x.Password, x=> x.BucketName)
             .ValidFlags( x=> x.DirectionFlag, x => x.LimitFlag);
    }

    public override async Task<bool> Execute(MigrationInput input)
    {
        IBucket bucket;
        try
        {
            var cluster = await Cluster.ConnectAsync(input.ConnectionString, input.Username, input.Password);
            bucket = await cluster.BucketAsync(input.BucketName);
        }
        catch (Exception ex)
        {
            ConsoleWriter.Write(ConsoleColor.Red, "There was an error connecting to Couchbase");
            ConsoleWriter.Write(ConsoleColor.Red, ex.Message);
            return false;
        }

        Assembly assembly;
        try
        {
            assembly = Assembly.LoadFile(input.AssemblyName);
        }
        catch (Exception ex)
        {
            ConsoleWriter.Write(ConsoleColor.Red, "There was an error loading the assembly.");
            ConsoleWriter.Write(ConsoleColor.Red, ex.Message);
            return false;
        }

        var runSettings = new RunSettings();
        runSettings.Bucket = bucket;
        if (input.DirectionFlag.HasValue)
            runSettings.Direction = input.DirectionFlag.Value;
        if (input.LimitFlag.HasValue)
            runSettings.Limit = input.LimitFlag;

        try
        {
            var runner = new MigrationRunner();
            await runner.Run(assembly, runSettings);
        }
        catch (Exception ex)
        {
            ConsoleWriter.Write(ConsoleColor.Red, "There was a problem running the migrations.");
            ConsoleWriter.Write(ConsoleColor.Red, ex.Message);
            return false;
        }

        return true;
    }
}