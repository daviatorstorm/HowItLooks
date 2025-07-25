﻿using HowItLooks.Migrations;

namespace HowItLooks.Services
{
    public class MigrationsService
    {
        private List<BaseMigration> _migrations = new List<BaseMigration>();

        public MigrationsService()
        {
            Init();
        }

        private void Init()
        {
            _migrations.Add(new _150620251724_Initial());
        }

        public void Migrate(int currentVersion, Action<BaseMigration> iter)
        {
            var migrationList = _migrations.Where(x => x.Version > currentVersion);
            foreach (var migration in migrationList)
            {
                iter(migration);
            }
        }
    }
}
