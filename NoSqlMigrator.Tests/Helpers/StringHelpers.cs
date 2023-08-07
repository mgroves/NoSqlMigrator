namespace NoSqlMigrator.Tests.Helpers;

public static class RandomHelpers
{
    /// <summary>
    /// Generate a random string of letters.
    /// By default, will use 0-9,A-Z,a-z
    /// But you can specify a different character pool
    /// </summary>
    /// <param name="size">Length of the random string</param>
    /// <param name="characters">(Optional) Character pool to pull random characters from</param>
    /// <returns>Random string</returns>
    public static string String(this Random @this, int size, string? characters = null)
    {
        const string chars
            = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        characters ??= chars;

        return new string(Enumerable.Range(0, size)
            .Select(_ => characters[@this.Next(characters.Length)]).ToArray());
    }
}