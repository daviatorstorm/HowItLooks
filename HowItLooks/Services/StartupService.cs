using HowItLooks.Helpers;
using SQLite;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HowItLooks.Services;

public class StartupService
{
    private readonly SQLiteConnection _dbConnection;

    public StartupService()
    {
        _dbConnection = DatabaseHelper.CreateDatabaseConnection();
    }

    public void Run()
    {
        Migrate();
    }

    private void Migrate()
    {
        
    }
}
