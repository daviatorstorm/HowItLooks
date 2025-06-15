using HowItLooks.Common;
using SQLite;

namespace HowItLooks.Helpers;

public class DatabaseHelper
{
    public static SQLiteConnection CreateDatabaseConnection() => new SQLiteConnection(Constants.DBPath);
}
