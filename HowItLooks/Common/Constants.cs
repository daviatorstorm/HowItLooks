namespace HowItLooks.Common;

public class Constants
{
    public const string DBName = "local.db";
    public static string DBPath = Path.Combine(FileSystem.AppDataDirectory, DBName);
}
