using HowItLooks.Helpers;
using SQLite;
using static SQLite.SQLiteConnection;

namespace HowItLooks.Services;

public class StartupService
{
    private readonly SQLiteConnection _dbConnection;
    private readonly MigrationsService _migrationsService;

    public StartupService(MigrationsService migrationsService)
    {
        _dbConnection = DatabaseHelper.CreateDatabaseConnection();
        _migrationsService = migrationsService;
    }

    public void Run()
    {
        Migrate();
    }

    private void Migrate()
    {
        var currentVersion = _dbConnection.ExecuteScalar<int>("PRAGMA user_version;");
        _migrationsService.Migrate(currentVersion, migration =>
        {
            var result = _dbConnection.Execute(migration.GetSql());
            result = _dbConnection.Execute($"PRAGMA user_version = {migration.Version};");
        });
        List<ColumnInfo> columns = _dbConnection.GetTableInfo("Enemies");
    }
}
