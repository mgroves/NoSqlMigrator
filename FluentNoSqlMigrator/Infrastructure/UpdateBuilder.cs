using FluentNoSqlMigrator.Update;

namespace FluentNoSqlMigrator.Infrastructure;

public class UpdateBuilder
{
    /// <summary>
    /// Collection to apply updates to every document within.
    /// Note: the collection needs a primary index for this to work.
    /// </summary>
    /// <param name="collectionName">Collection name</param>
    /// <returns></returns>
    public IUpdateCollection Collection(string collectionName)
    {
        return new UpdateCollection(collectionName);
    }
}