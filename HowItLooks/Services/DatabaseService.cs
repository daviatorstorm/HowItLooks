using HowItLooks.Entities;
using SQLite;

namespace HowItLooks.Services
{
    public class DatabaseService
    {
        private SQLiteConnection _db;

        public DatabaseService()
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "local.db");
            _db = new SQLiteConnection(dbPath);
            _db.CreateTable<EnemyEntity>(); // Create table if not exists
        }

        public List<EnemyEntity> GetAllMonsters()
        {
            return _db.Table<EnemyEntity>().ToList();
        }

        public EnemyEntity GetMonsterById(int id)
        {
            return _db.Table<EnemyEntity>().FirstOrDefault(x =>  x.Id == id);
        }

        public EnemyEntity AddMonster(string name)
        {
            EnemyEntity enemy = new()
            {
                HitPointsLeft = 0,
                HitPoints = 0,
                Initiative = 0,
                Name = name
            };
            var res = _db.Insert(enemy);

            return enemy;
        }

        public void UpdateMonster(EnemyEntity enemy)
        {
            _db.Update(enemy);
        }

        public void DeleteMonster(EnemyEntity enemy)
        {
            _db.Delete(enemy);
        }
    }
}
