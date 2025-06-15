using HowItLooks.Common;
using SQLite;

namespace HowItLooks.Helpers;

public class DatabaseHelper
{
    public static SQLiteConnection CreateDatabaseConnection()
    {
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, Constants.DBName);
        return new SQLiteConnection(dbPath);
    }
}
