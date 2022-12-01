using Couchbase;

namespace NoSqlMigrator.Runner;

public class RunSettings
{
    public DirectionEnum Direction { get; set; }
    public IBucket Bucket { get; set; }
    /// <summary>
    /// Limit is the HIGHEST that the migration will run UP to
    /// Or the LOWEST that the migration will run DOWN to
    /// (inclusive)
    /// If it's not set, ALL migrations will be run
    /// </summary>
    public int? Limit { get; set; }
}