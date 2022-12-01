using Oakton;

namespace FluentNoSqlMigrator.Runner;

public class MigrationInput
{
    [Description("The name of the assembly containing your migrations. E.g. 'mymigrations.dll'")]
    public string AssemblyName { get; set; }
    
    [Description("Connection string. E.g. 'couchbase://localhost'")]
    public string ConnectionString { get; set; }
    
    [Description("Username. E.g. 'Administrator'")]
    public string Username { get; set; }

    [Description("Username. E.g. 'password123$'")]
    public string Password { get; set; }

    [Description("Bucket name. E.g. 'travel-sample'")]
    public string BucketName { get; set; }
    
    [FlagAlias("-l")]
    [Description("The highest (up) or lowest (down) migration number to run (if not specified, all will be run)")]
    public int? LimitFlag { get; set; }
    
    [FlagAlias("-d")]
    [Description("Direction of migration: Up or Down. (if not specified, 'Up' will be assumed)")]
    public DirectionEnum? DirectionFlag { get; set; }
}